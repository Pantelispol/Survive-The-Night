using SmallHedge.SoundManager;
using UnityEngine;

public class CombatState : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private GameObject _currentWeaponInHand;

    public void DrawWeapon(GameObject weapon, Transform weaponHolder)
    {
        _currentWeaponInHand = Instantiate(weapon, weaponHolder.position, weaponHolder.rotation, weaponHolder);
        _animator.SetTrigger("drawWeapon");
        SoundManager.PlaySound(SoundType.Drawsword, transform.position);
    }


    public bool IsDrawingWeapon()
    {
        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("DrawWeapon") && stateInfo.normalizedTime < 1.0f;
    }


    public void SheathWeapon()
    {
        if (_currentWeaponInHand == null)
        {
            Debug.LogWarning("No weapon to sheath.");
            return;
        }

        EndDealDamage();
        Destroy(_currentWeaponInHand);
        _currentWeaponInHand = null; // Ensure the reference is cleared
        _animator.SetTrigger("sheathWeapon");
        SoundManager.PlaySound(SoundType.SheathSword, transform.position);
    }

    public void StartDealDamage()
    {
        if (_currentWeaponInHand != null)
        {
            var damageDealer = _currentWeaponInHand.GetComponentInChildren<DamageDealer>();
            if (damageDealer != null)
            {
                damageDealer.StartDealDamage();
                Debug.Log("Started dealing damage.");
            }
            else
            {
                Debug.LogError("DamageDealer component not found on the current weapon.");
            }
        }
        else
        {
            Debug.LogError("No weapon in hand to start dealing damage.");
        }
    }

    public void EndDealDamage()
    {
        if (_currentWeaponInHand != null)
        {
            var damageDealer = _currentWeaponInHand.GetComponentInChildren<DamageDealer>();
            if (damageDealer != null)
            {
                damageDealer.EndDealDamage();
                Debug.Log("Stopped dealing damage.");
            }
            else
            {
                Debug.LogError("DamageDealer component not found on the current weapon.");
            }
        }
        else
        {
            Debug.LogWarning("No weapon in hand to end dealing damage."); // Changed to warning
        }
    }
}