using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    bool canDealDamage;
    List<GameObject> hasDealtDamage;

    [SerializeField] float weaponLength;
    [SerializeField] float weaponDamage;
    [SerializeField] LayerMask enemyLayer;
    void Start()
    {
        canDealDamage = false;
        hasDealtDamage = new List<GameObject>();
    }

    void Update()
    {
        if (canDealDamage)
        {
            RaycastHit hit;

            //int layerMask = 1 << 9;
            if (Physics.Raycast(transform.position, -transform.up, out hit, weaponLength, enemyLayer))
            {
                Debug.DrawRay(transform.position, -transform.up * weaponLength, Color.red, 0.5f);
                if (hit.transform.TryGetComponent(out Enemy enemy) && !hasDealtDamage.Contains(hit.transform.gameObject))
                    if (!hasDealtDamage.Contains(hit.transform.gameObject))
                    {
                        enemy.TakeDamage(weaponDamage);
                        enemy.HitVFX(hit.point);
                        hasDealtDamage.Add(hit.transform.gameObject);
                    }
            }
        }
    }
    public void StartDealDamage()
    {
        Debug.Log("Animation Event  StartDealDamage fired");
        canDealDamage = true;
        hasDealtDamage.Clear();
    }

    public void EndDealDamage()
    {
        Debug.Log("Animation Event  EndDealDamage fired");
        canDealDamage = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * weaponLength);
    }
}