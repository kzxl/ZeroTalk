using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using ChatBox.Server.Models;
using ChatBox.Shared.Protocol;

namespace ChatBox.Server.Services
{
    /// <summary>
    /// Routes packets to specific clients or broadcasts across the network.
    /// </summary>
    public class MessageRouter : IMessageRouter
    {
        private readonly ConcurrentDictionary<string, ConnectedClient> _clients;
        private readonly Action<string> _log;

        public MessageRouter(ConcurrentDictionary<string, ConnectedClient> clients, Action<string> log)
        {
            _clients = clients;
            _log = log;
        }

        /// <summary>
        /// Sends packet to a specific client
        /// </summary>
        public void SendToClient(string userId, Packet packet)
        {
            ConnectedClient client;
            if (_clients.TryGetValue(userId, out client) && client.IsAuthenticated)
            {
                try
                {
                    PacketSerializer.SendPacket(client.Stream, packet);
                }
                catch (Exception ex)
                {
                    _log?.Invoke($"[ERROR] Failed to send packet to {userId}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Broadcasts packet to all connected clients (except excluded user)
        /// </summary>
        public void Broadcast(Packet packet, string excludeUserId = null)
        {
            foreach (var kvp in _clients)
            {
                if (kvp.Key == excludeUserId) continue;
                if (!kvp.Value.IsAuthenticated) continue;

                try
                {
                    PacketSerializer.SendPacket(kvp.Value.Stream, packet);
                }
                catch (Exception ex)
                {
                    _log?.Invoke($"[ERROR] Broadcast to {kvp.Key} failed: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Broadcasts online user list to all connected clients.
        /// Data = "user1|DisplayName1,user2|DisplayName2,..."
        /// </summary>
        public void BroadcastUserList()
        {
            var onlineUsers = _clients.Values
                .Where(c => c.IsAuthenticated)
                .Select(c => c.UserId + "|" + c.DisplayName);

            var userListData = string.Join(",", onlineUsers);
            var packet = new Packet(PacketType.UserList, "server", null, userListData);

            Broadcast(packet);
        }
    }
}
