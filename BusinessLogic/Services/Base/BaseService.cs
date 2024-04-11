﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DataAccess.Repositories.UnitOfWork;
using DataModel.ViewModels.Auth.LogIn;

namespace BusinessLogic.Services.Base
{
    public class BaseService
    {
        protected internal IHttpContextAccessor _httpContextAccessor;
        protected internal IUnitOfWork _unitOfWork { get; set; }
        public BaseService(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public string GetUserID() => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        public string GetUserName() => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        public int GetRoleId() => Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role));
        public List<AppModule> GetCurrentAuthorizeModule() => JsonSerializer.Deserialize<List<AppModule>>(_httpContextAccessor.HttpContext.User.Claims
                    .FirstOrDefault(x => x.Type == "appauthorize").Value);
        public string GetIpAddress() => $"{_httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.MapToIPv4()}";
        public string GetUserAgnet() => $"{_httpContextAccessor.HttpContext.Request.Headers["User-Agent"]}";
        public string GetMachineName() => Dns.GetHostEntry(Dns.GetHostName()).HostName;
    }
}