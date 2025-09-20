using UnityEngine;
using SmallHedge.SoundManager;
using Script.Controller;

public class StaminaRegenPotion : Item
{
    [SerializeField] private float regenMultiplier = 2f;
    [SerializeField] private float duration = 10f;

    public override void ConsumeItem()
    {
        base.ConsumeItem();

        SoundManager.PlaySound(SoundType.Drinking, transform.position);

        var player = FindObjectOfType<PlayerLocomotionInput>();
        if (player != null)
        {
            player.ApplyStaminaRegenBoost(regenMultiplier, duration);
            Debug.Log($"Stamina potion consumed: x{regenMultiplier} regen for {duration} seconds.");
        }
        else
        {
            Debug.LogError("Stamina potion error: PlayerLocomotionInput not found.");
        }
    }

    public override bool CanConsumeItem()
    {
        return true;
    }
}
