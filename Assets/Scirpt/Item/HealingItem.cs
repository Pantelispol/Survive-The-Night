using SmallHedge.SoundManager;
using UnityEngine;

    public class HealingItem : Item
    {
        [SerializeField] private int healingAmount = 20;

        public override void ConsumeItem()
        {
            base.ConsumeItem();
            SoundManager.PlaySound(SoundType.Drinking, transform.position);
            if (PlayerHealthSystem.localPlayerHealth != null)
            {
                Debug.Log($"HealingItem: Adding {healingAmount} health.");
                PlayerHealthSystem.localPlayerHealth.AddHealth(healingAmount);
            }
            else
            {
                Debug.LogError("HealingItem: PlayerHealthSystem.localPlayerHealth is null.");
            }
        }

        public override bool CanConsumeItem()
        {
            return true;
        }
    }