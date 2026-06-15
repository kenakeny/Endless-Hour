// ============ CorenPhase2.cs ============
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorenPhase2 : CorenState
{
    [Header("Clock Sweep")]
    public GameObject clockHandPrefab;
    public float clockSweepDamage = 25f;
    public float clockSweepRadius = 4f;
    public float clockSweepDuration = 1.5f;

    [Header("Aura Burst")]
    public Sprite[] auraBurstSpritesheet;
    public float auraBurstDamage = 20f;
    public float auraBurstRadius = 5f;
    public float auraBurstDuration = 2f;
    public float auraBurstStartScale = 0.5f;
    public float auraBurstEndScale = 3f;
    public float spriteFrameTime = 0.1f;

    [Header("Attack Timing")]
    private float attackCooldown = 8f;
    private float attackTimer = 8f;
    private int lastAttack = -1;

    public override void EnterState()
    {
        Debug.Log("Entered Coren Phase 2");
        coren.anim.SetTrigger("Phase2");
        attackTimer = 0f;
    }

    public override void UpdateState()
    {
        if (machine.activeState != this)
            return;

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            if (lastAttack == 0)
            {
                AuraBurst();
                lastAttack = 1;
            }
            else
            {
                ClockSweep();
                lastAttack = 0;
            }
            attackTimer = 0f;
        }
    }

    private void ClockSweep()
    {
        Debug.Log("Clock Sweep triggered!");
        coren.anim.SetTrigger("Attack1");
        StartCoroutine(ClockSweepSequence());
    }

    private IEnumerator ClockSweepSequence()
    {
        yield return new WaitForSeconds(0.5f);

        if (clockHandPrefab == null)
        {
            Debug.LogError("CLOCK HAND PREFAB NOT ASSIGNED!");
            yield break;
        }

        Vector3 clockPos = new Vector3(coren.transform.position.x, 0f, 0f);
        GameObject clockHand = Instantiate(clockHandPrefab, clockPos, Quaternion.identity);
        Debug.Log("Clock instantiated at " + clockPos);
        clockHand.SetActive(true);

        float elapsedTime = 0f;
        float rotationSpeed = 360f / clockSweepDuration;

        while (elapsedTime < clockSweepDuration && clockHand != null)
        {
            elapsedTime += Time.deltaTime;
            float newRotation = rotationSpeed * elapsedTime;
            clockHand.transform.rotation = Quaternion.AngleAxis(newRotation, Vector3.forward);
            yield return null;
        }

        if (clockHand != null)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(clockPos, clockSweepRadius);
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    hit.GetComponent<Atreus6PlayerSuperclassa>().TakeDamage((int)clockSweepDamage);
                    Debug.Log("Player hit by Clock Sweep!");
                    break;
                }
            }

            Destroy(clockHand);
        }
    }

    private void AuraBurst()
    {
        Debug.Log("Aura Burst triggered!");
        coren.anim.SetTrigger("Attack2");
        StartCoroutine(AuraBurstSequence());
    }

    private IEnumerator AuraBurstSequence()
    {
        yield return new WaitForSeconds(0.3f);

        if (auraBurstSpritesheet == null || auraBurstSpritesheet.Length == 0)
        {
            Debug.LogError("AURA BURST SPRITESHEET NOT ASSIGNED!");
            yield break;
        }

        GameObject auraBurst = new GameObject("AuraBurst");
        auraBurst.transform.position = coren.transform.position;
        SpriteRenderer spriteRenderer = auraBurst.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -1;

        float elapsedTime = 0f;
        float frameTimer = 0f;

        while (elapsedTime < auraBurstDuration && auraBurst != null)
        {
            elapsedTime += Time.deltaTime;
            frameTimer += Time.deltaTime;
            
            if (frameTimer >= spriteFrameTime)
            {
                int frameIndex = (int)(elapsedTime / spriteFrameTime) % auraBurstSpritesheet.Length;
                spriteRenderer.sprite = auraBurstSpritesheet[frameIndex];
                frameTimer = 0f;
            }
            
            float scale = Mathf.Lerp(auraBurstStartScale, auraBurstEndScale, elapsedTime / auraBurstDuration);
            auraBurst.transform.localScale = new Vector3(scale, scale, 1);
            
            Collider2D[] hits = Physics2D.OverlapCircleAll(coren.transform.position, auraBurstRadius * (scale / auraBurstEndScale));
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    hit.GetComponent<Atreus6PlayerSuperclassa>().TakeDamage((int)auraBurstDamage);
                    Debug.Log("Player hit by Aura Burst!");
                    break;
                }
            }

            yield return null;
        }

        if (auraBurst != null)
            Destroy(auraBurst);
    }
}