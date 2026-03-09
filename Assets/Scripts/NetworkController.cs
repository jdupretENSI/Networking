using Unity.Netcode;
using UnityEngine;

public class NetworkController : NetworkBehaviour
{
    private ChatManager _chatManager;
    private void Start()
    {
        NetworkManager.Singleton.StartHost();
        
        _chatManager = FindAnyObjectByType<ChatManager>();
    }

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
    [ServerRpc]
    private void SendChatMessageServerRpc(ChatBoxMessage message, ServerRpcParams rpcParams = default)
    {
        // Get the REAL sender ID from network system
        ulong senderId = rpcParams.Receive.SenderClientId;
        
        // Broadcast to ALL clients (including sender)
        Debug.Log(message);
        BroadcastChatMessageClientRpc(message, senderId);
    }

    [ClientRpc] 
    private void BroadcastChatMessageClientRpc(ChatBoxMessage message, ulong senderId)
    {
        // Make sure ChatManager exists
        if (_chatManager != null)
        {
            Debug.Log(message);
            _chatManager.ReceiveMessage(message, senderId);
        }
        else
        {
            Debug.LogError("ChatManager not found!");
        }
    }
    
    // Helper to check if we're connected
    public bool IsConnected()
    {
        return NetworkManager.Singleton.IsConnectedClient;
    }
}
