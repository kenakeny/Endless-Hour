using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class trigspawnShield : MonoBehaviour
{

    public GameObject ShieldSolider;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")){

            Instantiate(ShieldSolider);
        }
    }
}
