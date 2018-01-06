using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace MyAspNetMvcApp
{
    public class ChatHub : Hub
    {
        public void SendToAll(string name, string message)
        {
            Clients.All.InvokeAsync("sendToAll", name, message);
        }
    }

    //public class SignalRHub : Hub
    //{
    //    public static void NotifyNewBookAdded(int Id)
    //    {
    //        IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SignalRHub>();

    //        // the update client method will update the connected client about any recent changes in the server data
    //        context.Clients.All.BooksAddedNotif(Id);
    //    }

    //    public static void NotifyNewBookUpdated(int Id)
    //    {
    //        IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SignalRHub>();
    //        context.Clients.All.BooksUpdatedNotif(Id);
    //    }

    //    public static void NotifyNewBookDeleted(int Id)
    //    {
    //        IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SignalRHub>();
    //        context.Clients.All.BooksDeletedNotif(Id);
    //    }

    //    public void SendToAll(string name, string message)
    //    {
    //        Clients.All.InvokeAsync("sendToAll", name, message);
    //    }
    //}
}

// Angular cli component inside an existing .net MVC application.
// https://github.com/angular/angular-cli/issues/8271