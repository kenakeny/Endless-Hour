using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // if the collider of the object whose name is Player GameObject touches the checkpoint circle collider
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            if (levelManager != null)
                levelManager.SetCheckpoint(transform);
            //go to the level manager script and update the value of currentcheckpoint to become the new checkpoint the player has just pass through. this is necessary when you have several checkpoints in a level.
        }
    }
}
