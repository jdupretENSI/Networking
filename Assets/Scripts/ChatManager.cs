using System;
using Unity.Netcode;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    private ulong _localClientId;
    
    [SerializeField] private ChatBoxManager _chatBoxManager;
    [SerializeField]private NetworkController _networkController;

    private void Start()
    {
        _chatBoxManager = FindFirstObjectByType<ChatBoxManager>();
        ChatBoxManager.Instance.OnMessageSend += MessageSent;
        GetLocalClientId();
    }

    private void MessageSent(object sender, ChatBoxMessage message)
    {
        _networkController.SendMessage(message);
    }
    
    private void OnDisable()
    {
        // Clean up subscription
        if (ChatBoxManager.Instance != null)
        {
            ChatBoxManager.Instance.OnMessageSend -= MessageSent;
        }
    }

    public void ReceiveMessage(ChatBoxMessage message, ulong senderId)
    {
        // Store our local ID for comparison
        if (NetworkManager.Singleton != null)
        {
            _localClientId = NetworkManager.Singleton.LocalClientId;
        }
        
        // Display message in UI
        if (_chatBoxManager != null)
        {
            _chatBoxManager.DisplayMessage(message);
            
            // Optional: Log for debugging
            string senderType = senderId == _localClientId ? "Me" : $"Player {senderId}";
            _chatBoxManager.DisplayMessage(message);
        }
    }
    // Get client ID properly from NetworkManager
    private void GetLocalClientId()
    {
        if (NetworkManager.Singleton != null)
        {
            _chatBoxManager.SetCurrentUser(NetworkManager.Singleton.LocalClientId.ToString());
        }
    }
}
