using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
   [SerializeField] private int maxHealth = 100;

    [SerializeField] private int currentHealth;

    public static PlayerHealth localPlayerHealth { get; private set; }


    protected void OnSpawned()
    {
        localPlayerHealth = this;
        currentHealth = maxHealth;
    

        if(!InstanceHandler.TryGetInstance(out HUDManager hudManager))
            {
                Debug.LogError("HUDManager instance not found.");
                return;
            }
        hudManager.SetMaxHealth(maxHealth);
        hudManager.SetHealth(maxHealth);
    }

    protected void OnDespawned()
    {
        localPlayerHealth = null;
    }

    public void AddHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        if (!InstanceHandler.TryGetInstance(out HUDManager hudManager))
        {
            Debug.LogError("HUDManager instance not found.");
            return;
        }
        hudManager.SetHealth(currentHealth);
    }



}
