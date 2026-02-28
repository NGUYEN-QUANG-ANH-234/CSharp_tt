using entity_framework_core.Data;
using entity_framework_core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace entity_framework_core.Repositories
{
    public class CommentRepo
    {
        private readonly MyDbContext _dbContext;
        public CommentRepo(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Insert(List<Comment> comment)
        {
            _dbContext.comments.AddRange(comment);
        }

        public List<Comment> Get()
        {
            return _dbContext.comments.Include(p => p).ToList();
        }

        public void UpdatePost(List<Comment> comment)
        {
            _dbContext.comments.UpdateRange(comment);
        }

        public void DeletePost(int id)
        {
            var post = _dbContext.comments.Find(id);
            if (post != null)
            {
                _dbContext.comments.Remove(post);
            }
        }
    }
}
