
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Vodo.Application.Requests.Divisions.CreateDivision;
using Vodo.Application.Requests.Divisions.DeleteDivision;
using Vodo.Application.Requests.Divisions.GetDivisions;
using Vodo.Application.Requests.Divisions.UpdateDivision;
using Vodo.Models;

namespace Vodo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DivisionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DivisionsController> _logger;

        public DivisionsController(IMediator mediator, ILogger<DivisionsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Получить список подразделений
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Division>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Division>>> GetList()
        {
            try
            {
                var result = await _mediator.Send(new GetDivisionsQuery());
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка подразделений");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ошибка при получении списка: {ex.Message}");
            }
        }

        /// <summary>
        /// Создать новое подразделение
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateDivisionCommand command)
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
                _logger.LogError(ex, "Ошибка при создании подразделения");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ошибка при создании: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновить подразделение
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> Update(Guid id, [FromBody] UpdateDivisionCommand command)
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
                return NotFound($"Division with Id {id} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении подразделения с Id {DivisionId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ошибка при обновлении: {ex.Message}");
            }
        }

        /// <summary>
        /// Удалить подразделение
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteDivisionCommand { Id = id });
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Попытка удалить несуществующее подразделение с Id {DivisionId}", id);
                return NotFound($"Division with Id {id} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении подразделения с Id {DivisionId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ошибка при удалении: {ex.Message}");
            }
        }
    }
}