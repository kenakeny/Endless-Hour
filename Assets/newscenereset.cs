using UnityEngine;

public class newscenereset : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1f; // Unfreeze time
        Debug.Log("Scene loaded, time unfrozen");
    }
}
