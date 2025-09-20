using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class PlayerHealthSystem : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth;
        [SerializeField] private float health = 100; // For compatibility with existing logic
        [SerializeField] private GameObject hitVFX;
        [SerializeField] private GameObject ragdoll;
        [SerializeField] private Transform _camera;


        [Header("References")]
        private Animator animator;
        [SerializeField] private GameOverScreen gameOverScreen; // Assign via Inspector
        [SerializeField] private TimeManager timeManager;

        public static PlayerHealthSystem localPlayerHealth { get; private set; }

        private void Start()
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component not found on " + gameObject.name);
            }

            // Initialize health
            currentHealth = maxHealth;
            localPlayerHealth = this;

            if (!InstanceHandler.TryGetInstance(out HUDManager hudManager))
            {
                Debug.LogError("HUDManager instance not found.");
                return;
            }
            hudManager.SetMaxHealth(maxHealth);
            hudManager.SetHealth(maxHealth);
        }   

        private void OnDestroy()
        {
            if (localPlayerHealth == this)
            {
                localPlayerHealth = null;
            }
        }

        public int GetCurrentHealth()
        {
            return currentHealth;
        }

        public void TakeDamage(float damageAmount)
        {
            // Subtract damage from currentHealth
            currentHealth -= Mathf.RoundToInt(damageAmount);
            health -= damageAmount; // For compatibility with existing logic

            if (currentHealth < 0)
                currentHealth = 0;

            // Update HUD
            if (InstanceHandler.TryGetInstance(out HUDManager hudManager))
            {
                hudManager.SetHealth(currentHealth);
            }

            // Trigger damage animation
            if (animator != null)
            {
                animator.SetTrigger("damage");
            }
            else
            {
                Debug.LogError("Animator is null on " + gameObject.name);
            }

            // Trigger camera shake
            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.ShakeCamera(3f, 0.5f);
            }
            else
            {
                Debug.LogError("CameraShake instance is null");
            }

            // Check if health is zero or below
            if (currentHealth <= 0)
            {
                Die();
            }
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

        private void Die()
    {
        //Instantiate a ragdoll at the player's position
        Instantiate(ragdoll, transform.position, transform.rotation);

        // Create a free-look camera at the player's death location
        GameObject freeLookCamera = new GameObject("FreeLookCamera");
        Camera cameraComponent = freeLookCamera.AddComponent<Camera>();
        //freeLookCamera.AddComponent<FreeLookCamera>();

        // Position the camera slightly above the player's death location
        freeLookCamera.transform.position = _camera.position;
        freeLookCamera.transform.rotation = _camera.rotation;

        //Destroy the player GameObject
        Destroy(this.gameObject);
        int days = timeManager.GetDays();
            int hours = timeManager.GetHours();
            Debug.Log(days);
            Debug.Log(hours);
            // Activate game over screen
            if (gameOverScreen != null)
                gameOverScreen.Setup(days, hours);
            else
                Debug.LogError("GameOverScreen reference missing!");
        }

        public void HitVFX(Vector3 hitPosition)
        {
            GameObject hit = Instantiate(hitVFX, hitPosition, Quaternion.identity);
            Destroy(hit, 3f);
        }
    }