using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changecam : MonoBehaviour
{

    public CameraFollow camfollow;

    public void OnTriggerEnter2D(Collider2D other)
{

    if (other.CompareTag("Player")){

camfollow.minY=35;

    }


}}
