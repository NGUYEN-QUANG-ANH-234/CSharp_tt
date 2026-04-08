using AutoMapper;
using DemoWebAPI.Application.DTOs;
using DemoWebAPI.Application.Interfaces;
using DemoWebAPI.Core.Entities;
using DemoWebAPI.Core.Interfaces;
using DemoWebAPI.Infrastructure.Data;
using DemoWebAPI.Infrastructure.Persistence.Implementations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sprache;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

namespace DemoWebAPI.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepo _postRepo;
        private readonly IAppCache _appCache;
        private readonly IMapper _mapper;
        private readonly ILockService _lockService;        
        
        public PostService(IPostRepo postRepo, IAppCache appCache, IMapper mapper, ILockService lockService) 
        { 
            _postRepo = postRepo;
            _appCache = appCache;
            _mapper = mapper;
            _lockService = lockService;
        }

        public async Task<PostBasicVM> GetPostByIdAsync(Guid postId) 
        {
            // Cache key
            string cacheKey = $"post_{postId}";

            // Kiếm thử trong cache
            var cacheData = await _appCache.GetAsync<PostBasicVM>(cacheKey);
            if (cacheData is not null) return cacheData;

            // Nếu chưa có thì ta sẽ truy vấn !
            return await _lockService.GetWithLockAsync(cacheKey, async () =>
                {
                    // Double check
                    var data = await _appCache.GetAsync<PostBasicVM>(cacheKey);
                    if (data is not null) return data;
                    
                    // Nếu vẫn chưa có thì query
                    var post = await _postRepo.GetByIdAsync(postId);
                    if (post is null)
                    {
                        var emptyResult = new PostBasicVM();
                        await _appCache.SetAsync(cacheKey, emptyResult, TimeSpan.FromMinutes(1)); 
                        return emptyResult;
                    }
                    var result = _mapper.Map<PostBasicVM>(post);
                    await _appCache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15));

                    return result;                        
                }
            );
        }
        
        public async Task<List<PostBasicVM>> GetPostsByUserIdAsync(Guid userId, QueryPostDto queryPostDto)
        {
            // Cache key
            string cacheKey = $"posts_by_user_{userId}";

            // Kiếm thử trong cache
            var cacheData = await _appCache.GetAsync<List<PostBasicVM>>(cacheKey);
            if (cacheData is not null) return cacheData;

            // Nếu chưa có thì ta sẽ truy vấn !
            return await _lockService.GetWithLockAsync(cacheKey, async () =>
            {
                // Double check
                var data = await _appCache.GetAsync<List<PostBasicVM>>(cacheKey);
                if (data is not null) return data;

                int page = queryPostDto.Page;
                int pageSize = queryPostDto.PageSize;
                string? sortBy = queryPostDto.SortBy;
                string? searchTitle = queryPostDto.SearchTitle;
                bool? isDescending = queryPostDto.IsDescending;

                // Nếu vẫn chưa có thì query
                var post = await _postRepo.GetPostsByAuthorId(userId, page, pageSize, sortBy, searchTitle, isDescending);
                if (post is null || !post.Any())
                {
                    var emptyResult = new List<PostBasicVM>();
                    await _appCache.SetAsync(cacheKey, emptyResult, TimeSpan.FromMinutes(1));
                    return emptyResult;
                }
                var result = _mapper.Map<List<PostBasicVM>>(post);
                await _appCache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15));

                return result;
            }
            );
        }

        public async Task<bool> DeletePostAsync(Guid postId) 
        {
            var existingComment = await _postRepo.GetByIdAsync(postId);
            if (existingComment is null) return false;

            await _postRepo.DeleteAsync(postId);

            await _appCache.RemoveAsync($"post_{postId}");
            return true;
        }

        public async Task<bool> UpdatePostAsync(Guid postId, UpdatePostDto updatePostDto) 
        {
            string cacheKey = $"post_{postId}";

            var existingPost = await _postRepo.GetByIdAsync(postId);
            if (existingPost is null) return false;

            _mapper.Map(updatePostDto, existingPost);
            await _postRepo.UpdateAsync(existingPost);

            await _appCache.RemoveAsync($"post_{postId}");

            var result = _mapper.Map<PostBasicVM>(existingPost);
            await _appCache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15));
            return true;
        }


        public async Task<PostBasicVM> CreatePostAsync(CreatePostDto createPostDto)
        {            
            if (createPostDto == null || createPostDto.UserId == Guid.Empty)
            {
                return new PostBasicVM();
            }

            var newPost = _mapper.Map<Post>(createPostDto);

            string cacheKey = $"post_{newPost.Id}";

            await _postRepo.InsertAsync(newPost);
            await _appCache.RemoveAsync(cacheKey);

            await _appCache.SetAsync(cacheKey, newPost, TimeSpan.FromMinutes(15));
            return _mapper.Map<PostBasicVM>(newPost);
        }
    }
}
