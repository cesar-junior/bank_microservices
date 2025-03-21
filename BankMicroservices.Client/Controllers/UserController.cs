﻿using BankMicroservices.Client.Data.ValueObjects;
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
            var userIsAdmin = User.Claims.Where(u => u.Type == "role" && u.Value == Role.Admin)?.FirstOrDefault() != null;
            if (userIsAdmin || userId == userClaimId)
            {
                var vo = await _repository.GetByUserId(userId);
                if (vo == null) return NotFound();
                return Ok(vo);
            }
            return BadRequest();
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<UserVO>>> GetByNameOrEmail(string name = "", string email = "")
        {
            var userIsAdmin = User.Claims.Where(u => u.Type == "role" && u.Value == Role.Admin)?.FirstOrDefault() != null;
            if (!userIsAdmin) return Forbid();

            var userClaimId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            List<UserVO> userVOs = await _repository.GetByNameOrEmail(name, email);
            return Ok(userVOs);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserVO>> Create([FromBody] CreateUserVO vo)
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
            var userIsAdmin = User.Claims.Where(u => u.Type == "role" && u.Value == Role.Admin)?.FirstOrDefault() != null;

            if (!userIsAdmin && userId != userClaimsId) return BadRequest();
            try
            {
                var hasBalance = await _repository.UserHasBalance(userId, quantity);
                return Ok(hasBalance);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
            
        }

        [HttpPatch]
        [Authorize]
        public async Task<ActionResult<UserVO>> TransferBalance([FromForm] string senderUserId, [FromForm] string receiverUserId, [FromForm] float quantity)
        {
            var userClaimsId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            var userIsAdmin = User.Claims.Where(u => u.Type == "role" && u.Value == Role.Admin)?.FirstOrDefault() != null;

            if (!userIsAdmin && senderUserId != userClaimsId) return BadRequest();
            var userVO = await _repository.TransferBalance(senderUserId, receiverUserId, quantity);
            return Ok(userVO);
        }

        [HttpPut]
        [Authorize]
        public async Task<ActionResult<UserVO>> Update([FromBody] UserVO vo)
        {
            var userId = User.Claims.Where(u => u.Type == "sub")?.FirstOrDefault()?.Value;
            bool isAdmin = User.Claims.Where(u => u.Type == "role" && u.Value == Role.Admin)?.FirstOrDefault() != null;
            if (vo == null || !isAdmin && vo.UserId != userId) return BadRequest();
            var user = await _repository.Update(vo, isAdmin);
            return Ok(user);
        }
    }
}
