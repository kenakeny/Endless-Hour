// ============ CorenPhase1.cs ============
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorenPhase1 : CorenState
{
    [Header("Time Reversal Box")]
    public GameObject timeReversalBoxPrefab;
    public float boxDuration = 10f;
    public float boxHealAmount = 30f;
    public float boxDamageAmount = 15f;
    public float boxRadius = 3f;

    [Header("Attack Timing")]
    private float attackCooldown = 10f;
    private float attackTimer = 10f;

    public override void EnterState()
    {
        Debug.Log("Entered Coren Phase 1");
        coren.anim.SetTrigger("Phase1");
        attackTimer = 0f;
    }

    public override void UpdateState()
    {
        if (machine.activeState != this)
            return;

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            TimeReversal();
            attackTimer = 0f;
        }
    }

    private void TimeReversal()
    {
        coren.anim.SetTrigger("Attack1");
        StartCoroutine(TimeReversalSequence());
    }

    private IEnumerator TimeReversalSequence()
    {
        Vector3 boxPos = coren.playerTransform.position;
        boxPos.y += 1f;
        GameObject box = Instantiate(timeReversalBoxPrefab, boxPos, Quaternion.identity);
        box.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

        box.SetActive(true);

        yield return new WaitForSeconds(boxDuration);

        if (box == null)
            yield break;

        Collider2D[] hits = Physics2D.OverlapCircleAll(boxPos, boxRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<Atreus6PlayerSuperclassa>().TakeDamage((int)boxDamageAmount);
                coren.currentHealth += (int)boxHealAmount;
                coren.currentHealth = Mathf.Min(coren.currentHealth, coren.maxHealth);
                Debug.Log("Coren healed for " + boxHealAmount);
                break;
            }
        }

        if (box != null)
            Destroy(box);
    }
}