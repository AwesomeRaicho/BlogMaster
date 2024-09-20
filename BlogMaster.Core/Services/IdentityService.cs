﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using BlogMaster.Core.Contracts;
using BlogMaster.Core.DTO;
using BlogMaster.Core.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace BlogMaster.Core.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly string _domainName;


        public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IEmailService emailService, IConfiguration configuration )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _domainName = configuration["Domain:DomainName"] ?? "";
        }

        public async Task<IdentityResult> AddRoleToUser(IdentityRequestDto identityRequestDto)
        {
            if(identityRequestDto == null || identityRequestDto.UserId == null || string.IsNullOrEmpty(identityRequestDto.RoleName))
            {
                throw new ArgumentNullException(nameof(identityRequestDto));
            }

            var user = await _userManager.FindByIdAsync(identityRequestDto.UserId.Value.ToString());

            bool roleExists = await _roleManager.RoleExistsAsync(identityRequestDto.RoleName);

            if(!roleExists || user == null)
            {
                throw new InvalidOperationException("Role or User does not exist.");
            }
            return await _userManager.AddToRoleAsync(user, identityRequestDto.RoleName);

        }

        public async Task<IdentityResult> RemoveRoleToUser(IdentityRequestDto identityRequestDto)
        {
            if (identityRequestDto == null || identityRequestDto.UserId == null || string.IsNullOrEmpty(identityRequestDto.RoleName))
            {
                throw new ArgumentNullException(nameof(identityRequestDto));
            }

            var user = await _userManager.FindByIdAsync(identityRequestDto.UserId.Value.ToString());

            bool roleExists = await _roleManager.RoleExistsAsync(identityRequestDto.RoleName);

            if (!roleExists || user == null)
            {
                throw new InvalidOperationException("Role or User does not exist.");
            }
            return await _userManager.RemoveFromRoleAsync(user, identityRequestDto.RoleName);
        }

        public async Task<IdentityResult> CreateRole(IdentityRequestDto identityRequestDto)
        {
            if(identityRequestDto == null)
            {
                throw new ArgumentNullException(nameof(identityRequestDto));
            }

            var role = new ApplicationRole()
            {
                Id = Guid.NewGuid(),
                Name = identityRequestDto.RoleName,

            };

            return await _roleManager.CreateAsync(role);

        }

        public async Task<IdentityResult> DeleteRole(IdentityRequestDto identityRequestDto)
        {
            if (identityRequestDto == null || string.IsNullOrEmpty(identityRequestDto.RoleName))
            {
                throw new ArgumentNullException(nameof(identityRequestDto), "Role name must be provided.");
            }

            var role = await _roleManager.FindByNameAsync(identityRequestDto.RoleName);

            if (role == null)
            {
                throw new InvalidOperationException("This Role does not exist.");
            }

            return await _roleManager.DeleteAsync(role);
        }

        public async Task<IdentityResult> DeleteUserAsync(IdentityRequestDto identityRequestDto)
        {
            if(identityRequestDto == null || string.IsNullOrEmpty(identityRequestDto.UserName)) 
            {
                throw new ArgumentNullException(nameof(identityRequestDto), "User name cannot be empty");
            }

            var user = await _userManager.FindByNameAsync(identityRequestDto.UserName);

            if(user == null)
            {
                throw new InvalidOperationException("This user does not exist.");
            }

            return await _userManager.DeleteAsync(user);
        }

        public async Task<IdentityResponseDto> GetUserByIdAsync(IdentityRequestDto identityRequestDto)
        {
            if(identityRequestDto == null || identityRequestDto.UserId == null)
            {
                throw new ArgumentNullException(nameof(identityRequestDto), "User Id needs to be provided to search by ID.");
            }

            var user = await _userManager.FindByIdAsync(identityRequestDto.UserId.Value.ToString());

            if(user == null)
            {
                throw new InvalidOperationException("User does not exist");
            }

            return new IdentityResponseDto()
            {
                EmailAddress = user.Email,
                UserId = user.Id,
                UserName = user.UserName,
            };
        }

        public async Task LogOut()
        {

            await _signInManager.SignOutAsync();

            throw new NotImplementedException();
        }

        public async Task<IdentityResult> RegisterUserAsync(IdentityRegistrationDto identityRegistrationDto)
        {
            if(identityRegistrationDto == null || string.IsNullOrEmpty(identityRegistrationDto.Username) || string.IsNullOrEmpty(identityRegistrationDto.EmailAddress) || string.IsNullOrEmpty(identityRegistrationDto.Password) || string.IsNullOrEmpty(identityRegistrationDto.ConfirmPassword))
            {
                throw new ArgumentNullException("All parameters must be filled to register");
            }

            if(identityRegistrationDto.ConfirmPassword != identityRegistrationDto.Password)
            {
                throw new InvalidOperationException("Password and password confirmation does not match");
            }

            ApplicationUser user = new ApplicationUser()
            {
                Email = identityRegistrationDto.EmailAddress,
                UserName = identityRegistrationDto.Username,
                EmailConfirmed = false,
            };

            var result = await _userManager.CreateAsync(user, identityRegistrationDto.Password);

            if(result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var userId = user.Id;

                var callbackUrl = $"{_domainName}/identity/confirmemail?userId={userId}&token={Uri.EscapeDataString(token)}";


                if (callbackUrl != null)
                {
                    await _emailService.SendEmailConfirmation(user.Email, user.UserName, callbackUrl);
                }
            }



            return result;
        }

        public async Task<SignInResponseDto> SignIn(IdentityRequestDto identityRequestDto)
        {
            if (identityRequestDto == null || string.IsNullOrEmpty(identityRequestDto.UserName) || string.IsNullOrEmpty(identityRequestDto.Password))
            {
                throw new ArgumentNullException(nameof(identityRequestDto));
            }

            var user = await _userManager.FindByNameAsync(identityRequestDto.UserName);

            if (user == null)
            {
                throw new InvalidOperationException("User does not exist.");
            }

            if (!user.EmailConfirmed)
            {
                //await _signInManager.SignOutAsync();

                return new SignInResponseDto
                {
                    IsSeccess = false,
                    ErrorMessage = "Email needs to be confirmed"
                };
            }

            var result = await _signInManager.PasswordSignInAsync(user, identityRequestDto.Password, isPersistent: false, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                return new SignInResponseDto
                {
                    IsSeccess = true
                };
            }
            else if (result.IsLockedOut)
            {
                return new SignInResponseDto
                {
                    IsSeccess = false,
                    ErrorMessage = "Account is locked due to too many failed login attempts."
                };
            }
            else
            {
                return new SignInResponseDto
                {
                    IsSeccess = false,
                    ErrorMessage = "Invalid login attempt."
                };
            }
        }

        public async Task<IdentityResult> UpdateRole(IdentityRequestDto identityRequestDto)
        {
            if(identityRequestDto == null || string.IsNullOrEmpty(identityRequestDto.RoleName) || identityRequestDto.RoleId == null)
            {
                throw new ArgumentNullException("All fields must be filled to change a role.");
            }

            var role = await _roleManager.FindByIdAsync(identityRequestDto.RoleId.Value.ToString());

            if(role == null)
            {
                throw new InvalidOperationException("Roile does not exist.");
            }

            role.Name = identityRequestDto.RoleName;

            return await _roleManager.UpdateAsync(role);

        }

        public async Task<ApplicationUser?> GetEntityById(Guid userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }

        public async Task<bool> Exists(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user != null;
        }

        public async Task<ApplicationUser?> GetByUserName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);

        }

        public async Task<IdentityResult> ConfirmEmailAsync(string userId, string token)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new IdentityResult() { };
            }
            return await _userManager.ConfirmEmailAsync(user, token);
        }
    }
}
