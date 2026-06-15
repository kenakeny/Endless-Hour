using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base player controller shared by every level's player.
// Level-specific players (Atreus5, Atreus6PlayerSuperclassa, lvl3playercontroller, ...)
// inherit from this and override only the pieces that differ.
//
// Update() is split into small protected virtual helpers so subclasses can replace one
// behaviour (e.g. how the sprite flips, or which attack trigger fires) without
// re-implementing the whole movement/immunity/boost loop.
public class PlayerSuperclass : MonoBehaviour
{
    // Movement Variables incl. jump and its relation w/ ground and speed
    public float currentSpeed;
    public float moveSpeed;
    public float jumpHeight;
    public KeyCode Spacebar = KeyCode.Space;
    public KeyCode L = KeyCode.A;
    public KeyCode R = KeyCode.D;
    public KeyCode RunKey = KeyCode.LeftShift;
    public Transform groundCheck; // empty child positioned at the player's feet. Used to detect if the player is touching ground.
    public float groundCheckRadius;
    public LayerMask whatIsGround; // which physics layers count as ground.
    public bool grounded;

    // Components
    public Animator anim;
    public SpriteRenderer sr;
    public Rigidbody2D rb;

    // Speed Boost
    public float speedBoostTime = 0f;
    public float originalMoveSpeed = 5f;
    public float speedBoostDuration = 10f;
    public bool speedBoosted = false;

    // Health Variables
    public float health = 100;
    public float maxHealth = 100;
    public Healthbar healthBarUI;
    public int normalAttackDamage = 5;
    public int AttackDamage = 5;
    public float BoostDuration = 30f;
    public float BoostTime = 0f;
    public bool isBoosted = false;

    // Immunity
    public bool isImmune = false;
    public float flickerTime = 0f;
    public float flickerDuration = 0.1f;
    public float immunityTime = 0f;
    public float immunityDuration = 1.5f;

    // Attack
    public float attackDuration = 0.3f; // How long the attack lasts
    public bool isAttacking = false;

    // Shield / block
    public bool isBlocking = false;

    // Attack hitbox + VFX (used by sword-style players such as Atreus5 / Atreus6)
    public GameObject slashVFX;
    protected attackhitbox6 attackHitbox;

    // Set true by subclasses (e.g. LV6) that must survive a scene load mid-fight.
    public bool keepBetweenScenes = false;

    public virtual void Start()
    {
        moveSpeed = originalMoveSpeed;
        currentSpeed = moveSpeed;
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        attackHitbox = GetComponentInChildren<attackhitbox6>();

        if (keepBetweenScenes)
            DontDestroyOnLoad(gameObject);
    }

    public virtual void Update()
    {
        HandleMovement();
        HandleImmunity();
        HandleBoosts();
        HandleAttackInput();
        UpdateAnimator();
    }

    public virtual void FixedUpdate()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    // ---- Update helpers (override individually in subclasses) ----

    protected virtual void HandleMovement()
    {
        if (Input.GetKey(RunKey))
            currentSpeed = 2 * moveSpeed;
        else
            currentSpeed = moveSpeed;

        if (Input.GetKeyDown(Spacebar) && grounded)
            Jump();

        if (!Input.GetKey(L) && !Input.GetKey(R))
            rb.velocity = new Vector2(0, rb.velocity.y);

        if (Input.GetKey(L))
        {
            rb.velocity = new Vector2(-currentSpeed, rb.velocity.y);
            if (sr != null) sr.flipX = true;
        }

        if (Input.GetKey(R))
        {
            rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
            if (sr != null) sr.flipX = false;
        }
    }

    protected virtual void HandleImmunity()
    {
        if (!isImmune) return;

        SpriteFlicker();
        immunityTime += Time.deltaTime;
        if (immunityTime >= immunityDuration)
        {
            isImmune = false;
            if (sr != null) sr.enabled = true;
        }
    }

    protected virtual void HandleBoosts()
    {
        if (isBoosted)
        {
            BoostTime += Time.deltaTime;
            if (BoostTime >= BoostDuration)
            {
                isBoosted = false;
                AttackDamage = normalAttackDamage;
            }
        }

        if (speedBoosted)
        {
            speedBoostTime += Time.deltaTime;
            if (speedBoostTime >= speedBoostDuration)
            {
                speedBoosted = false;
                moveSpeed = originalMoveSpeed;
            }
        }
    }

    protected virtual void HandleAttackInput()
    {
        if (Input.GetMouseButton(0))
            Attack();

        if (Input.GetMouseButton(1))
        {
            isBlocking = true;
            anim.SetBool("ShieldUp", true);
        }
        else
        {
            isBlocking = false;
            anim.SetBool("ShieldUp", false);
        }
    }

    protected virtual void UpdateAnimator()
    {
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("Height", rb.velocity.y);
        anim.SetBool("Grounded", grounded);
        anim.SetBool("IsRunning", Input.GetKey(RunKey));
    }

    // ---- Attack ----

    public virtual void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            anim.SetTrigger("Attack");
            StartCoroutine(ResetAttack());
        }
    }

    public virtual IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(attackDuration);
        if (attackHitbox != null) attackHitbox.DisableDamage();
        isAttacking = false;
    }

    // Called from an animation event at the moment the swing connects.
    public virtual void OnAttackConnect()
    {
        if (attackHitbox != null) attackHitbox.EnableDamage();
    }

    public void PlaySlashVFX()
    {
        if (slashVFX != null)
        {
            slashVFX.SetActive(true);
            StartCoroutine(DisableVFXAfterDelay(0.3f));
        }
    }

    protected IEnumerator DisableVFXAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (slashVFX != null) slashVFX.SetActive(false);
    }

    public virtual void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
    }

    public virtual void SpriteFlicker()
    {
        if (flickerTime < flickerDuration)
            flickerTime += Time.deltaTime;
        else
        {
            if (sr != null) sr.enabled = !sr.enabled;
            flickerTime = 0f;
        }
    }

    // ---- Health / boosts ----

    // Send a DMG value to subtract from health; respawns via LevelManager on death.
    public virtual void TakeDamage(int damage)
    {
        if (isImmune) return;

        health -= damage;
        health = Mathf.Clamp(health, 0f, maxHealth);
        if (healthBarUI != null) healthBarUI.updateHealthBar();

        if (health <= 0)
        {
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            if (levelManager != null) levelManager.RespawnPlayer();
            health = maxHealth;
            if (healthBarUI != null) healthBarUI.updateHealthBar();
        }

        isImmune = true;
        immunityTime = 0f;
    }

    public virtual void Heal(int healAmount)
    {
        health += healAmount;
        if (health > maxHealth) health = maxHealth;
    }

    public virtual void AttackBoost(int boostAmount)
    {
        if (!isBoosted)
        {
            AttackDamage += boostAmount;
            isBoosted = true;
        }
        BoostTime = 0f;
    }

    public virtual void speedBoost(int boostAmount)
    {
        if (!speedBoosted)
        {
            moveSpeed += boostAmount;
            speedBoosted = true;
            currentSpeed = moveSpeed;
        }
        Debug.Log("Speed Upgrade: " + moveSpeed);
    }

    public virtual void SpecialBoost(int boostAmount)
    {
        normalAttackDamage += boostAmount;
        AttackDamage = normalAttackDamage;
        Debug.Log("Permanent Attack Damage Upgrade: " + AttackDamage);
    }
}
