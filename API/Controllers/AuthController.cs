﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModel.ViewModels.Auth.LogIn;
using BusinessLogic.Services.Auth;
using DataModel.ViewModels.Auth.Token;
using DataModel.ViewModels.Auth.Register;
using BusinessLogic.Services.Base;
using Microsoft.Extensions.Logging;
using NLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using API.Security;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly BaseService _baseService;
        private readonly Logger _logger;
        public AuthController(AuthService authService, BaseService baseService)
        {
            this._authService = authService;
            this._baseService = baseService;

            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        #region LogIn
        /// <summary>
        /// API endpoint to login a user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///    
        ///     {
        ///        "username": "widelyusername",
        ///        "password": "widelypassword",
        ///     }
        ///
        /// </remarks>
        /// <returns> Unauthorizied if the login fails, The jwt token as string if the login succeded</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LogInRequest request)
        {

            #region Logging
            _logger
               .WithProperty("username", request.Username)
               .WithProperty("action", "LogIn")
               .WithProperty("status", "Request")
               .Debug("{user} is logging in.", request.Username);
            #endregion
            
            var response = await this._authService.Login(request);

            if (!response.Success)
            {
                #region Logging
                _logger
                    .WithProperty("username", request.Username)
                    .WithProperty("action", "LogIn")
                    .WithProperty("status", "Failure")
                    .Debug("{msg}", response.Message);
                #endregion

                return Unauthorized(response);
            }

            #region Logging
            _logger
               .WithProperty("username", request.Username)
               .WithProperty("action", "LogIn")
               .WithProperty("status", "Success")
               .Debug("{msg}", response.Message);
            #endregion

            return Ok(response);
        }
        #endregion

        #region LogOut
        /// <summary>
        /// API endpoint to revoke token
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///        "token": "xxxxxxxxxxxxxxxxxxxxxxx"
        ///     }
        ///
        /// </remarks>
        /// <returns></returns>
        /// 
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ModulePermission("*", "*")]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshTokenRequest tokenRequest)
        {
            #region Logging
            _logger
                .WithProperty("username", this._baseService.GetUserName())
                .WithProperty("action", "Logout")
                .WithProperty("status", "Request")
                .Debug("request by token : {token}", tokenRequest.Token);
            #endregion

            if (string.IsNullOrEmpty(tokenRequest.Token))
            {
                #region Logging
                _logger
                   .WithProperty("username", this._baseService.GetUserName())
                   .WithProperty("action", "Logout")
                   .WithProperty("status", "Failure")
                   .Debug("Token is required.");
                #endregion

                return Unauthorized(new { Message = "Token is required" });
            }

            var response = await _authService.Logout(tokenRequest.Token);

            if (!response.Success)
            {
                #region Logging
                _logger
                   .WithProperty("username", this._baseService.GetUserName())
                   .WithProperty("action", "Logout")
                   .WithProperty("status", "Failure")
                   .Debug("failed by token : {token}", tokenRequest.Token);
                #endregion

                return Unauthorized(new { message = response.Message });
            }

            #region Logging
            _logger
                .WithProperty("username", this._baseService.GetUserName())
                .WithProperty("action", "Logout")
                .WithProperty("status", "Success")
                .Debug("Successfully !!");
            #endregion

            return Ok(response);
        }
        #endregion

        #region Refresh Token
        /// <summary>
        /// API endpoint to refresh token
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///        "token": "xxxxxxxxxxxxxxxxxxxxxxx"
        ///     }
        ///
        /// </remarks>
        /// <returns></returns>
        /// 
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ModulePermission("*", "*")]
        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest tokenRequest)
        {
            //var getuser = _baseService.GetUserName(); test
            #region Logging
            _logger
                .WithProperty("username", this._baseService.GetUserName())
                .WithProperty("action", "RefreshToken")
                .WithProperty("status", "Request")
                .Debug("request by token : {token}", tokenRequest.Token);
            #endregion

            if (string.IsNullOrEmpty(tokenRequest.Token))
            {
                #region Logging
                _logger
                   .WithProperty("username", this._baseService.GetUserName())
                   .WithProperty("action", "RefreshToken")
                   .WithProperty("status", "Failure")
                   .Debug("Token is required.");
                #endregion

                return Unauthorized(new { message = "Token is required" });
            }

            var response = await this._authService.RefreshToken(tokenRequest.Token);

            if (!response.Success)
            {
                #region Logging
                _logger
                   .WithProperty("username", this._baseService.GetUserName())
                   .WithProperty("action", "RefreshToken")
                   .WithProperty("status", "Failure")
                   .Debug("failed by token : {token}", tokenRequest.Token);
                #endregion

                return Unauthorized(new { message = response.Message });
            }

            #region Logging
            _logger
                .WithProperty("username", this._baseService.GetUserName())
                .WithProperty("action", "RefreshToken")
                .WithProperty("status", "Success")
                .Debug("Success by token : {token}", response.Data.RefreshToken);
            #endregion

            return Ok(response);

        }
        #endregion
        

        #region Register
        /// <summary>
        /// API endpoint to register (development)
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///        "username": "widelyuser",
        ///        "password": "widelypassword",
        ///        "fName": "นาย ไวด์ลี่",
        ///        "lName": "เน็กท์"
        ///     }
        ///
        /// </remarks>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            var response = await this._authService.Register(registerRequest);
            return Ok(response);
        }
        #endregion
    }
}
