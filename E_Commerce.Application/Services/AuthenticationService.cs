using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public AuthenticationService(IIdentityService identityService, ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
        }

        public async Task<Result<bool>> CheckEmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _identityService.EmailExistsAsync(email, cancellationToken);
        }

        public async Task<Result<AddressDto>> GetCurrentUserAddressAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _identityService.GetUserAddressByEmailAsync(email, cancellationToken);
        }

        public async Task<Result<UserDto>> GetCurrentUserAsync(string email, CancellationToken cancellationToken = default)
        {
            var userResult = await _identityService.FindUserByEmailAsync(email, cancellationToken);

            var user = userResult.data;
            var roleResult = await _identityService.GetUserRolesAsync(email, cancellationToken);

            var token =  _tokenService.CreateToken(user.Id, user.Email, user.UserName, user.DisplayName, roleResult.data);
            
            return new UserDto() { DisplayName = user.DisplayName, Email = user.Email, Token = token };
        }

        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
        {
            // Get User By Email

            var userResult = await _identityService.FindUserByEmailAsync(loginDto.Email);
            if (!userResult.IsSuccess)
            {
                return Result<UserDto>.Fail(userResult.Errors);
            }

           // Check Password

           var passwordResult = await _identityService.CheckPasswordAsync(loginDto.Email, loginDto.Password, cancellationToken);
           if(!passwordResult.IsSuccess) 
           {
                return Result<UserDto>.Fail(userResult.Errors);
           }
           if(!passwordResult.data)
           {
                return Result<UserDto>.Fail(Error.Unauthorized("Invalid Email Or Password"));
           }

            // Return Result + User Dto
            var user = userResult.data;
            var rolesResult = await _identityService.GetUserRolesAsync(user.Email);
            var roles = rolesResult.data;

            var token = _tokenService.CreateToken(user.Id, user.Email, user.UserName, user.DisplayName, roles);
            return new UserDto()
            {
                Email = loginDto.Email,
                DisplayName = userResult.data.DisplayName,
                Token = token
            };
        }

        public async Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
        {
           var userResult =  await  _identityService.CreateUserAsync(registerDto, cancellationToken);

           if(!userResult.IsSuccess)
           {
                return Result<UserDto>.Fail(userResult.Errors);
           }

            var user = userResult.data;
            var rolesResult = await _identityService.GetUserRolesAsync(user.Email);
            var roles = rolesResult.data;
            var token = _tokenService.CreateToken(user.Id, user.Email, user.UserName, user.DisplayName, roles);

            return Result<UserDto>.OK(new UserDto()
           {
               Email = user.Email,
               DisplayName = user.DisplayName,
               Token = token


            });
        }
    } 
}
 