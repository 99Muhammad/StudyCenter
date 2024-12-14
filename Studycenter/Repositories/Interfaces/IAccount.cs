using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto;



namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IAccount
    {
        public Task<DtoUserResponse> Register(DtoUserRegisterRequest registerDto, ModelStateDictionary modelState);
        public Task<DtoUserResponse> Register(DtoAdminRegisterRequest registerDto, ModelStateDictionary modelState);
        public Task<DtoUserResponse> Login(DtoUserLoginRequest loginDto);
        public Task Logout(ClaimsPrincipal userPrincipal);
        public Task<DtoUserResponse> RefreshToken(TokenDto tokenDto);
        Task<bool> ResetPasswordAsync(ResetPasswordReqDTO resetPasswordDto);
        Task<bool> ForgotPasswordAsync(ForgotPasswordReqDTO forgotPasswordDto);
        public Task<bool> ConfirmEmailAsync(string email, string code);
       
    }
}
