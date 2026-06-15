using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class floorSpikes : MonoBehaviour
{
    public int AttackDamage=14;
private void OnCollisionEnter2D(Collision2D other){

    if (other.gameObject.CompareTag("Player")){
        other.gameObject.GetComponent<PlayerSuperclass>().TakeDamage(AttackDamage);

    }

}

}
