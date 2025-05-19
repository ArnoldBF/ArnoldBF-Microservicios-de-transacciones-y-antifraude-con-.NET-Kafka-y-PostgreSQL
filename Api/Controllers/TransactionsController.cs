using Application.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;



namespace Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {


        private readonly CreateTransactionHandler _createTransactionHandler;
        private readonly GetTransactionByIdHandler _getTransactionByIdHandler;
        private readonly GetDailyTotalHandler _getDailyTotalHandler;

        public TransactionsController(CreateTransactionHandler createTransactionHandler,
            GetTransactionByIdHandler getTransactionByIdHandler,
            GetDailyTotalHandler getDailyTotalHandler)
        {
            _createTransactionHandler = createTransactionHandler;

            _getTransactionByIdHandler = getTransactionByIdHandler;
            _getDailyTotalHandler = getDailyTotalHandler;
        }



        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] Application.Commands.CreateTransactionCommand command)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _createTransactionHandler.Handle(command);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionById(Guid id)
        {
            if (id == Guid.Empty) return BadRequest(new { error = "El id no puede ser vacio" });
            var transaction = await _getTransactionByIdHandler.Handle(new Application.Queries.GetTransactionByIdQuery(id));
            if (transaction == null) return NotFound();

            return Ok(new
            {
                transactionExternalId = transaction.Id,
                createdAt = transaction.CreatedAt,
            });
        }

        [HttpGet("daily-total/{sourceAccountId}")]
        public async Task<IActionResult> GetDailyTotal(Guid sourceAccountId)
        {
            if (sourceAccountId == Guid.Empty) return BadRequest(new { error = "El sourceAccountId no puede ser vacio" });
            var total = await _getDailyTotalHandler.Handle(new Application.Queries.GetDailyTotalQuery(sourceAccountId));
            return Ok(new
            {
                total
            });
        }








    }
}