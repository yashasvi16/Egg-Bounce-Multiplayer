using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Collider2D))]
public class PlayerStateManager : NetworkBehaviour
{
    [Header("Elements")]
    [SerializeField] Collider2D playerCollider;
    [SerializeField] SpriteRenderer[] renderers;

    public void Enable()
    {
        EnableClientRpc();
    }

    [ClientRpc]
    private void EnableClientRpc()
    {
        playerCollider.enabled = true;
        foreach (var renderer in renderers)
        {
            Color color = renderer.color;
            color.a = 1f;
            renderer.color = color;
        }
    }

    public void Disable()
    {
        DisableClientRpc();
    }

    [ClientRpc]
    private void DisableClientRpc()
    {
        playerCollider.enabled = false;
        foreach (var renderer in renderers)
        {
            Color color = renderer.color;
            color.a = 0.2f;
            renderer.color = color;
        }
    }
}
