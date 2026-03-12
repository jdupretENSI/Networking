using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkController : NetworkBehaviour
{
    private ChatManager _chatManager;
    
    private Dictionary<ulong, string> _clientUsernames = new();
    public Dictionary<ulong, string>  ClientUsernames => _clientUsernames;
    private void Start()
    {
        _chatManager = FindAnyObjectByType<ChatManager>();
    }

    #region Messaging
    public void SendMessage(ChatBoxMessage message)
    {
        // Don't pass ID - get it from NetworkManager
        if (IsHost)
        {
            // Host: broadcast to everyone including self
            BroadcastChatMessageClientRpc(message, NetworkManager.Singleton.LocalClientId);
        }
        else if (IsClient)
        {
            // Client: send to host
            SendChatMessageServerRpc(message);
        }
    }
    
    // On the peer/client sending the message
    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(ChatBoxMessage message, ServerRpcParams rpcParams = default)
    {
        // Get the REAL sender ID from network system
        ulong senderId = rpcParams.Receive.SenderClientId;
        
        // Broadcast to ALL clients (including sender)
        BroadcastChatMessageClientRpc(message, senderId);
    }

    [ClientRpc] 
    private void BroadcastChatMessageClientRpc(ChatBoxMessage message, ulong senderId)
    {
        if (!_chatManager) return;
        
        // Replace senderId with username if we have it
        if (_clientUsernames.TryGetValue(senderId, out string username))
        {
            message.PlayerFrom = username;
        }
        _chatManager.ReceiveMessage(message, senderId);
    }
    #endregion

    #region Utils

    // Helper to check if we're connected
    public bool IsConnected()
    {
        return NetworkManager.Singleton.IsConnectedClient;
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void SendPlayerJoinedServerRpc(string playerName, ServerRpcParams rpcParams = default)
    {
        ulong senderId = rpcParams.Receive.SenderClientId;
    
        _clientUsernames.Add(senderId, playerName);
    }

    #endregion
    
    
    
}
