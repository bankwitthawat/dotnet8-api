using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Services;
using BusinessLogic.Services.AppRole;
using BusinessLogic.Services.AppUser;
using BusinessLogic.Services.Auth;
using BusinessLogic.Services.Base;
using BusinessLogic.Services.UserProfile;
using BusinessLogic.Utilities;
using DataAccess.DataContext;
using DataAccess.Repositories.Approles;
using DataAccess.Repositories.Appusers;
using DataAccess.Repositories.Auth;
using DataAccess.Repositories.Base;
using DataAccess.Repositories.UnitOfWork;
using DataAccess.Repositories.UserProfile;
using Infrastructure.AutoMapper;

namespace API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>))
                .AddScoped<IAuthRepository, AuthRepository>()
                .AddScoped<IAppusersRepository, AppusersRepository>()
                .AddScoped<IApprolesRepository, ApprolesRepository>()
                .AddScoped<IUserProfileRepository, UserProfileRepository>()
                //add new repository here.
                ;
        }

        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            return services
                .AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            #region SQL Server
            // return services.AddDbContext<DatabaseContext>(options =>
            // {
            //     var connetionString = configuration.GetConnectionString("DefaultConnection");
            //     options.UseSqlServer(connetionString);
            // });
            #endregion

            #region MySQL
            return services.AddDbContext<DatabaseContext>(options =>
            {
               var connetionString = configuration.GetConnectionString("DefaultConnection");
               options.UseMySql(connetionString, ServerVersion.AutoDetect(connetionString));
            });
            #endregion
        }

        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            return services
                .AddScoped<JwtManager>()
                .AddScoped<BaseService>()
                .AddScoped<AuthService>()
                .AddScoped<ApprolesService>()
                .AddScoped<AppusersService>()
                .AddScoped<UserProfileService>()
                // add new service here.
                ;
        }

        public static IServiceCollection AddHttpContext(this IServiceCollection services)
        {
            return services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            return services.AddAutoMapper(typeof(AutoMapperProfile));
        }
    }
}
