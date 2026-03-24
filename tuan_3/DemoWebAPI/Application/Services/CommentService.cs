using AutoMapper;
using DemoWebAPI.Application.DTOs;
using DemoWebAPI.Application.Interfaces;
using DemoWebAPI.Core.Entities;
using DemoWebAPI.Core.Interfaces;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebAPI.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepo _commentRepo;
        private readonly IAppCache _cache;
        private readonly IMapper _mapper;

        public CommentService(ICommentRepo commentRepo, IAppCache cache, IMapper mapper) 
        {
            _commentRepo = commentRepo;
            _cache = cache;
            _mapper = mapper;        
        }

        public async Task<CommentBasicVM> CreateCommentAsync(Guid postId, CreateCommentDto createDto) 
        {
            // Mapping du lieu dau vao
            var commentEntity = _mapper.Map<Comment>(createDto);
            commentEntity.PostId = postId;

            // Insert du lieu vao bang du lieu
            await _commentRepo.InsertAsync(commentEntity);

            // Xoa cache
            await _cache.RemoveAsync($"comment_tree_{postId}");

            // Mapping du lieu tra ve
            return _mapper.Map<CommentBasicVM>(commentEntity);
        }

        // 2.Read


        public async Task<List<CommentTreeVM>> GetCommentsTreeLoopAsync(Guid postId) {
            string cacheKey = $"comment_tree_loop_{postId}";

            // Kiem tra cache
            var cacheData = await _cache.GetAsync<List<CommentTreeVM>>(cacheKey);
            if (cacheData != null) return cacheData;

            // Chua co cache
            var comments = await _commentRepo.GetAllCommentsCTE(postId);
            if (comments == null) return new List<CommentTreeVM>();

            // Mapping
            var allNodes = _mapper.Map<List<CommentTreeVM>>(comments);
            var tree = allNodes.Where(x => x.ParentCommentId == null).ToList();

            // Setup thoi gian luu cache
            await _cache.SetAsync(cacheKey, tree, TimeSpan.FromMinutes(15));

            return tree;
        }

        public async Task<List<CommentTreeVM>> GetCommentTreeAsync(Guid postId)
        {
            string cacheKey = $"comment_tree_{postId}";
            var cacheData = await _cache.GetAsync<List<CommentTreeVM>>(cacheKey);

            // Kiem tra cache
            if (cacheData != null) return cacheData;

            // Chua co cache
            var comments = await _commentRepo.GetAllCommentsCTE(postId);
            if (comments == null) return new List<CommentTreeVM>();

            // Mapping
            var allNodes = _mapper.Map<List<CommentTreeVM>>(comments);
            var tree = allNodes.Where(x => x.ParentCommentId == null).ToList();

            // Lưu vào Redis Cache trong 15 phút
            await _cache.SetAsync(cacheKey, tree, TimeSpan.FromMinutes(15));

            return tree;
        }


        public async Task<List<CommentFlatVM>> GetCommentsFlatAsync(Guid postId)
        {
            string cacheKey = $"comment_flat_{postId}";
            var cacheData = await _cache.GetAsync<List<CommentFlatVM>>(cacheKey);

            // Kiem tra cache
            if (cacheData != null) return cacheData;

            // Thuc hien lay comment
            var comments = await _commentRepo.GetAllCommentsCTE(postId);
            if (comments == null) return new List<CommentFlatVM>();

            // Mapping du lieu dau ra
            var flatList = _mapper.Map<List<CommentFlatVM>>(comments).ToList();


            // Luu cache
            await _cache.SetAsync(cacheKey, flatList, TimeSpan.FromMinutes(15));
            return flatList;
        }

        public async Task<List<CommentBasicVM>> GetCommentsByPostAsync(Guid postId, Guid authorId, ReadCommentDto readDto) 
        {
            string cacheKey = $"comments_post_{postId}_author_{authorId}";
            var cacheData = await _cache.GetAsync<List<CommentBasicVM>>(cacheKey);

            // Kiem tra cache
            if (cacheData != null) return cacheData;

            // Thuc hien tim du lieu
            var comments = _commentRepo.GetAllCommentsByUserInPost(postId, authorId, readDto);
            if (comments == null) return new List<CommentBasicVM>();

            // Mapping du lieu tra ve
            var commentsByPost = _mapper.Map<List<CommentBasicVM>>(comments);

            // Luu cache
            await _cache.SetAsync(cacheKey, commentsByPost, TimeSpan.FromMinutes(15));
            return commentsByPost;
        }

        // 3.Update
        public async Task<bool> UpdateCommentAsync(Guid id, UpdateCommentDto updateDto) 
        { 
            var existingComment = await _commentRepo.GetByIdAsync(id);
            if (existingComment == null) return false;
            
            _mapper.Map(updateDto, existingComment);
            await _commentRepo.UpdateAsync(existingComment);

            await _cache.RemoveAsync($"comment_tree_{existingComment.Id}");
            return true;
        }

        // 4.Delete
        public async Task<bool> DeleteCommentAsync(Guid id) 
        {
            var existingComment = await _commentRepo.GetByIdAsync(id);
            if (existingComment == null) return false;

            await _commentRepo.DeleteAsync(id);

            await _cache.RemoveAsync($"comment_tree_{existingComment.Id}");
            return true;
        }

        Task<CommentBasicVM> ICommentService.GetCommentsTreeLoopAsync(Guid postId)
        {
            throw new NotImplementedException();
        }

        Task<CommentBasicVM> ICommentService.GetCommentsByPostAsync(Guid postId, Guid authorId, ReadCommentDto readDto)
        {
            throw new NotImplementedException();
        }
    }
}
