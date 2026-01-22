using MediatR;
using Microsoft.AspNetCore.Mvc;
using WhatsAppBusiness.Application.DTOs;
using WhatsAppBusiness.Application.UseCases.Commands;
using WhatsAppBusiness.Application.UseCases.Queries;

namespace WhatsAppBusiness.WebAPI.Controllers;

/// <summary>
/// Controller for managing messages
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MessagesController> _logger;

    /// <summary>
    /// Initializes a new instance of the MessagesController
    /// </summary>
    public MessagesController(IMediator mediator, ILogger<MessagesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get messages for a specific conversation
    /// </summary>
    /// <param name="conversationId">The conversation ID</param>
    /// <param name="limit">Optional limit for number of messages to retrieve</param>
    /// <returns>List of messages in the conversation</returns>
    [HttpGet("conversation/{conversationId}")]
    [ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetConversationMessages(Guid conversationId, [FromQuery] int? limit = null)
    {
        _logger.LogInformation("Getting messages for conversation {ConversationId}", conversationId);
        
        var query = new GetConversationHistoryQuery(conversationId, limit);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to get messages for conversation {ConversationId}: {Error}", conversationId, result.Error);
            
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(new { error = result.Error });
            
            return StatusCode(500, new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Send a message in a conversation
    /// </summary>
    /// <param name="request">The send message request</param>
    /// <returns>Success result</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        _logger.LogInformation("Sending message to conversation {ConversationId}", request.ConversationId);
        
        var command = new SendMessageCommand
        {
            ConversationId = request.ConversationId,
            Text = request.Text,
            IsFromAI = request.IsFromAI,
            OperatorId = request.OperatorId
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            _logger.LogError("Failed to send message: {Error}", result.Error);
            
            if (result.Error?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true)
                return NotFound(new { error = result.Error });
            
            if (result.Error?.Contains("validation", StringComparison.OrdinalIgnoreCase) == true)
                return BadRequest(new { error = result.Error });
            
            return StatusCode(500, new { error = result.Error });
        }

        return StatusCode(201, new { message = "Message sent successfully" });
    }
}

/// <summary>
/// Request model for sending a message
/// </summary>
public class SendMessageRequest
{
    /// <summary>
    /// The conversation ID to send the message to
    /// </summary>
    public Guid ConversationId { get; set; }

    /// <summary>
    /// The message text content
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the message is from AI
    /// </summary>
    public bool IsFromAI { get; set; }

    /// <summary>
    /// The operator ID if sent by an operator
    /// </summary>
    public Guid? OperatorId { get; set; }
}
