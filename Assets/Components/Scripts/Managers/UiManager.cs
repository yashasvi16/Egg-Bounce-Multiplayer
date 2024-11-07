using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject connectionPanel;
    [SerializeField] GameObject waitingPanel;
    [SerializeField] GameObject gamePanel;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject losePanel;

    void Start()
    {
        ShowConnectionPanel();
        GameManager.onGameStateChanged += GameStateChangedCallback;
    }

    private void OnDestroy()
    {
        GameManager.onGameStateChanged -= GameStateChangedCallback;
    }

    private void GameStateChangedCallback(GameManager.State gameState)
    {
        switch (gameState)
        {
            case GameManager.State.Game:
                ShowGamePanel(); 
                break;
            case GameManager.State.Win:
                ShowWinPanel(); 
                break;
            case GameManager.State.Lose:
                ShowLosePanel();
                break;
        }
    }
    
    private void ShowConnectionPanel()
    {
        connectionPanel.SetActive(true);
        waitingPanel.SetActive(false);
        gamePanel.SetActive(false);

        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    private void ShowWaitingPanel()
    {
        connectionPanel.SetActive(false);
        waitingPanel.SetActive(true);
        gamePanel.SetActive(false);
    }

    private void ShowGamePanel()
    {
        connectionPanel.SetActive(false);
        waitingPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    private void ShowWinPanel()
    {
        winPanel.SetActive(true);
    }

    private void ShowLosePanel()
    {
        losePanel.SetActive(true);
    }

    public void HostButtonCallback()
    {
        //NetworkManager.Singleton.StartHost();
        ShowWaitingPanel();
        RelayManager.Instance.StartHostRelay();
    }

    public void ClientButtonCallback()
    {
        /*string ipAddress = IpManager.Instance.GetInputIp();

        UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
        utp.SetConnectionData(ipAddress, 7777);

        NetworkManager.Singleton.StartClient();*/

        RelayManager.Instance.StartClientRelay();
        ShowWaitingPanel();
    }

    public void NextButtonCallback()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        NetworkManager.Singleton.Shutdown();
    }

    public void PlayButtonCallback()
    {
        ShowWaitingPanel();

        MatchmakingManager.Instance.PlayButtonCallback();
    }
}
