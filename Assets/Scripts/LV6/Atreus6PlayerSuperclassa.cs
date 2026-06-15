using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// LV6 player. Inherits all shared logic (health, damage, immunity, boosts, VFX) from
// PlayerSuperclass and only customises the LV6-specific behaviour:
//  - flips the whole transform (so the child attack hitbox flips with the player),
//  - click-to-attack with no shield,
//  - drives an "isAttacking" animator bool,
//  - persists across the phase-2 scene load.
public class Atreus6PlayerSuperclassa : PlayerSuperclass
{
    public override void Start()
    {
        attackDuration = 1f;
        keepBetweenScenes = true; // LV6 carries the player across the phase-2 scene load
        base.Start();
    }

    public override void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            anim.SetTrigger("isAttacking");
            PlaySlashVFX();
            StartCoroutine(ResetAttack());
        }
    }

    // Flip the whole transform so the child attack hitbox flips with the player.
    protected override void HandleMovement()
    {
        if (Input.GetKey(RunKey))
            currentSpeed = moveSpeed * 2f;
        else
            currentSpeed = moveSpeed;

        if (Input.GetKeyDown(Spacebar) && grounded)
            Jump();

        if (!Input.GetKey(L) && !Input.GetKey(R))
            rb.velocity = new Vector2(0, rb.velocity.y);

        if (Input.GetKey(L))
        {
            rb.velocity = new Vector2(-currentSpeed, rb.velocity.y);
            transform.localScale = new Vector3(-1, 1, 1);
        }

        if (Input.GetKey(R))
        {
            rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    // Click-to-attack, and LV6 has no shield/block.
    protected override void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
            Attack();
    }

    protected override void UpdateAnimator()
    {
        base.UpdateAnimator();
        anim.SetBool("isAttacking", isAttacking);
    }
}
