using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandEnemy : MonoBehaviour
{
    public Transform[] points;
    public float speed;
    
    private Rigidbody2D rb;
    private Animator anim;
    private Transform currentPoint;
    private int currentPointIndex;
    private SpriteRenderer sr;
    public int AttackDamage=3;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr= GetComponent<SpriteRenderer>();
        currentPoint = points[0];
    }

    void Update()
    {
        Vector2 toPoint = (Vector2)currentPoint.position - (Vector2)transform.position;
        Vector2 dir = toPoint.normalized;

        // Normalize the direction first, then scale by speed so horizontal speed
        // is constant regardless of current vertical velocity.
        rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f)
        {
            currentPointIndex++;
            
            if (currentPointIndex >= points.Length)
            {
                currentPointIndex = 0;
            }
            
            currentPoint = points[currentPointIndex];
        }

        Flip();
    }

private void Flip()
{
    if (currentPoint.position.x < transform.position.x)
    {
        sr.flipX = true;
    }
    else if (currentPoint.position.x > transform.position.x) 
    {
        sr.flipX = false;
    }
}    
private void OnCollisionEnter2D(Collision2D other){

    if (other.gameObject.CompareTag("Player")){
        other.gameObject.GetComponent<PlayerSuperclass>().TakeDamage(AttackDamage);

    }

}
}