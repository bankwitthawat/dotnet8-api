using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLogic.Services.Auth;
using DataAccess.DataContext;
using DataModel.ViewModels.Auth.LogIn;

namespace API.Security
{
    public class ModulePermission : TypeFilterAttribute
    {
        public ModulePermission(string module, string permission) : base(typeof(ModulePermissionAuthorizeFilter))
        {
            Arguments = new object[] { module, permission };
        }
    }

    public class ModulePermissionAuthorizeFilter : IAuthorizationFilter
    {
        private readonly DatabaseContext _context;
        private readonly string _appModule;
        private readonly string _action;
        private readonly AuthService _authService;

        private bool hasPermission = false;

        public ModulePermissionAuthorizeFilter(
            string AppModule
            , string action
            , DatabaseContext context
            , AuthService authService
            )
        {
            _context = context;
            _appModule = AppModule;
            _action = action;
            _authService = authService;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (_appModule == "*") return;
            try
            {
                var moduleArr = Array.ConvertAll(_appModule.Split(','), p => p.Trim());
                var currentRefreshToken = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "refreshtoken").Value;
                var dbRefreshToken = _context.Authtokens.FirstOrDefault(x => x.Token == currentRefreshToken);

                if (dbRefreshToken == null)
                {
                    context.Result = new UnauthorizedResult(); //401
                    return;
                }

                Guid.TryParse(context.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out Guid userId);
                
                var userName = context.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                var _user = _context.Appusers?.FirstOrDefault(x => x.Id == userId && x.Username == userName);
                if (_user == null)
                {
                    context.Result = new UnauthorizedResult(); //401
                    return;
                }

                var appauthorize = this._authService.GetRootModules(_user.RoleId.Value).Result;

                this.HasAuthorize(appauthorize, moduleArr, _action);
                bool isAuthorized = this.hasPermission;

                if (!isAuthorized)
                {
                    context.Result = new ForbidResult("Access Denied."); //403
                }
            }
            catch
            {
                context.Result = new UnauthorizedResult();
            }
        }

        private void HasAuthorize(List<AppModule> appModules, string[] appModuleName, string permission)
        {
            if (this.hasPermission)
            {
                return;
            }

            foreach (var item in appModules)
            {
                bool hasAccess = appModuleName.Any(x => !string.IsNullOrEmpty(item.AuthCode) && x.ToUpper() == item.AuthCode.ToUpper()) && item.IsAccess == true;

                if (hasAccess)
                {
                    this.hasPermission = permission.ToUpper() switch
                    {
                        "*" => true,
                        "CREATE" when item.IsCreate == true => true,
                        "EDIT" when item.IsEdit == true => true,
                        "VIEW" when item.IsView == true => true,
                        "DELETE" when item.IsDelete == true => true,
                        _ => false
                    };

                    if (this.hasPermission)
                        return;

                }

                if (item.Children != null)
                {
                    HasAuthorize(item.Children, appModuleName, permission);
                }
            }

            return;
        }
    }
}
