using UnityEngine;
using UnityEngine.InputSystem;

namespace WaveMagicSurvivor.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float deceleration = 10f;

        [Header("Health")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private float invincibilityDuration = 1f;

        [Header("References")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Rigidbody2D rb;
        private Vector2 moveInput;
        private int currentHealth;
        private float invincibilityTimer = 0f;
        private bool isInvincible = false;

        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;
        public bool IsInvincible => isInvincible;

        public System.Action<int, int> OnHealthChanged;
        public System.Action OnPlayerDeath;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            rb.gravityScale = 0;
            rb.linearDamping = 10f;
            currentHealth = maxHealth;
        }

        private void Update()
        {
            HandleInvincibility();
            HandleBoundaries();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        private void HandleMovement()
        {
            if (moveInput.magnitude > 0.1f)
            {
                Vector2 targetVelocity = moveInput * moveSpeed;
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
            }
            else
            {
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            }
        }

        private void HandleBoundaries()
        {
            if (Core.GameManager.Instance != null)
            {
                float radius = Core.GameManager.Instance.GameAreaRadius;
                Vector3 position = transform.position;
                
                if (Vector3.Distance(position, Vector3.zero) > radius)
                {
                    Vector3 direction = (Vector3.zero - position).normalized;
                    transform.position = direction * radius;
                }
            }
        }

        private void HandleInvincibility()
        {
            if (isInvincible)
            {
                invincibilityTimer -= Time.deltaTime;
                
                // Flash effect
                if (spriteRenderer != null)
                {
                    float flashSpeed = 10f;
                    spriteRenderer.color = Mathf.Sin(invincibilityTimer * flashSpeed) > 0 
                        ? new Color(1, 1, 1, 0.5f) 
                        : Color.white;
                }

                if (invincibilityTimer <= 0)
                {
                    isInvincible = false;
                    if (spriteRenderer != null)
                        spriteRenderer.color = Color.white;
                }
            }
        }

        public void TakeDamage(int damage)
        {
            if (isInvincible) return;

            currentHealth = Mathf.Max(0, currentHealth - damage);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                Die();
            }
            else
            {
                isInvincible = true;
                invincibilityTimer = invincibilityDuration;
            }
        }

        public void Heal(int amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        private void Die()
        {
            OnPlayerDeath?.Invoke();
            Core.GameManager.Instance?.GameOver();
            // Disable player
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                TakeDamage(10); // Default damage from enemy contact
            }
        }
    }
}

