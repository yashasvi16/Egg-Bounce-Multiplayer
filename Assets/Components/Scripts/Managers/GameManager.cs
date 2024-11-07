using UnityEngine;
using Unity.Netcode;
using System;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public enum State { Menu, Game, Win, Lose }
    private State gameState;

    private int connectedPlayers = 0;

    [Header("Events")]
    public static Action<State> onGameStateChanged;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
    }


    private void NetworkManager_OnServerStarted()
    {
        Debug.Log("Gm checking");
        if (!IsServer)
            return;

        Debug.Log("Gm server");

        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        connectedPlayers++;

        if(connectedPlayers >= 2)
        {
            StartGame();
        }
    }

    void Start()
    {
        gameState = State.Menu;
    }

    public void SetGameState(State _gameState)
    {
        gameState = _gameState;
        onGameStateChanged?.Invoke(gameState);
    }

    private void StartGame()
    {
        StartGameClientRpc();
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        gameState = State.Game;
        onGameStateChanged?.Invoke(gameState);
    }
}
