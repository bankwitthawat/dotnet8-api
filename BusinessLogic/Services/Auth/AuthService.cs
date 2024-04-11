using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Services.Base;
using BusinessLogic.Utilities;
using DataAccess.DataContext.Entities;
using DataAccess.Repositories.Auth;
using DataAccess.Repositories.UnitOfWork;
using DataModel.ViewModels.Auth.LogIn;
using DataModel.ViewModels.Auth.Register;
using DataModel.ViewModels.Auth.Token;
using DataModel.ViewModels.Common;
using Infrastructure.Exceptions;

namespace BusinessLogic.Services.Auth
{
    public class AuthService : BaseService
    {
        private readonly IMapper _mapper;
        private readonly JwtManager _jwtManager;
        public readonly IAuthRepository _authRepository;

        private readonly IConfiguration _configuration;
        public List<AppModule> treeNode = new List<AppModule>();

        public AuthService(
            IHttpContextAccessor httpContextAccessor
            , IUnitOfWork unitOfWork
            , IAuthRepository authRepository
            , JwtManager jwtManager
            , IMapper mapper
            , IConfiguration configuration
            ) : base(httpContextAccessor, unitOfWork)
        {
            var testHttpContext = httpContextAccessor;

            _configuration = configuration;
            _mapper = mapper;
            _jwtManager = jwtManager;
            _authRepository = authRepository;

        }

