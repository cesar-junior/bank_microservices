using BankMicroservices.Transfer.Data.ValueObjects;
using BankMicroservices.Transfer.Repository;
using BankMicroservices.Transfer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BankMicroservices.Transfer.Controllers
{
    [Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private ITransferRepository _repository;

        public TransferController(ITransferRepository repository)
        {
            _repository = repository ?? throw new
                ArgumentNullException(nameof(repository));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<TransferVO>> GetTransferById(long id)
        {
            var userClaimId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var vo = await _repository.GetTransferById(id);
            if (vo == null) return NotFound();
            if (User.IsInRole(Role.Admin) || userClaimId == vo.SenderUserId|| userClaimId == vo.ReceiverUserId)
            {
                return Ok(vo);
            }

            return BadRequest();
        }

        [HttpGet]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<List<TransferVO>>> GetTransfersByUser(string userId = "")
        {
            var userClaimId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            if(userId.IsNullOrEmpty()) userId = userClaimId ?? throw new ArgumentNullException();
            if (User.IsInRole(Role.Admin) || userClaimId == userId)
            {
                List<TransferVO> transferVOs = await _repository.GetTransfersByUser(userId);
                return Ok(transferVOs);
            }

            return BadRequest();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TransferVO>> Create([FromBody] SendTransferVO vo)
        {
            string? token = Request.Headers["Authorization"];
            var userClaimId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var userEmail = User.Claims.Where(u => u.Type == "email")?.FirstOrDefault()?.Value;
            if (vo == null || vo.SenderUserId != userClaimId) return BadRequest();
            var user = await _repository.Create(vo, token ?? "", userEmail ?? "");
            return Ok(user);
        }

        [HttpPatch]
        [Authorize]
        public async Task<ActionResult<TransferVO>> ReturnTransfer(long id)
        {
            string? token = Request.Headers["Authorization"];
            var userClaimId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value ?? "";
            var done = await _repository.ReturnTransfer(id, userClaimId, token ?? "");
            return Ok(done);
        }

    }
}
