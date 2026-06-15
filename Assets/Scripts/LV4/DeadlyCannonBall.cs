using UnityEngine;

public class DeadlyCannonBall : MonoBehaviour
{
    private LevelManager levelManager;

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    // This script must be on an object with a Collider2D set to Is Trigger = true,
    // and the object entering must have a Rigidbody2D.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (levelManager == null)
            levelManager = FindObjectOfType<LevelManager>();

        if (levelManager != null)
            levelManager.RespawnPlayer();
    }
}
