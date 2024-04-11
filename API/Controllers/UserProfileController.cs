using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Security;
using BusinessLogic.Services.UserProfile;
using DataModel.ViewModels.UserProfile;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserProfileController : ControllerBase
    {
        private readonly UserProfileService _userProfileService;

        public UserProfileController(UserProfileService userProfileService)
        {
            this._userProfileService = userProfileService;
        }

        [ModulePermission("*", "*")]
        [HttpPost("force-change-password")]
        public async Task<IActionResult> ForceChangePassword(UserProfileForceChangePasswordRequest request)
        {
            var result = await this._userProfileService.ForceChangePassword(request);
            return Ok(result);
        }

        [ModulePermission("*", "*")]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(UserProfileChangePasswordRequest request)
        {
            var result = await this._userProfileService.ChangePassword(request);
            return Ok(result);
        }

        [ModulePermission("*", "*")]
        [HttpGet("getuserprofile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var result = await this._userProfileService.GetUserProfile();
            return Ok(result);
        }

        [ModulePermission("*", "*")]
        [HttpPut("updateuserprofile")]
        public async Task<IActionResult> UpdateUserProfile(UserProfileUpdateRequest request)
        {
            var result = await this._userProfileService.UpdateUserProfile(request);
            return Ok(result);
        }
    }
}
