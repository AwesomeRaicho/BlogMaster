using BlogMaster.Core.DTO;
using BlogMaster.Core.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.Core.Contracts
{
    public interface IIdentityService
    {
        public Task<IdentityResult> RegisterUserAsync(IdentityRegistrationDto identityRegistrationDto);
        public Task<IdentityResult> DeleteUserAsync(IdentityRequestDto identityRequestDto);
        public Task<IdentityResponseDto> GetUserByIdAsync(IdentityRequestDto identityRequestDto);

        public Task<ApplicationUser?> GetEntityById(Guid userId);


        public Task<SignInResponseDto> SignIn(IdentityRequestDto identityRequestDto);
        public Task LogOut();

        public Task<IdentityResult> CreateRole(IdentityRequestDto identityRequestDto);

        public Task<IdentityResult> UpdateRole(IdentityRequestDto identityRequestDto);
        public Task<IdentityResult> DeleteRole(IdentityRequestDto identityRequestDto);

        public Task<IdentityResult> AddRoleToUser(IdentityRequestDto identityRequestDto);

        public Task<IdentityResult> RemoveRoleToUser(IdentityRequestDto identityRequestDto);

        public Task<ApplicationUser?> GetByUserName(string userName); 
        public Task<bool> Exists(Guid UserId);

        public Task<IdentityResult> ConfirmEmailAsync(string userId, string token);

        public Task<IdentityResult> CreateAccount(AccountCreationRequestDto accountCreationRequestDto);

        public Task<List<IdentityResponseDto>> GetWrittersEditors();

        public Task<string?> GetStripeCustomerId(string userId);

        public Task<bool> UpdateStripeCustomerId(string userId, string stripeCustomerId);

        public Task<bool> IsCorrectPassword(string userId, string password);

        public Task<bool> SendChangePasswordEmailConfirmation(string userId);

        public Task<IdentityResult?> ChangePassword(PasswordResetDto passwordResetDto);

        //Change password and reser password will be implemented after the EmailService gets implemented.

        //public Task<IdentityResult> ChangePasswordAsync(IdentityRequestDto identityRequestDto);

        //public Task<IdentityResult> ResetPasswordAsync(IdentityRequestDto identityRequestDto);
    }
}
