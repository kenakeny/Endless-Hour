using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointtLvl4 : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            if (levelManager != null)
                levelManager.SetCheckpoint(transform);
        }
    }
}
