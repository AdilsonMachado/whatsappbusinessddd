using MediatR;
using Microsoft.AspNetCore.Mvc;
using WhatsAppBusiness.Application.DTOs;
using WhatsAppBusiness.Application.UseCases.Commands;
using WhatsAppBusiness.Application.UseCases.Queries;

namespace WhatsAppBusiness.WebAPI.Controllers;

/// <summary>
/// Controller for managing conversations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConversationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ConversationsController> _logger;

    /// <summary>
    /// Initializes a new instance of the ConversationsController
    /// </summary>
    public ConversationsController(IMediator mediator, ILogger<ConversationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all active conversations
    /// </summary>
    /// <returns>List of active conversations</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<ConversationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetActiveConversations()
    {
        _logger.LogInformation("Getting all active conversations");
        
        var query = new GetActiveConversationsQuery();
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to get active conversations: {Error}", result.Error);
            return StatusCode(500, new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get a conversation by ID
    /// </summary>
    /// <param name="id">The conversation ID</param>
    /// <returns>The conversation details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ConversationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetConversationById(Guid id)
    {
        _logger.LogInformation("Getting conversation {ConversationId}", id);
        
        var query = new GetConversationByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            _logger.LogWarning("Conversation {ConversationId} not found", id);
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Assign a conversation to an operator
    /// </summary>
    /// <param name="id">The conversation ID</param>
    /// <param name="request">The assignment request</param>
    /// <returns>Success result</returns>
    [HttpPost("{id}/assign")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AssignToOperator(Guid id, [FromBody] AssignConversationRequest request)
    {
        _logger.LogInformation("Assigning conversation {ConversationId} to operator {OperatorId}", id, request.OperatorId);
        
        var command = new AssignConversationToOperatorCommand
        {
            ConversationId = id,
            OperatorId = request.OperatorId
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to assign conversation {ConversationId}: {Error}", id, result.Error);
            
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(new { error = result.Error });
            
            return BadRequest(new { error = result.Error });
        }

        return Ok(new { message = "Conversation assigned successfully" });
    }

    /// <summary>
    /// Close a conversation
    /// </summary>
    /// <param name="id">The conversation ID</param>
    /// <returns>Success result</returns>
    [HttpPost("{id}/close")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CloseConversation(Guid id)
    {
        _logger.LogInformation("Closing conversation {ConversationId}", id);
        
        // Note: We need to add a CloseConversationCommand in the future
        // For now, we'll return a NotImplemented response
        return StatusCode(501, new { error = "Close conversation endpoint not yet implemented" });
    }

    /// <summary>
    /// Get conversations assigned to a specific operator
    /// </summary>
    /// <param name="operatorId">The operator ID</param>
    /// <returns>List of conversations for the operator</returns>
    [HttpGet("operator/{operatorId}")]
    [ProducesResponseType(typeof(List<ConversationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetOperatorConversations(Guid operatorId)
    {
        _logger.LogInformation("Getting conversations for operator {OperatorId}", operatorId);
        
        var query = new GetOperatorConversationsQuery(operatorId);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to get operator conversations: {Error}", result.Error);
            return StatusCode(500, new { error = result.Error });
        }

        return Ok(result.Value);
    }
}

/// <summary>
/// Request model for assigning a conversation to an operator
/// </summary>
public class AssignConversationRequest
{
    /// <summary>
    /// The operator ID to assign the conversation to
    /// </summary>
    public Guid OperatorId { get; set; }
}
