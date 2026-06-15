using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : MonoBehaviour
{
    public Transform player;
    public PlayerSuperclass playerScript;
    
    // Start is called before the first frame update
    void Start()
    {
        playerScript=player.GetComponent<PlayerSuperclass>();
    }

void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")){
            if (playerScript != null)
                playerScript.TakeDamage(5);
        }
        else if (other.CompareTag("ground"))
        {
            Destroy(gameObject);
        }
    }
}