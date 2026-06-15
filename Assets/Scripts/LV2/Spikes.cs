using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    Rigidbody2D rb;
    BoxCollider2D boxCollider2D;
    public float distance;
    bool isFalling = false;
    public int damage=3;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        Physics2D.queriesStartInColliders = false;
        if (!isFalling)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distance);

            Debug.DrawRay(transform.position, Vector2.down * distance, Color.red);
            if (hit.transform != null)
            {
                if (hit.transform.CompareTag("Player"))
                {
                    rb.gravityScale =4.5f;
                    isFalling = true;
                }

            }
        }

    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerSuperclass>().TakeDamage(damage);
            Destroy(gameObject);
        }
        else
        {
            // Landed on something other than the player: stop falling but stay armed-down
            // (do NOT reset isFalling, or the spike would re-trigger and fall again).
            rb.gravityScale=0;
        }
    }
}