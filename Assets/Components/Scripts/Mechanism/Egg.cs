using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Egg : MonoBehaviour
{
    [Header("Physics Settings")]
    [SerializeField] float bounceVelocity;
    Rigidbody2D rb;
    private bool isAlive;
    private float gravityScale;

    [Header("Events")]
    public static Action onHit;
    public static Action onFellInWater;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isAlive = true;

        gravityScale = rb.gravityScale;
        rb.gravityScale = 0;

        StartCoroutine(WaitAndFall());
    }

    IEnumerator WaitAndFall()
    {
        yield return new WaitForSeconds(2);

        rb.gravityScale = gravityScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isAlive)
        {
            return;
        }

        if (collision.collider.TryGetComponent(out PlayerController playerController))
        {
            Bounce(collision.GetContact(0).normal);
            onHit?.Invoke();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isAlive)
        {
            return;
        }

        if(collision.CompareTag("Water"))
        {
            isAlive = false;
            onFellInWater?.Invoke();
        }
    }

    private void Bounce(Vector2 normal)
    {
        rb.linearVelocity = normal * bounceVelocity;
    }

    public void ReuseEgg()
    {
        transform.position = Vector2.up * 5;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0;

        isAlive = true;

        StartCoroutine(WaitAndFall());
    }
}
