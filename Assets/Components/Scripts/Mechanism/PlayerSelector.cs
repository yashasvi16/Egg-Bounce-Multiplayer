using UnityEngine;
using Unity.Netcode;

public class PlayerSelector : NetworkBehaviour
{
    public static PlayerSelector Instance;

    private bool isHostTurn = false;

    private void Awake()
    {
        if (Instance == null)
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

    private void NetworkManager_OnServerStarted()
    {
        if(!IsServer)
            return;

        GameManager.onGameStateChanged += GameStateChangedCallback;
        Egg.onHit += SwitchPlayers;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
        GameManager.onGameStateChanged -= GameStateChangedCallback;
        Egg.onHit -= SwitchPlayers;
    }

    private void GameStateChangedCallback(GameManager.State gameState)
    {
        switch (gameState)
        {
            case GameManager.State.Game:
                Initialize();
                break;
        }
    }

    private void Initialize()
    {
        PlayerStateManager[] playerStateManagers = FindObjectsByType<PlayerStateManager>(FindObjectsSortMode.None);

        for (int i = 0; i < playerStateManagers.Length; i++)
        {
            if (playerStateManagers[i].GetComponent<NetworkObject>().IsOwnedByServer)
            {
                if(isHostTurn)
                {
                    playerStateManagers[i].Enable();
                }
                else
                {
                    playerStateManagers[i].Disable();
                }
            }
            else
            {
                if(isHostTurn)
                {
                    playerStateManagers[i].Disable();
                }
                else
                {
                    playerStateManagers[i].Enable();
                }
            }
        }
    }

    private void SwitchPlayers()
    {
        isHostTurn = !isHostTurn;
        Initialize();
    }

    public bool IsHostTurn()
    {
        return isHostTurn;
    }
}
