using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.API.Controllers
{

    public class AuthenticationController : ApiBaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        //Login 
        [HttpPost("Login")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto, CancellationToken cancellationToken)
        {
            return  ToActionResult(await _authenticationService.LoginAsync(loginDto, cancellationToken));
        }
         
        //Register  
        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto, CancellationToken cancellationToken)
        {
                
            
            return  ToActionResult(await _authenticationService.RegisterAsync(registerDto, cancellationToken));
        }


        // Check Email Exists
        [HttpGet("EmailExists")]
        public async Task<ActionResult<bool>> CheckEmail([FromQuery] string email, CancellationToken cancellationToken)
        {
            return ToActionResult(await _authenticationService.CheckEmailExistsAsync(email, cancellationToken));
        }

        // Get Current User
        [Authorize]
        [HttpGet("CurrentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken cancellationToken)
        {
            //var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedAccessException("No Email Claim Found");
            return ToActionResult(await _authenticationService.GetCurrentUserAsync(GetEmailFromToken(), cancellationToken));
        }

        // Get Current User Address
        [Authorize]
        [HttpGet("UserAddress")]
        public async Task<ActionResult<AddressDto>> GetCurrentUserAddress(CancellationToken cancellationToken)
        {
            //var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedAccessException("No Email Claim Found");

            return ToActionResult(await _authenticationService.GetCurrentUserAddressAsync(GetEmailFromToken(), cancellationToken));
        }
    }
}
