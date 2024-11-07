using UnityEngine;
using Unity.Netcode;
using TMPro;

public class ScoreManager : NetworkBehaviour
{
    [Header("Elements")]
    [SerializeField] TMP_Text scoreTxt;
    private int hostScore;
    private int clientScore;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        NetworkManager.OnServerStarted += NetworkManager_OnServerStarted;
    }

    private void NetworkManager_OnServerStarted()
    {
        if(!IsServer)
            return;

        Egg.onFellInWater += EggFellInWaterCallback;
        GameManager.onGameStateChanged += GameStateChangedCallback;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        NetworkManager.OnServerStarted -= NetworkManager_OnServerStarted;
        Egg.onFellInWater -= EggFellInWaterCallback;
        GameManager.onGameStateChanged -= GameStateChangedCallback;
    }

    private void GameStateChangedCallback(GameManager.State gameState)
    {
        switch(gameState)
        {
            case GameManager.State.Game:
                ResetScores();
                break;
        }
    }

    private void EggFellInWaterCallback()
    {
        if(PlayerSelector.Instance.IsHostTurn())
        {
            clientScore++;
        }
        else
        {
            hostScore++;
        }

        UpdateScoreClientRpc(hostScore, clientScore);
        UpdateScoreText();

        CheckForEndGame();
    }

    [ClientRpc]
    private void UpdateScoreClientRpc(int _hostScore, int _clientScore)
    {
        hostScore = _hostScore;
        clientScore = _clientScore;
    }

    private void UpdateScoreText()
    {
        UpdateScoreTextClientRpc();
    }

    [ClientRpc]
    private void UpdateScoreTextClientRpc()
    {
        scoreTxt.text = "<color=#0055ffff>" + hostScore + "</color> - <color=#ff0055ff>" + clientScore + "</color>";
    }

    private void ResetScores()
    {
        hostScore = 0;
        clientScore = 0;

        UpdateScoreClientRpc(hostScore, clientScore);
        UpdateScoreText();
    }

    private void CheckForEndGame()
    {
        if(hostScore >= 5)
        {
            HostWin();
        }
        else if(clientScore >= 5)
        {
            ClientWin();
        }
        else
        {
            ReuseEgg();
        }
    }

    private void HostWin()
    {
        HostWinClientRpc();
    }

    [ClientRpc]
    private void HostWinClientRpc()
    {
        if(IsServer)
        {
            GameManager.Instance.SetGameState(GameManager.State.Win);
        }
        else
        {
            GameManager.Instance.SetGameState(GameManager.State.Lose);
        }
    }

    private void ClientWin()
    {
        ClientWinClientRpc();
    }

    [ClientRpc]
    private void ClientWinClientRpc()
    {
        if (IsServer)
        {
            GameManager.Instance.SetGameState(GameManager.State.Lose);
        }
        else
        {
            GameManager.Instance.SetGameState(GameManager.State.Win);
        }
    }

    private void ReuseEgg()
    {
        EggManager.Instance.ReuseEgg();
    }
}
