using UnityEngine;

public class FireDamage : MonoBehaviour
{
    [SerializeField] private float damagePerSecond = 10f; // Damage dealt per second
    private float damageInterval = 1f; // Interval between damage ticks
    private float nextDamageTime;

    private void OnTriggerStay(Collider other)
    {
        // Check if the player is in the fire
        if (other.CompareTag("Player"))
        {
            // Get the PlayerHealthSystem component
            PlayerHealthSystem playerHealth = other.GetComponent<PlayerHealthSystem>();
            if (playerHealth != null && Time.time >= nextDamageTime)
            {
                // Apply damage to the player
                playerHealth.TakeDamage(damagePerSecond);
                nextDamageTime = Time.time + damageInterval; // Set the next damage time
            }
        }
    }
}
