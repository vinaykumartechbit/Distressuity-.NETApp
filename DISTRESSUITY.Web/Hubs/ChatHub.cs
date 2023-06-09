using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using DISTRESSUITY.Service.Model;
using System.Threading.Tasks;
using DISTRESSUITY.Service.Services;

namespace DISTRESSUITY.Web.Hubs
{
    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections =
            new Dictionary<T, HashSet<string>>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }

    //[Authorize]
    public class ChatHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        //string UserEmail =(new DISTRESSUITY.Service.Service.UserService()).getLoggedInUserEmail();
        string UserEmail ="";
        public override Task OnConnected()
        {
            UserEmail = Context.QueryString["userEmail"];
            _connections.Add(UserEmail, Context.ConnectionId);
            return base.OnConnected();

        }

        public override Task OnDisconnected(bool stopCalled)
        {
            _connections.Remove(Context.QueryString["userEmail"], Context.ConnectionId);
            return base.OnDisconnected(true);
        }

        public override Task OnReconnected()
        {
            if (!_connections.GetConnections(Context.QueryString["userEmail"]).Contains(Context.ConnectionId))
            {
                _connections.Add(Context.QueryString["userEmail"], Context.ConnectionId);
            }
            return base.OnReconnected();
        }
        
        public void Send(string name, string message)
        {
            Clients.All.broadcastMessage(name, message);
        }
        public void Send(string name1, string who, ConversationReplyModel reply)
        {
            var connectionId = _connections.GetConnections(who.ToLower());
            Clients.All.broadcastMessage(connectionId, reply);
            //foreach (var connectionId in _connections.GetConnections(who.ToLower()))
            //{
            //    Clients.Client(connectionId).broadcastMessage("", reply);
            //}
        }
        public void RunMe(string logginUserEmail)
        {
            System.Diagnostics.Debug.WriteLine("Client Started");
        }
        public void Connected()
        {
            OnConnected();
        }
        public void Disconnected()
        {
            OnDisconnected(true);
        }
        public static void Notify(string name, string message)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            hubContext.Clients.All.broadcastMessage(name, message);
        }
    }
}