using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// LV5 player. Inherits movement, health, immunity, boosts, the attack hitbox and the
// slash VFX from PlayerSuperclass; only the attack trigger differs.
public class Atreus5 : PlayerSuperclass
{
    public override void Start()
    {
        attackDuration = 1f;
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
}
