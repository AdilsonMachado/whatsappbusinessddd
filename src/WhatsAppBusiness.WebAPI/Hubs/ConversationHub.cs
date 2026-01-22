using Microsoft.AspNetCore.SignalR;
using WhatsAppBusiness.Application.DTOs;
using WhatsAppBusiness.Domain.ValueObjects;

namespace WhatsAppBusiness.WebAPI.Hubs;

/// <summary>
/// SignalR hub for real-time conversation updates
/// </summary>
public class ConversationHub : Hub
{
    private readonly ILogger<ConversationHub> _logger;

    /// <summary>
    /// Initializes a new instance of the ConversationHub
    /// </summary>
    public ConversationHub(ILogger<ConversationHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        
        if (exception != null)
        {
            _logger.LogError(exception, "Client disconnected with error: {ConnectionId}", Context.ConnectionId);
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Join a conversation room to receive updates
    /// </summary>
    /// <param name="conversationId">The conversation ID to join</param>
    public async Task JoinConversation(string conversationId)
    {
        _logger.LogInformation("Client {ConnectionId} joining conversation {ConversationId}", 
            Context.ConnectionId, conversationId);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        await Clients.Caller.SendAsync("JoinedConversation", conversationId);
    }

    /// <summary>
    /// Leave a conversation room
    /// </summary>
    /// <param name="conversationId">The conversation ID to leave</param>
    public async Task LeaveConversation(string conversationId)
    {
        _logger.LogInformation("Client {ConnectionId} leaving conversation {ConversationId}", 
            Context.ConnectionId, conversationId);
        
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        await Clients.Caller.SendAsync("LeftConversation", conversationId);
    }

    /// <summary>
    /// Join operator room to receive operator-specific updates
    /// </summary>
    /// <param name="operatorId">The operator ID</param>
    public async Task JoinOperatorRoom(string operatorId)
    {
        _logger.LogInformation("Operator {OperatorId} joined with connection {ConnectionId}", 
            operatorId, Context.ConnectionId);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, $"operator_{operatorId}");
        await Clients.Caller.SendAsync("JoinedOperatorRoom", operatorId);
    }

    /// <summary>
    /// Broadcast a new message to conversation participants
    /// </summary>
    /// <param name="conversationId">The conversation ID</param>
    /// <param name="message">The message DTO</param>
    public async Task NotifyNewMessage(Guid conversationId, MessageDto message)
    {
        _logger.LogInformation("Broadcasting new message to conversation {ConversationId}", conversationId);
        
        await Clients.Group($"conversation_{conversationId}")
            .SendAsync("NewMessage", conversationId, message);
    }

    /// <summary>
    /// Broadcast operator status change
    /// </summary>
    /// <param name="operatorId">The operator ID</param>
    /// <param name="isOnline">Whether the operator is online</param>
    public async Task NotifyOperatorStatusChanged(Guid operatorId, bool isOnline)
    {
        _logger.LogInformation("Broadcasting operator {OperatorId} status change: {IsOnline}", 
            operatorId, isOnline);
        
        await Clients.All.SendAsync("OperatorStatusChanged", operatorId, isOnline);
    }

    /// <summary>
    /// Broadcast conversation assignment
    /// </summary>
    /// <param name="conversationId">The conversation ID</param>
    /// <param name="operatorId">The operator ID</param>
    public async Task NotifyConversationAssigned(Guid conversationId, Guid operatorId)
    {
        _logger.LogInformation("Broadcasting conversation {ConversationId} assigned to operator {OperatorId}", 
            conversationId, operatorId);
        
        await Clients.Group($"operator_{operatorId}")
            .SendAsync("ConversationAssigned", conversationId, operatorId);
        
        await Clients.Group($"conversation_{conversationId}")
            .SendAsync("ConversationAssigned", conversationId, operatorId);
    }

    /// <summary>
    /// Broadcast conversation status change
    /// </summary>
    /// <param name="conversationId">The conversation ID</param>
    /// <param name="status">The new conversation status</param>
    public async Task NotifyConversationStatusChanged(Guid conversationId, ConversationStatus status)
    {
        _logger.LogInformation("Broadcasting conversation {ConversationId} status change: {Status}", 
            conversationId, status);
        
        await Clients.Group($"conversation_{conversationId}")
            .SendAsync("ConversationStatusChanged", conversationId, status.ToString());
        
        await Clients.All.SendAsync("ConversationStatusChanged", conversationId, status.ToString());
    }
}
