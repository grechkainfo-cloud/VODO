using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Vodo.Application.Requests.JobObjects.CreateJobObject;
using Vodo.Application.Requests.JobObjects.DeleteJobObject;
using Vodo.Application.Requests.JobObjects.UpdateJobObject;
using Vodo.Models;
using Microsoft.Extensions.Logging;
using Vodo.Application.Requests.JobObjects.GetJobObjects;

namespace Vodo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobObjectsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<JobObjectsController> _logger;

        public JobObjectsController(IMediator mediator, ILogger<JobObjectsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Получить список объектов работ (JobObjects)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<JobObject>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<JobObject>>> GetList()
        {
            try
            {
                var result = await _mediator.Send(new GetJobObjectsQuery());
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка объектов работ");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ошибка при получении списка: {ex.Message}");
            }
        }

        /// <summary>
        /// Создать новый объект работ
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateJobObjectCommand command)
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
                _logger.LogError(ex, "Ошибка при создании объекта работ");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ошибка при создании: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновить объект работ
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> Update(Guid id, [FromBody] UpdateJobObjectCommand command)
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
                return NotFound($"JobObject with Id {id} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении объекта работ с Id {JobObjectId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ошибка при обновлении: {ex.Message}");
            }
        }

        /// <summary>
        /// Удалить объект работ
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteJobObjectCommand { Id = id });
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Попытка удалить несуществующий объект работ с Id {JobObjectId}", id);
                return NotFound($"JobObject with Id {id} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении объекта работ с Id {JobObjectId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ошибка при удалении: {ex.Message}");
            }
        }
    }
}
