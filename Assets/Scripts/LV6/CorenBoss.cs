using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class CorenBoss : MonoBehaviour
{
    public Transform playerTransform;
    // Health parameters
    public int maxHealth = 100;
    public int currentHealth;
    public float phase2Threshold = 0.35f; // 35% health to enter phase 2
    private bool hasEnteredPhase2 = false; // Prevent multiple transitions
    
    //Combat parameters
    public float attackDamage = 10f;
    public float meleeAttackCooldown = 3f;
    private float meleeAttackTimer = 0f;
    
    // Components
    public Animator anim;
    public Rigidbody2D rb;
    public CorenStateMachine stateMachine;
    
    // Phase 2 is a separate scene; set its build name here.
    public string phase2SceneName = "LV6Scene2";

    // Resources paths are relative to a folder literally named "Resources/" (no "Assets/" prefix).
    // These are only used as a fallback if the prefab/sprites aren't already assigned on the Phase 2 state.
    private string clockHandPrefabPath = "clocksweep";
    private string auraBurstSpritesheetPath = "Sprites/AuraBurst";
    
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        stateMachine = GetComponent<CorenStateMachine>();
        DontDestroyOnLoad(gameObject);

        // Find player in new scene
        if (playerTransform == null)
        {
            playerTransform = FindObjectOfType<Atreus6PlayerSuperclassa>()?.transform;
        }
    }

    void Update()
    {
        if (stateMachine != null)
            stateMachine.Tick();
    }

    // Phase 1 attacks
    public void TimeBoltBarrage()
    {
        Debug.Log("Coren Boss Phase 1 Attack 1 executed.");
    }

    public void ClockHandSweep()
    {
        Debug.Log("Coren Boss Phase 1 Attack 3 executed.");
    }

    public void TimeProjectiles()
    {
        Debug.Log("Coren Boss Phase 1 Attack 4 executed.");
    }

    // Phase 2 attacks
    public void TemporalCloning()
    {
        Debug.Log("Coren Boss Phase 2 Attack 1 executed.");
    }

    public void TimeFreezeTrap()
    {
        Debug.Log("Coren Boss Phase 2 Attack 2 executed.");
    }

    public void DiveBomb()
    {
        Debug.Log("Coren Boss Phase 2 Attack 4 executed.");
    }

    private void UpdateMeleeAttack()
    {
        meleeAttackTimer -= Time.deltaTime;
        if (meleeAttackTimer <= 0f)
        {
            anim.SetTrigger("MeleeAttack");
            meleeAttackTimer = meleeAttackCooldown;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Coren took " + damage + " damage. Health: " + currentHealth);

        // Check for phase transition
        if (!hasEnteredPhase2 && currentHealth <= maxHealth * phase2Threshold && currentHealth > 0)
        {
            hasEnteredPhase2 = true;
            Debug.Log("Coren entering Phase 2!");
            StartCoroutine(TransitionToPhase2());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator TransitionToPhase2()
    {
        // Load Phase 2 scene by name (was hard-coded index 2 = LV1Scene2, which sent the player back to Level 1).
        SceneManager.LoadScene(phase2SceneName);
        yield return null;

        // Wait one more frame for scene to fully load
        yield return null;

        // Pass the Phase 2 assets. Prefer Inspector-assigned references; only fall back to
        // Resources.Load (which needs the assets under a "Resources/" folder).
        if (stateMachine != null && stateMachine.phase2State != null)
        {
            if (stateMachine.phase2State.clockHandPrefab == null)
            {
                GameObject clockHand = Resources.Load<GameObject>(clockHandPrefabPath);
                if (clockHand != null)
                    stateMachine.phase2State.clockHandPrefab = clockHand;
                else
                    Debug.LogError($"Clock Hand prefab not assigned and not found in Resources/{clockHandPrefabPath}");
            }

            if (stateMachine.phase2State.auraBurstSpritesheet == null || stateMachine.phase2State.auraBurstSpritesheet.Length == 0)
            {
                Sprite[] auraBurst = Resources.LoadAll<Sprite>(auraBurstSpritesheetPath);
                if (auraBurst.Length > 0)
                    stateMachine.phase2State.auraBurstSpritesheet = auraBurst;
                else
                    Debug.LogError($"Aura Burst sprites not assigned and not found in Resources/{auraBurstSpritesheetPath}");
            }

            // Change to Phase 2 state
            stateMachine.ChangeState(stateMachine.phase2State);
        }

        // Reset position
        transform.position = new Vector2(0, 0);
    }

    private void Die()
    {
        Debug.Log("Coren died!");
        anim.SetTrigger("Death"); // Trigger your death animation
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Wait for the death animation to complete
        // Replace 2f with your actual animation length
        yield return new WaitForSeconds(2f);
        
        gameObject.SetActive(false);
    }
}