
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

public class ProductViewTracker 
{
    private readonly ConcurrentDictionary<int, HashSet<string>> _views = new();
    private readonly ConcurrentDictionary<string, int> _connections = new();

    public int AddViewer(int productId, string connectionId)
    {
        var viewers = _views.GetOrAdd(productId, _ => new HashSet<string>());

        lock (viewers)
        {
            viewers.Add(connectionId);
        }

        _connections[connectionId] = productId;
        return viewers.Count;
    }

    public int RemoveViewer(string connectionId)
    {
        if (!_connections.TryRemove(connectionId, out var productId))
            return 0;

        if (!_views.TryGetValue(productId, out var viewers))
            return 0;

        lock (viewers)
        {
            viewers.Remove(connectionId);
            return viewers.Count;
        }
    }

    public int GetViewers(int productId)
    {
        return _views.TryGetValue(productId, out var viewers)
            ? viewers.Count
            : 0;
    }
}





public class ProductViewHub : Hub
{
    private readonly ProductViewTracker _tracker;

    public ProductViewHub(ProductViewTracker tracker)
    {
        _tracker = tracker;
    }

    public async Task JoinProduct(int productId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"product-{productId}");

        var count = _tracker.AddViewer(productId, Context.ConnectionId);

        await Clients.Group($"product-{productId}")
            .SendAsync("ViewersUpdated", count);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var count = _tracker.RemoveViewer(Context.ConnectionId);

        if (count >= 0)
        {
            await Clients.All.SendAsync("ViewersUpdated", count);
        }

        await base.OnDisconnectedAsync(exception);
    }
}























//using Microsoft.AspNetCore.SignalR;
//using WallsShop.Properties.Entity;

//namespace WallsShop.Hubs;

//using Microsoft.AspNetCore.SignalR;
//using System.Security.Claims;

//public class OrdersHub : Hub
//{
//    public override async Task OnConnectedAsync()
//    {
//        //var user = Context.User;

//        //if (user?.Identity?.IsAuthenticated == true)
//        //{
//        //    var isAgent = user.IsInRole("Agent");

//        //    if (isAgent)
//        //    {
//        //        await Groups.AddToGroupAsync(Context.ConnectionId, "Agents");

//        //        var agentId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//        //        var agentName = user.FindFirst(ClaimTypes.Name)?.Value;

//        //        Console.WriteLine($"Agent connected: {agentName} (ID: {agentId})");
//        //    }
//        //}

//        //await base.OnConnectedAsync();


//    }

//    public override async Task OnDisconnectedAsync(Exception exception)
//    {
//        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Agents");
//        await base.OnDisconnectedAsync(exception);
//    }

//    public async Task NotifyAgentsNewOrder(Order order)
//    {
//        await Clients.Group("Agents").SendAsync("ReceiveNewOrder",order);
//    }
//}