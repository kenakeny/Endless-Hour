using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class redSkull : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other){
        if (other.CompareTag("Skull")) { Destroy(gameObject); }
    }
}
