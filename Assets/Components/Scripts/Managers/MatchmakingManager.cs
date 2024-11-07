using System;
using System.Net.Security;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Core;
using UnityEngine;
using System.Collections.Generic;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Collections;

public class MatchmakingManager : MonoBehaviour
{
    public static MatchmakingManager Instance;
    Lobby lobby;

    [Header("Settings")]
    [SerializeField] string _joinCode;

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(Instance);
    }

    public async void PlayButtonCallback()
    {
        await Authenticate();

        lobby = await QuickJoinLobby() ?? await CreateLobby();
    }

    async Task<Lobby> QuickJoinLobby()
    {
        try
        {
            Lobby _lobby = await Lobbies.Instance.QuickJoinLobbyAsync();

            JoinAllocation allocation = 
                await RelayService.Instance.JoinAllocationAsync(_lobby.Data[_joinCode].Value);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData
                (
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.ConnectionData,
                    allocation.HostConnectionData
                );

            NetworkManager.Singleton.StartClient();
            return _lobby;
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }

    async Task<Lobby> CreateLobby()
    {
        try
        {
            int maxPlayers = 2;
            string lobbyName = "CoolLobby";

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            CreateLobbyOptions options = new CreateLobbyOptions();
            options.Data = new Dictionary<string, DataObject> { { _joinCode, new DataObject(DataObject.VisibilityOptions.Public, joinCode) } };

            Lobby _lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            StartCoroutine(HeartbeatLobbyCoroutine(lobby.Id, 15));

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData
                (
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.ConnectionData
                );

            NetworkManager.Singleton.StartHost();

            return _lobby;
        }
        catch(Exception e)
        {
            Debug.Log(e);
            return null;
        }
    }

    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        var delay = new WaitForSeconds(waitTimeSeconds);

        while(true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }


    async Task Authenticate()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            var playerID = AuthenticationService.Instance.PlayerId;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
