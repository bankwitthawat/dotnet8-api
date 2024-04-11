using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.DataContext.Entities;
using DataModel.ViewModels.Approles.ListView;
using DataModel.ViewModels.Appusers.ItemView;
using DataModel.ViewModels.Appusers.ListView;
using DataModel.ViewModels.Auth.LogIn;
using DataModel.ViewModels.Auth.Token;

namespace Infrastructure.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Authtokens, RefreshToken>();
            //.ForMember(dest => dest.IsExpired, src => src.MapFrom(s => s.ExpiresOn >= DateTime.UtcNow));
            CreateMap<RefreshToken, Authtokens>();
            CreateMap<Appusers, LogInResponse>()
                .ForMember(dest => dest.UserId, src => src.MapFrom(s => s.Id))
                .ForMember(dest => dest.RoleId, src => src.MapFrom(s => s.Role == null ? (Guid?)null : s.Role.Id))
                .ForMember(dest => dest.RoleName, src => src.MapFrom(s => s.Role == null ? string.Empty : s.Role.Name))
                .ForMember(dest => dest.RoleDescription, src => src.MapFrom(s => s.Role == null ? string.Empty : s.Role.Description))
                .ForMember(dest => dest.Title, src => src.MapFrom(s => s.Title == null ? string.Empty : s.Title))
                .ForMember(dest => dest.FirstName, src => src.MapFrom(s => s.Fname == null ? string.Empty : s.Fname))
                .ForMember(dest => dest.LastName, src => src.MapFrom(s => s.Lname == null ? string.Empty : s.Lname))
                ;

            // App role
            CreateMap<Approles, AppRoleResponse>()
                .ForMember(dest => dest.CreatedDate, src => src.MapFrom(s => s.CreatedDate == null ? string.Empty : s.CreatedDate.Value.ToString("dd/MM/yyyy")))
                .ForMember(dest => dest.ModifiedDate, src => src.MapFrom(s => s.ModifiedDate == null ? string.Empty : s.ModifiedDate.Value.ToString("dd/MM/yyyy")))
                ;

            // App user
            CreateMap<Appusers, AppUserListViewResponse>()
                .ForMember(dest => dest.RoleId, src => src.MapFrom(s => s.Role == null ? (Guid?)null : s.Role.Id))
                .ForMember(dest => dest.RoleName, src => src.MapFrom(s => s.Role == null ? string.Empty : s.Role.Name))
                .ForMember(dest => dest.RoleDescription, src => src.MapFrom(s => s.Role == null ? string.Empty : s.Role.Description))
                .ForMember(dest => dest.FullName, src => src.MapFrom(s => $"{s.Title}{s.Fname} {s.Lname}"))
                ;

            CreateMap<Appusers, AppUserItemViewResponse>()
                .ForMember(dest => dest.RoleId, src => src.MapFrom(s => s.Role == null ? (Guid?)null : s.Role.Id))
                .ForMember(dest => dest.RoleName, src => src.MapFrom(s => s.Role == null ? string.Empty : s.Role.Name))
                .ForMember(dest => dest.RoleDescription, src => src.MapFrom(s => s.Role == null ? string.Empty : s.Role.Description))
                //.ForMember(dest => dest.BirthDate, src => src.MapFrom(s => s.BirthDate == null ? string.Empty : s.BirthDate.Value.ToString("dd/MM/yyyy")))
                ;

        }
    }
}
