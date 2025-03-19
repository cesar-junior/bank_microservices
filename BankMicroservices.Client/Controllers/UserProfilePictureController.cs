using BankMicroservices.Client.Data.ValueObjects;
using BankMicroservices.Client.Repository;
using BankMicroservices.Client.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BankMicroservices.Client.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class UserProfilePictureController : ControllerBase
    {
        private IUserProfilePictureRepository _repository;

        public UserProfilePictureController(IUserProfilePictureRepository repository)
        {
            _repository = repository ?? throw new
                ArgumentNullException(nameof(repository));
        }

        [HttpGet("{userId}")]
        [Authorize]
        public async Task<ActionResult<UserProfilePictureVO>> GetByUserId(string userId)
        {
            var userClaimId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            if (User.IsInRole(Role.Admin) || userId == userClaimId)
            {
                var vo = await _repository.GetByUserId(userId);
                if (vo == null) return NotFound();
                return Ok(vo);
            }
            return BadRequest();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserProfilePictureVO>> Create([FromBody] UserProfilePictureVO vo)
        {
            if (vo == null) return BadRequest();
            var user = await _repository.Create(vo);
            return Ok(user);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<UserProfilePictureVO>> Update([FromBody] UserProfilePictureVO vo)
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            if (vo.UserId.IsNullOrEmpty())
                vo.UserId = userId ?? "";
            if (vo == null || !User.IsInRole(Role.Admin) && vo.UserId != userId) return BadRequest();
            try
            {
                var user = await _repository.Update(vo);
                return Ok(user);

            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
