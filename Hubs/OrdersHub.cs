using Microsoft.AspNetCore.SignalR;
using WallsShop.Properties.Entity;

namespace WallsShop.Hubs;

using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

public class OrdersHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var user = Context.User;
        
        if (user?.Identity?.IsAuthenticated == true)
        {
            var isAgent = user.IsInRole("Agent");
            
            if (isAgent)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Agents");
                
                var agentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var agentName = user.FindFirst(ClaimTypes.Name)?.Value;
                
                Console.WriteLine($"Agent connected: {agentName} (ID: {agentId})");
            }
        }
        
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Agents");
        await base.OnDisconnectedAsync(exception);
    }
    
    public async Task NotifyAgentsNewOrder(Order order)
    {
        await Clients.Group("Agents").SendAsync("ReceiveNewOrder",order);
    }
}