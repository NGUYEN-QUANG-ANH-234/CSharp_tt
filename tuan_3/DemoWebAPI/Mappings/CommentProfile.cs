using AutoMapper;
using DemoWebAPI.Models.DTOs;
using DemoWebAPI.Models.Entities;
using DemoWebAPI.Models.ViewModels;

namespace DemoWebAPI.Mappings
{
    public class CommentProfile : Profile
    {
        public CommentProfile() 
        {
            CreateMap<CreateCommentDto, Comment>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateCommentDto, Comment>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Comment, CommentBasicVM>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FName} {src.User.LName}" : string.Empty))
                .ForMember(dest => dest.ReplyCount,opt => opt.MapFrom(src => src.Replies != null ? src.Replies.Count : 0));

            CreateMap<Comment, CommentTreeVM>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FName} {src.User.LName}" : string.Empty))
                .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies != null ? src.Replies : new List<Comment>()))
                .ForMember(dest => dest.ReplyCount, opt => opt.MapFrom(src => src.Replies != null ? src.Replies.Count : 0));

            CreateMap<Comment, CommentFlatVM>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FName} {src.User.LName}" : string.Empty))
                .ForMember(dest => dest.ReplyCount, opt => opt.MapFrom(src => src.Replies != null ? src.Replies.Count : 0));

        }
    }
}
