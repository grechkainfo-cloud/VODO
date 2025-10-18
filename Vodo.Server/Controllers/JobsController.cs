using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Vodo.Application.Requests.Jobs.CreateJob;
using Vodo.Application.Requests.Jobs.DeleteJob;
using Vodo.Application.Requests.Jobs.GetJobs;
using Vodo.Application.Requests.Jobs.UpdateJob;
using Vodo.Models;
using Microsoft.Extensions.Logging;

namespace Vodo.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<JobsController> _logger;

        public JobsController(IMediator mediator, ILogger<JobsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Получить список работ
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Job>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Job>>> GetList()
        {
            try
            {
                var result = await _mediator.Send(new GetJobsQuery());
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка работ");
                //return StatusCode(StatusCodes.Status500InternalServerError, $"Ошибка при получении списка: {ex.Message}");

                return this.BadRequest(ex);
            }
        }

        /// <summary>
        /// Создать новую работу
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateJobCommand command)
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
                _logger.LogError(ex, "Ошибка при создании работы");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ошибка при создании: {ex.Message}");
            }
        }

        /// <summary>
        /// Обновить работу
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Guid>> Update(Guid id, [FromBody] UpdateJobCommand command)
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
                return NotFound($"Job with Id {id} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении работы с Id {JobId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ошибка при обновлении: {ex.Message}");
            }
        }

        /// <summary>
        /// Удалить работу
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteJobCommand { Id = id });
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Попытка удалить несуществующую работу с Id {JobId}", id);
                return NotFound($"Job with Id {id} not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении работы с Id {JobId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Ошибка при удалении: {ex.Message}");
            }
        }
    }
}
