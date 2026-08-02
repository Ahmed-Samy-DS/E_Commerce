using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Identity;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Identity.Services
{
    internal class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;

        }
        public async Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result<bool>.Fail(Error.NotFound("User Not Found", $"User With {email} Is Not Found"));
            }
            else
            {
                return await _userManager.CheckPasswordAsync(user, password);

            }
        }



        public async Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result<IdentityUserResult>.Fail(Error.NotFound("User Not Found", $"User With Email {email} Is Not Found"));
            }
            else
            {
                return Result<IdentityUserResult>.OK(new IdentityUserResult(user.Id, user.DisplayName, user.Email, user.UserName));
            }
        }


        public async Task<Result<IdentityUserResult>> CreateUserAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
        {

            var user = new ApplicationUser()
            {
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                UserName = registerDto.UserName,
                DisplayName = registerDto.DisplayName

            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
                return Result<IdentityUserResult>.Fail(errors);

            }

            return Result<IdentityUserResult>.OK(new IdentityUserResult(user.Id, user.Email, user.UserName, user.DisplayName));

        }

        public async Task<Result<IReadOnlyList<string>>> GetUserRolesAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Error.NotFound("User Not Found", $"User With Email {email} Is Not Found");
            }

            //var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        public async Task<Result<bool>> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }

        public async Task<Result<AddressDto>> GetUserAddressByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users.Include(x => x.Address).FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

            if (user.Address == null)
            {

                return Result<AddressDto>.Fail(Error.NotFound("Address Not Found", $"Address Of User With Email {email} Is Not Exists"));

            }

                var address = user.Address;
                return new AddressDto()
            {
                FirstName = address.FirstName,
                LastName = address.LastName,
                City = address.City,
                Country = address.Country,
                Street = address.Street,

            };

        }
    }
}

