using UnityEngine;
using Unity.Netcode;

public class PlayerColorizer : NetworkBehaviour
{
    [Header("Elements")]
    [SerializeField] SpriteRenderer[] sprites;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if(!IsServer && IsOwner)
        {
            ColorizeServerRpc(Color.red);
        }
    }

    [Rpc(SendTo.Server)]
    private void ColorizeServerRpc(Color color)
    {
        ColorizeClientRpc(color);
    }

    [ClientRpc]
    private void ColorizeClientRpc(Color color)
    {
        foreach (var sprite in sprites)
        {
            sprite.color = color;
        }
    }
}
