using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Vodo.Application.Requests.Contractors.CreateContractor;
using Vodo.Application.Requests.Contractors.DeleteContractor;
using Vodo.Application.Requests.Contractors.GetContractors;
using Vodo.Application.Requests.Contractors.UpdateContractor;
using Vodo.Models;

namespace Vodo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractorsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ContractorsController> _logger;

        public ContractorsController(IMediator mediator, ILogger<ContractorsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Получить список подрядчиков
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Contractor>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Contractor>>> GetList()
        {
            try
            {
                var result = await _mediator.Send(new GetContractorsQuery());
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка подрядчиков");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ошибка при получении списка: {ex.Message}");
            }
        }

        /// <summary>
        /// Создать нового подрядчика
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateContractorCommand command)
        {
            try
            {
                var id = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetList), new { id }, id);
            }
            catch (ValidationException vex)
            {
                return BadRequest(vex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании подрядчика");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ошибка при создании: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновить подрядчика
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> Update(Guid id, [FromBody] UpdateContractorCommand command)
        {
            if (id != command.Id)
                return BadRequest("Id in route and body do not match");

            try
            {
                var updatedId = await _mediator.Send(command);
                return Ok(updatedId);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Contractor with Id {id} not found.");
            }
            catch (Exception ex)
            {
                // Логирование ex можно добавить здесь
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ошибка при обновлении: {ex.Message}");
            }
        }

        /// <summary>
        /// Удалить подрядчика
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteContractorCommand { Id = id });
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Попытка удалить несуществующего подрядчика с Id {ContractorId}", id);
                return NotFound($"Contractor with Id {id} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении подрядчика с Id {ContractorId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ошибка при удалении: {ex.Message}");
            }
        }
    }
}
