using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageDealer : MonoBehaviour
{
    bool canDealDamage;
    bool hasDealtDamage;

    [SerializeField] float weaponLength;
    [SerializeField] private float baseDamage;

    private Enemy enemyScript;

    void Start()
    {
        canDealDamage = false;
        hasDealtDamage = false;

        enemyScript = GetComponentInParent<Enemy>();
        SetBaseDamage();
    }

    void Update()
    {
        if (canDealDamage && !hasDealtDamage)
        {
            RaycastHit hit;

            int layerMask = 1 << 8;
            if (Physics.Raycast(transform.position, -transform.up, out hit, weaponLength, layerMask))
            {
                if (hit.transform.TryGetComponent(out PlayerHealthSystem health))
                {
                    float currentDamage = CalculateDamage();
                    health.TakeDamage(currentDamage);
                    health.HitVFX(hit.point);
                    hasDealtDamage = true;
                    Debug.Log("Damage dealt: " + currentDamage);
                }
            }
        }
    }

    private void SetBaseDamage()
    {
        if (enemyScript == null)
            return;

        if (enemyScript.CompareTag("Skeleton"))
        {
            baseDamage = 10f;
        }
        else if (enemyScript.CompareTag("Zombie"))
        {
            baseDamage = 15f; 
        }
    }


    private float CalculateDamage()
    {
        int day = TimeManager.Instance != null ? TimeManager.Instance.Days : 0;

        float multiplier;

        if (enemyScript.CompareTag("Skeleton"))
        {
            multiplier = 1 + (day * 0.2f); 
        }
        else if (enemyScript.CompareTag("Zombie"))
        {
            multiplier = 1 + (day * 0.3f); 
        }
        else
        {
            multiplier = 1 + (day * 0.2f); 
        }

        return baseDamage * multiplier;
    }



    public void StartDealDamage()
    {
        canDealDamage = true;
        hasDealtDamage = false;
    }

    public void EndDealDamage()
    {
        canDealDamage = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * weaponLength);
    }
}
