using UnityEngine;
using UnityEngine.UI;

    public class HUDManager : MonoBehaviour
    {
        [SerializeField] private Slider healthSlider;
        [SerializeField] private Slider staminaSlider;

    private void Awake()
        {
            InstanceHandler.RegisterInstance(this);
        }

        private void OnDestroy()
        {
            InstanceHandler.UnregisterInstance<HUDManager>();
        }

        public void SetMaxHealth(float maxHealth)
        {
            healthSlider.maxValue = maxHealth;
        }

        public void SetHealth(float health)
        {
            healthSlider.value = health;
        }

    public void SetMaxStamina(float maxStamina)
    {
        staminaSlider.maxValue = maxStamina;
    }

    public void SetStamina(float stamina)
    {
        staminaSlider.value = stamina;
    }
}