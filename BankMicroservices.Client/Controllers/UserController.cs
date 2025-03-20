using BankMicroservices.Client.Data.ValueObjects;
using BankMicroservices.Client.Repository;
using BankMicroservices.Client.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMicroservices.Client.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserRepository _repository;

        public UserController(IUserRepository repository)
        {
            _repository = repository ?? throw new
                ArgumentNullException(nameof(repository));
        }

        [HttpGet("{userId}")]
        [Authorize]
        public async Task<ActionResult<UserVO>> GetByUserId(string userId)
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

        [HttpGet]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<List<UserVO>>> GetByNameOrEmail(string name = "", string email = "")
        {
            var userClaimId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            List<UserVO> userVOs = await _repository.GetByNameOrEmail(name, email);
            return Ok(userVOs);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserVO>> Create([FromBody] UserVO vo)
        {
            if (vo == null) return BadRequest();
            var user = await _repository.Create(vo);
            return Ok(user);
        }

        [HttpGet("{userId}/{quantity}")]
        [Authorize]
        public async Task<ActionResult<bool>> UserHasBalance(string userId, float quantity)
        {
            var userClaimsId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            
            if (!User.IsInRole(Role.Admin) && userId != userClaimsId) return BadRequest();
            var hasBalance = await _repository.UserHasBalance(userId, quantity);
            return Ok(hasBalance);
        }

        [HttpPatch]
        [Authorize]
        public async Task<ActionResult<UserVO>> TransferBalance(string senderUserId, string receiverUserId, float quantity)
        {
            var userClaimsId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            
            if (!User.IsInRole(Role.Admin) && senderUserId != userClaimsId) return BadRequest();
            var userVO = await _repository.TransferBalance(senderUserId, receiverUserId, quantity);
            return Ok(userVO);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<UserVO>> Update([FromBody] UserVO vo)
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            bool isAdmin = User.IsInRole(Role.Admin);
            if (vo == null || !isAdmin && vo.UserId != userId) return BadRequest();
            var user = await _repository.Update(vo, isAdmin);
            return Ok(user);
        }
    }
}
