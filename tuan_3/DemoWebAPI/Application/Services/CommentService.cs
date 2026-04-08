using AutoMapper;
using DemoWebAPI.Application.DTOs;
using DemoWebAPI.Application.Interfaces;
using DemoWebAPI.Core.Entities;
using DemoWebAPI.Core.Interfaces;
using DemoWebAPI.Infrastructure.Data;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

namespace DemoWebAPI.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepo _commentRepo;
        private readonly IAppCache _cache;
        private readonly IMapper _mapper;
        private readonly ILockService _lockService;

        public CommentService(ICommentRepo commentRepo, IAppCache cache, IMapper mapper, ILockService lockService)
        {
            _commentRepo = commentRepo;
            _cache = cache;
            _mapper = mapper;
            _lockService = lockService;
        }

        public async Task<CommentBasicVM> CreateCommentAsync(CreateCommentDto createCommentDto)
        {
            if (createCommentDto == null || createCommentDto.UserId == Guid.Empty)
            {
                return new CommentBasicVM();
            }

            // Mapping du lieu dau vao
            var commentEntity = _mapper.Map<Comment>(createCommentDto);
            

            // Insert du lieu vao bang du lieu
            await _commentRepo.InsertAsync(commentEntity);

            // Xoa cache
            await _cache.RemoveAsync($"comment_by_post_{commentEntity.PostId}");

            // Mapping du lieu tra ve
            return _mapper.Map<CommentBasicVM>(commentEntity);            
        }

        // 2.Read
        public async Task<List<CommentTreeVM>> GetCommentTreeAsync(Guid postId)
        {
            string cacheKey = $"comment_tree_{postId}";
            var cacheData = await _cache.GetAsync<List<CommentTreeVM>>(cacheKey);

            // Kiem tra cache
            if (cacheData is not null) return cacheData;

            // Chua co cache
            var comments = await _commentRepo.GetAllCommentsCTE(postId);
            if (comments is null) return new List<CommentTreeVM>();

            // Mapping 
            // Thuộc tính điều hướng User ở trạng thái tạo rỗng
            var allNodes = _mapper.Map<List<CommentTreeVM>>(comments);

            // Do cơ chế tự detect quan hệ của EF nên sẽ tự động thực hiện load mối quan hệ và lấp đầy thuộc tính con User
            //var tree = allNodes.Where(x => x.ParentCommentId == null).ToList();

            // Chủ động xây dựng comment tree từ khối comment truy vấn trả về
            var tree = BuildTree(allNodes);

            // Lưu vào Redis Cache trong 15 phút
            await _cache.SetAsync(cacheKey, tree, TimeSpan.FromMinutes(15));

            return tree;
        }


        public async Task<List<CommentFlatVM>> GetCommentsFlatAsync(Guid postId)
        {
            string cacheKey = $"comment_flat_{postId}";
            var cacheData = await _cache.GetAsync<List<CommentFlatVM>>(cacheKey);

            //// Kiem tra cache
            if (cacheData is not null) return cacheData;

            return await _lockService.GetWithLockAsync(cacheKey, async () =>
            {
                // Check lần 2 đề các client đến sau lấy trực tiếp từ cache đã cập nhật
                var data = await _cache.GetAsync<List<CommentFlatVM>>(cacheKey);
                if (data is not null) return data;

                // Thuc hien lay comment
                var comments = await _commentRepo.GetAllCommentsCTE(postId);

                // Kiếm tra nếu comments null hoặc không có phần tử nào 
                if (comments == null || !comments.Any())
                {
                    // Khởi tạo mảng rỗng (tránh miss cache liên tục)
                    var emptyList = new List<CommentFlatVM>();
                    await _cache.SetAsync(cacheKey, emptyList, TimeSpan.FromMinutes(1)); // Cache rỗng ngắn
                    return emptyList;
                }

                // Mapping du lieu dau ra
                var flatList = _mapper.Map<List<CommentFlatVM>>(comments).ToList();

                    // Luu cache
                    await _cache.SetAsync(cacheKey, flatList, TimeSpan.FromMinutes(15));
                    return flatList;
            });
        }

        public async Task<List<CommentBasicVM>> GetCommentsByPostAsync(Guid postId, Guid authorId, QueryCommentDto queryCommentDto)
        {
            string cacheKey = $"comments_post_{postId}_author_{authorId}";
            var cacheData = await _cache.GetAsync<List<CommentBasicVM>>(cacheKey);

            // Kiem tra cache
            if (cacheData is not null) return cacheData;

            // Thuc hien tim du lieu
            var comments = _commentRepo.GetAllCommentsByUserInPost(postId, authorId, queryCommentDto.Page, queryCommentDto.PageSize, queryCommentDto?.SortBy, queryCommentDto?.SearchText, queryCommentDto?.IsDescending);

            if (comments is null) return new List<CommentBasicVM>();

            // Mapping du lieu tra ve
            var commentsByPost = _mapper.Map<List<CommentBasicVM>>(comments);

            // Luu cache
            await _cache.SetAsync(cacheKey, commentsByPost, TimeSpan.FromMinutes(15));
            return commentsByPost;
        }

        // 3.Update
        public async Task<bool> UpdateCommentAsync(Guid id, UpdateCommentDto updateCommentDto)
        {
            var existingComment = await _commentRepo.GetByIdAsync(id);
            if (existingComment is null) return false;

            _mapper.Map(updateCommentDto, existingComment);
            await _commentRepo.UpdateAsync(existingComment);

            await _cache.RemoveAsync($"comment_tree_{existingComment.Id}");
            return true;
        }

        // 4.Delete
        public async Task<bool> DeleteCommentAsync(Guid id)
        {
            var existingComment = await _commentRepo.GetByIdAsync(id);
            if (existingComment is null) return false;

            await _commentRepo.DeleteAsync(id);

            await _cache.RemoveAsync($"comment_tree_{existingComment.Id}");
            return true;
        }

        public async Task<List<Comment>> GetCommentTreeLoopAsync(Guid postId)
        {
            string cacheKey = $"comment_tree_loop_{postId}";
            var cacheData = await _cache.GetAsync<List<Comment>>(cacheKey);

            if (cacheData is not null) return cacheData;

            var comments = await _commentRepo.GetAllCommentsCTE(postId);

            if (comments is null) return new List<Comment>();

            var loopCommentTree = _mapper.Map<List<Comment>>(comments);
            await _cache.SetAsync(cacheKey, loopCommentTree, TimeSpan.FromMinutes(15));

            return loopCommentTree;
        }

        // Hàm bổ trợ dựng comment tree
        private List<CommentTreeVM> BuildTree(List<CommentTreeVM> allNodes)
        {
            // Tạo Dictionary để tìm kiếm nhanh theo Id
            var dic = allNodes.ToDictionary(n => n.Id);
            var rootNodes = new List<CommentTreeVM>();

            foreach (var node in allNodes)
            {
                if (node.ParentCommentId == null || !dic.ContainsKey(node.ParentCommentId.Value))
                {
                    // Nếu không có cha -> nó là gốc
                    rootNodes.Add(node);
                }
                else
                {
                    // Nếu có cha -> tìm cha trong Dictionary và add vào danh sách Children
                    var parent = dic[node.ParentCommentId.Value];
                    if (parent.Replies== null) parent.Replies = new List<CommentTreeVM>();
                    parent.Replies.Add(node);
                }
            }
            return rootNodes;
        }
    }
}
