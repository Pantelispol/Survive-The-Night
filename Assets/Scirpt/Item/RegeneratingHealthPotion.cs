using SmallHedge.SoundManager;
using System.Collections;
using UnityEngine;

public class RegeneratingHealthPotion : Item
{
    [Header("Regeneration Settings")]
    [SerializeField] private int healingPerSecond = 10; // Amount of health restored per second
    [SerializeField] private float regenerationDuration = 5f; // Total duration of the regeneration

    public override void ConsumeItem()
    {
        base.ConsumeItem();

        // Play drinking sound
        SoundManager.PlaySound(SoundType.Drinking, transform.position);

        if (PlayerHealthSystem.localPlayerHealth != null)
        {
            Debug.Log($"RegeneratingHealthPotion: Starting health regeneration for {regenerationDuration} seconds.");
            PlayerHealthSystem.localPlayerHealth.StartCoroutine(RegenerateHealth());
        }
        else
        {
            Debug.LogError("RegeneratingHealthPotion: PlayerHealthSystem.localPlayerHealth is null.");
        }
    }

    private IEnumerator RegenerateHealth()
    {
        float elapsedTime = 0f;

        while (elapsedTime < regenerationDuration)
        {
            // Heal the player
            PlayerHealthSystem.localPlayerHealth.AddHealth(healingPerSecond);

            // Wait for 1 second
            yield return new WaitForSeconds(1f);

            // Increment elapsed time
            elapsedTime += 1f;
        }

        Debug.Log("RegeneratingHealthPotion: Health regeneration complete.");
    }

    public override bool CanConsumeItem()
    {
        return true;
    }
}
