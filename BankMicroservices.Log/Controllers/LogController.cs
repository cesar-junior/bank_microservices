using BankMicroservices.Log.Data.ValueObjects;
using BankMicroservices.Log.Repository;
using BankMicroservices.Log.Utils;
using BankMicroservices.Log.Utils.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankMicroservices.Log.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private ILogRepository _repository;

        public LogController(ILogRepository repository)
        {
            _repository = repository ?? throw new
                ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        [Authorize(Roles = Role.Admin)]
        public async Task<ActionResult<PagedResponse<List<LogVO>>>> GetWithOffsetPagination(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber >= 0 ? pageNumber : 0;
            pageSize = pageSize > 0 ? pageSize : 1;

            var logs = await _repository.GetWithOffsetPagination(pageNumber, pageSize);
            return Ok(logs);
        }
    }
}
