using UnityEngine;

namespace WaveMagicSurvivor.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private int maxHealth = 30;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private int damage = 10;
        [SerializeField] private int scoreValue = 10;
        [SerializeField] private float minDistanceFromPlayer = 0.5f;

        [Header("Visuals")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color damageColor = Color.red;

        private int currentHealth;
        private Transform playerTransform;
        private Rigidbody2D rb;
        private Color originalColor;
        private float colorFlashTimer = 0f;

        public bool IsAlive => currentHealth > 0;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
                originalColor = spriteRenderer.color;

            rb.gravityScale = 0;
            currentHealth = maxHealth;
        }

        private void Start()
        {
            FindPlayer();
        }

        private void Update()
        {
            UpdateColorFlash();
        }

        private void FixedUpdate()
        {
            MoveTowardsPlayer();
        }

        private void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        private void MoveTowardsPlayer()
        {
            if (playerTransform == null)
            {
                FindPlayer();
                return;
            }

            Vector2 direction = (playerTransform.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, playerTransform.position);

            if (distance > minDistanceFromPlayer)
            {
                rb.linearVelocity = direction * moveSpeed;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }

            // Face movement direction
            if (rb.linearVelocity.magnitude > 0.1f)
            {
                float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        public void TakeDamage(int damageAmount)
        {
            if (!IsAlive) return;

            currentHealth -= damageAmount;
            FlashDamageColor();

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void FlashDamageColor()
        {
            colorFlashTimer = 0.2f;
            if (spriteRenderer != null)
                spriteRenderer.color = damageColor;
        }

        private void UpdateColorFlash()
        {
            if (colorFlashTimer > 0)
            {
                colorFlashTimer -= Time.deltaTime;
                if (colorFlashTimer <= 0 && spriteRenderer != null)
                {
                    spriteRenderer.color = originalColor;
                }
            }
        }

        private void Die()
        {
            Core.GameManager.Instance?.EnemyDefeated();
            // Spawn death effect here if needed
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Player.PlayerController player = other.GetComponent<Player.PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(damage);
                }
            }
        }
    }
}

