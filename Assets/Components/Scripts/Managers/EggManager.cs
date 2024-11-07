using UnityEngine;
using Unity.Netcode;

public class EggManager : NetworkBehaviour
{
    public static EggManager Instance;

    [Header("Elements")]
    [SerializeField] Egg eggPrefab;

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

    private void Start()
    {
        GameManager.onGameStateChanged += GameStateChangedCallback;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        GameManager.onGameStateChanged -= GameStateChangedCallback;
    }

    private void GameStateChangedCallback(GameManager.State gameState)
    {
        switch (gameState)
        {
            case GameManager.State.Game:
                SpawnEgg();
                break;
        }
    }

    private void SpawnEgg()
    {
        Debug.Log("Em checking");
        if (!IsServer)
            return;

        Debug.Log("Em server");
        Egg eggInstance = Instantiate(eggPrefab, Vector2.up * 5f, Quaternion.identity);
        eggInstance.GetComponent<NetworkObject>().Spawn();
        eggInstance.transform.SetParent(transform);
    }

    public void ReuseEgg()
    {
        if(!IsServer)
            return;

        if (transform.childCount <= 0)
            return;

        transform.GetChild(0).GetComponent<Egg>().ReuseEgg();
    }
}