        public async Task<ServiceResponse<LogInResponse>> Login(LogInRequest request)
        {
            ServiceResponse<LogInResponse> response = new ServiceResponse<LogInResponse>();
            var userRepository = _unitOfWork.AsyncRepository<Appusers>();

            //var user = await userRepository.GetAsync(_ => _.Username.ToLower() == request.Username.ToLower());
            var user = await _authRepository.GetUserRelated(request.Username);

            if (user == null)
            {
                response.Success = false;
                response.Message = "Username or Password invalid.";
            }
            else if (user.IsActive == false)
            {
                response.Success = false;
                response.Message = "This account has been suspended, please contract adminstrator.";
            }
            else if (user.LoginAttemptCount > 5)
            {
                response.Success = false;
                response.Message = "This account has been locked, please contract adminstrator.";
            }
            else if (!PasswordHashUtility.VerifyPasswordHash(request.Username, request.Password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Username or Password invalid.";

                user.LoginAttemptCount++;
                await userRepository.UpdateAsync(user);
            }
            else
            {

                //update user
                user.LastLogin = DateTime.Now;
                user.LoginAttemptCount = 0;

                if (user.PasswordExpireDate != null)
                {
                    var isPwdExpire = DateTime.Compare(DateTime.Now, user.PasswordExpireDate.Value);

                    if (isPwdExpire >= 0)
                        user.IsForceChangePwd = true;

                    response.Message = "Password has expired, please change your password.";

                }
                else
                {
                    response.Message = "Successfully. !!";
                }

                await userRepository.UpdateAsync(user);

                response.Data = await this.GetLoginUserInfo(user);
                response.Success = true;
            }

            //save all changes to db
            await _unitOfWork.CommitAsync();

            return response;
        }

        private async Task<LogInResponse> GetLoginUserInfo(Appusers user)
        {
            Int32.TryParse(_configuration.GetSection("JwtSetting:ExpireMins").Value, out int tokenExpire);
            var authTokenRepository = _unitOfWork.AsyncRepository<Authtokens>();

            //remove old refresh token
            var oldRefreshToken = await authTokenRepository.ListAsync(_ => _.UserId == user.Id);
            await authTokenRepository.RemoveRangeAsync(oldRefreshToken);

            //save new refresh token
            var refreshToken = _mapper.Map<Authtokens>(_jwtManager.GenerateRefreshToken(GetIpAddress(), GetUserAgnet(), GetMachineName()));
            refreshToken.UserId = user.Id;
            await authTokenRepository.AddAsync(refreshToken);

            var userdto = _mapper.Map<LogInResponse>(user);
            userdto.AppModule = await GetRootModules(userdto.RoleId.Value);
            userdto.RefreshToken = refreshToken.Token;
            userdto.TokenExpire = DateTime.Now.AddMinutes(tokenExpire);
            userdto.TokenTimeoutMins = ((int)userdto.TokenExpire.Subtract(DateTime.Now).TotalMinutes) - 1;
            userdto.Token = _jwtManager.CreateToken(userdto);
            return userdto;
        }

        public async Task<List<AppModule>> GetRootModules(Guid roleId)
        {
            //init DbSet
            var moduleRepo = _unitOfWork.AsyncRepository<Appmodule>();
            var permissionRepo = _unitOfWork.AsyncRepository<Apppermission>();

            //query
            //#1 ค้นหาทุก module และ permission ที่ Role นี้มีสิทธิ์
            var modules = await _authRepository.GetModulePermissionByRole(roleId);

            //#2 ค้นหาที่ตาราง permission ที่ role นี้มีสิทธิ์
            //** ตาราง permission จะเก็บ module ที่เป้น type = "basic" เท่านั้น
            var modulePermission = await permissionRepo.ListAsync(x => x.RoleId == roleId);

            //#3 ancentor is all parent related
            var ancentor = new List<Guid>();

            //#4 นำค่าที่ได้จากตาราง permission มาหา parent (จากล่างขึ้นบนสุดตาม tree structure)
            foreach (var item in modulePermission)
            {
                List<Guid> ancestors = await _authRepository.FindAncestorById(item.ModuleId);

                //#5 เมื่อเจอทุก parent และตัวเอง จะถูกเก็บในตัวแปร ancentor ไว้ก่อน
                ancentor.AddRange(ancestors);
            }

            //#6 หาก children อยู่ใน tier เดียวกัน parent จะต้องซ้ำกันเลยต้องกรอง parent ที่ซ้ำออก
            ancentor = ancentor.Distinct().ToList();

            //#7 ใช้ตาราง Appmodules ตั้งเพื่อหาทุก module โดยจะกรองจะตัวแปร ancentor
            //**ทีนี้เราจะรู้แล้วว่า parent of children ตามสิทธิ์มีอะไรบ้าง
            var rootNode = modules.Where(x => ancentor.Any(r => r == x.ID)).ToList();

            //#8 ใช้ recursion มาทำงานเพื่อสร้าง tree structure และส่งออกไปเพื่อ render menu บนหน้าจอ
            //** ให้ front end ใช้ function recursive หาตาม tree นี้เองว่า menu ไหนมีสิทธิ์อะไรบ้างเช่น create/edit/view/delete
            var result = await this.GetModuleTreeList(null, rootNode);
            return result;

        }

        public async Task<List<AppModule>> GetModuleTreeList(AppModule appModule, List<AppModule> rootNode)
        {

            List<AppModule> mList = (from r in rootNode
                                     where appModule == null ? r.ParentID == null : r.ParentID == appModule.ID
                                     select new AppModule()
                                     {
                                         ID = r.ID,
                                         AuthCode = r.AuthCode,
                                         Title = r.Title,
                                         Subtitle = r.Subtitle,
                                         Type = r.Type,
                                         Icon = r.Icon,
                                         Path = r.Path,
                                         Sequence = r.Sequence,
                                         ParentID = r.ParentID,

                                         IsAccess = r.IsAccess,
                                         IsCreate = r.IsCreate,
                                         IsView = r.IsView,
                                         IsEdit = r.IsEdit,
                                         IsDelete = r.IsDelete,
                                         IsActive = r.IsActive,

                                     }).ToList();

            foreach (var item in mList)
            {
                item.Children = await GetModuleTreeList(item, rootNode);
            }

            return mList;
        }

        public async Task<ServiceResponse<LogInResponse>> RefreshToken(string oldToken)
        {
            ServiceResponse<LogInResponse> response = new ServiceResponse<LogInResponse>();

            var username = GetUserID();
            var user = await _authRepository.GetUserRelatedByToken(oldToken);

            if (string.IsNullOrEmpty(username))
            {
                response.Data = null;
                response.Success = false;
                response.Message = "Username is empty in request.";
            }
            else if (string.IsNullOrEmpty(oldToken))
            {
                response.Data = null;
                response.Success = false;
                response.Message = "Token is required";
            }
            else if (user == null)
            {
                response.Data = null;
                response.Success = false;
                response.Message = "Token not found.";
            }
            else
            {
                response.Data = await this.GetLoginUserInfo(user);
                response.Success = true;
                response.Message = "Successfully. !!";
            }

            await _unitOfWork.CommitAsync();

            return response;
        }

        public async Task<ServiceResponse<bool>> Register(RegisterRequest registerRequest)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>();

            try
            {
                var userRepository = _unitOfWork.AsyncRepository<Appusers>();

                var user = await userRepository.GetAsync(_ => _.Username.ToLower() == registerRequest.username.ToLower());

                if (user != null)
                {
                    response.Data = false;
                    response.Success = false;
                    response.Message = "มีชื่อผู้ใช้งานนี้แล้วในระบบ";
                }

                PasswordHashUtility.CreatePasswordHash(registerRequest.username, registerRequest.password, out byte[] passwordHash, out byte[] passwordSalt);

                var newUser = new Appusers()
                {
                    Username = registerRequest.username,
                    PasswordSalt = passwordSalt,
                    PasswordHash = passwordHash,
                    Fname = registerRequest.fName,
                    Lname = registerRequest.lName,
                    CreatedDate = DateTime.Now,
                    CreatedBy = "System",
                    IsActive = true
                };

                await userRepository.AddAsync(newUser);
                await _unitOfWork.CommitAsync();

                response.Data = true;
                response.Success = true;
                response.Message = "ลงทะเบียนสำเร็จ !!";
            }
            catch (Exception)
            {
                throw;
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> Logout(string oldtoken)
        {
            ServiceResponse<bool> response = new ServiceResponse<bool>() { Success = false, Data = false };
            var authTokenRepository = _unitOfWork.AsyncRepository<Authtokens>();

            var token = await authTokenRepository.GetAsync(_ => _.Token == oldtoken);

            if (string.IsNullOrEmpty(oldtoken))
            {

                response.Data = false;
                response.Success = false;
                response.Message = "Token is required";
            }
            else if (token == null)
            {
                response.Data = false;
                response.Success = false;
                response.Message = "Token not found.";
            }
            else
            {
                response.Data = true;
                response.Success = true;
                response.Message = "Logout successfully. !!";
                await authTokenRepository.RemoveAsync(token);
            }

            await _unitOfWork.CommitAsync();

            return response;
        }

    }
}
