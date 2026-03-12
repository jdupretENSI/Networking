using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    private ulong _localClientId;
    
    [SerializeField] private ChatBoxManager _chatBoxManager;
    [SerializeField]private NetworkController _networkController;

    private void Start()
    {
        if (!_chatBoxManager) _chatBoxManager = FindFirstObjectByType<ChatBoxManager>();
        
        ChatBoxManager.Instance.OnMessageSend += MessageSent;

        NetworkManager.Singleton.OnConnectionEvent += GetConnectedPeers;
    }

    private void OnDestroy()
    {
        ChatBoxManager.Instance.OnMessageSend -= MessageSent;

        NetworkManager.Singleton.OnConnectionEvent -= GetConnectedPeers;
    }

    private void MessageSent(object sender, ChatBoxMessage message)
    {
        _networkController.SendMessage(message);
    }

    public void ReceiveMessage(ChatBoxMessage message, ulong senderId)
    {
        // Store our local ID for comparison
        if (NetworkManager.Singleton != null)
        {
            _localClientId = NetworkManager.Singleton.LocalClientId;
        }
        
        // Display message in UI
        if (_chatBoxManager == null) return;
        
        string senderType = senderId == _localClientId ? "Me" : $"Player {senderId}";
        _chatBoxManager.DisplayMessage(message);
    }

    /// <summary>
    /// Not ideal, but every time a new client connects or disconnects
    /// we check all the clients connected and reset the list
    /// Ideally we would just add a new client each time a new one connects and remove one if they disconnect.
    /// </summary>
    /// <param name="newPeerId"></param>
    private void GetConnectedPeers(NetworkManager nm, ConnectionEventData ced)
    {
        if (!NetworkManager.Singleton) return;

        ulong localClientId = NetworkManager.Singleton.LocalClientId;
        
        IEnumerable<ulong> allClientIds = NetworkManager.Singleton.ConnectedClients
            .Where(client 
                => client.Key != localClientId)
            .Select(client 
                => client.Key);

        stringContainer[] clients = allClientIds
            .Select(id 
                => new stringContainer(_networkController.ClientUsernames
                    .TryGetValue(id, out string username) 
                    ? username : id.ToString()))
            .ToArray();
        
        _chatBoxManager.SetupDropdownTargets(clients);
    }

    public void SetupHostServer() 
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = _chatBoxManager.IP;
        transport.ConnectionData.Port = Convert.ToUInt16(_chatBoxManager.Port);
        
        NetworkManager.Singleton.StartHost();
        _networkController.ClientUsernames.Add(NetworkManager.Singleton.LocalClientId, _chatBoxManager.CurrentPlayerName);
    }

    public void ClientConnectToHost()
    {
        // Set the IP address you want to connect to
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = _chatBoxManager.IP;
        transport.ConnectionData.Port = Convert.ToUInt16(_chatBoxManager.Port);
        
        NetworkManager.Singleton.StartClient();
        _networkController.SendPlayerJoinedServerRpc(_chatBoxManager.CurrentPlayerName);
    }

    public void EndChat()
    {
        NetworkManager.Singleton.Shutdown();
    }
}
