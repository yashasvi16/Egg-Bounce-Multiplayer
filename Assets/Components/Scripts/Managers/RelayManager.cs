using System;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using TMPro;

public class RelayManager : MonoBehaviour
{
    public static RelayManager Instance;
    const int m_MaxConnections = 1; // max of 2 players

    [Header("Elements")]
    [SerializeField] TMP_Text joinCodeText;
    [SerializeField] TMP_InputField joinCodeIF;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // Corrected to destroy the duplicate GameObject
    }

    public async void StartHostRelay()
    {
        string joinCode = await StartHostWithRelay(m_MaxConnections);
        joinCodeText.text = joinCode;
    }

    public async void StartClientRelay()
    {
        bool result = await StartClientWithRelay(joinCodeIF.text);
        Debug.Log("Client started: " + result);
    }

    /// <summary>
    /// Starts a game host with a relay allocation: initializes the Unity services, signs in anonymously, and starts the host with a new relay allocation.
    /// </summary>
    /// <param name="maxConnections">Maximum number of connections to the created relay.</param>
    /// <returns>The join code that a client can use.</returns>
    public async Task<string> StartHostWithRelay(int maxConnections = 1)
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            if (NetworkManager.Singleton.StartHost())
            {
                Debug.Log("Host started successfully with join code: " + joinCode);
                return joinCode;
            }
            else
            {
                Debug.LogWarning("Failed to start host.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Relay initialization failed: " + ex);
            return null;
        }
    }

    /// <summary>
    /// Joins a game with relay: initializes the Unity services, signs in anonymously, joins the relay with the given join code, and starts the client.
    /// </summary>
    /// <param name="joinCode">The join code of the allocation.</param>
    /// <returns>True if starting the client was successful</returns>
    public async Task<bool> StartClientWithRelay(string joinCode)
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            if (NetworkManager.Singleton.StartClient())
            {
                Debug.Log("Client connected successfully with join code: " + joinCode);
                return true;
            }
            else
            {
                Debug.LogWarning("Failed to start client.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to join relay: " + ex);
            return false;
        }
    }
}
