using MediatR;
using Microsoft.AspNetCore.Mvc;
using WhatsAppBusiness.Application.DTOs;

namespace WhatsAppBusiness.WebAPI.Controllers;

/// <summary>
/// Controller for managing operators
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OperatorsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OperatorsController> _logger;

    /// <summary>
    /// Initializes a new instance of the OperatorsController
    /// </summary>
    public OperatorsController(IMediator mediator, ILogger<OperatorsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all operators
    /// </summary>
    /// <returns>List of operators</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<OperatorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOperators()
    {
        _logger.LogInformation("Getting all operators");
        
        // Note: We need to add a GetAllOperatorsQuery in the future
        // For now, we'll return a NotImplemented response
        return StatusCode(501, new { error = "Get operators endpoint not yet implemented" });
    }

    /// <summary>
    /// Get all online operators
    /// </summary>
    /// <returns>List of online operators</returns>
    [HttpGet("online")]
    [ProducesResponseType(typeof(List<OperatorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOnlineOperators()
    {
        _logger.LogInformation("Getting online operators");
        
        // Note: We need to add a GetOnlineOperatorsQuery in the future
        // For now, we'll return a NotImplemented response
        return StatusCode(501, new { error = "Get online operators endpoint not yet implemented" });
    }

    /// <summary>
    /// Update operator status (online/offline)
    /// </summary>
    /// <param name="id">The operator ID</param>
    /// <param name="request">The status update request</param>
    /// <returns>Success result</returns>
    [HttpPost("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOperatorStatusRequest request)
    {
        _logger.LogInformation("Updating status for operator {OperatorId} to {IsOnline}", id, request.IsOnline);
        
        // Note: We need to add an UpdateOperatorStatusCommand in the future
        // For now, we'll return a NotImplemented response
        return StatusCode(501, new { error = "Update operator status endpoint not yet implemented" });
    }
}

/// <summary>
/// Request model for updating operator status
/// </summary>
public class UpdateOperatorStatusRequest
{
    /// <summary>
    /// Indicates if the operator is online
    /// </summary>
    public bool IsOnline { get; set; }
}
