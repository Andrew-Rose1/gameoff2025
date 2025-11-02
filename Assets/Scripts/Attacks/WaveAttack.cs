using UnityEngine;

namespace WaveMagicSurvivor.Attacks
{
    public class WaveAttack : MonoBehaviour
    {
        [Header("Wave Properties")]
        [SerializeField] protected float speed = 5f;
        [SerializeField] protected float lifetime = 3f;
        [SerializeField] protected int damage = 10;
        [SerializeField] protected float radius = 0.5f;
        [SerializeField] protected float amplitude = 1f;
        [SerializeField] protected float frequency = 2f;

        [Header("Visuals")]
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected Color waveColor = Color.cyan;

        protected Vector2 direction;
        protected Vector2 initialPosition;
        protected float age = 0f;
        protected int waveType = 0; // 0 = normal, 1 = fast, 2 = slow, etc.

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();

            initialPosition = transform.position;
        }

        private void Start()
        {
            SetupVisuals();
        }

        protected virtual void SetupVisuals()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = waveColor;
                
                // Create wave-like sprite if none exists
                if (spriteRenderer.sprite == null)
                {
                    CreateWaveSprite();
                }
            }
        }

        private void CreateWaveSprite()
        {
            int size = 64;
            Texture2D texture = new Texture2D(size, size);
            Color[] colors = new Color[size * size];
            
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(size / 2, size / 2));
                    float alpha = 1f - Mathf.Clamp01(dist / (size / 2));
                    
                    // Create wave pattern
                    float wave = Mathf.Sin(dist * frequency / (size / 2) * Mathf.PI * 2) * 0.5f + 0.5f;
                    alpha *= wave;
                    
                    colors[y * size + x] = new Color(waveColor.r, waveColor.g, waveColor.b, alpha);
                }
            }
            
            texture.SetPixels(colors);
            texture.Apply();
            
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        public void Initialize(Vector2 dir, float spd = -1, int dmg = -1)
        {
            direction = dir.normalized;
            if (spd > 0) speed = spd;
            if (dmg > 0) damage = dmg;

            // Rotate to face direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        protected virtual void Update()
        {
            age += Time.deltaTime;

            if (age >= lifetime)
            {
                DestroyWave();
                return;
            }

            UpdateWaveMovement();
            CheckCollisions();
        }

        protected virtual void UpdateWaveMovement()
        {
            // Base movement
            Vector2 baseMovement = direction * speed * Time.deltaTime;
            
            // Add wave oscillation
            float waveOffset = Mathf.Sin(age * frequency) * amplitude * Time.deltaTime;
            Vector2 perpendicular = new Vector2(-direction.y, direction.x);
            Vector2 waveMovement = perpendicular * waveOffset;
            
            transform.position += (Vector3)(baseMovement + waveMovement);

            // Scale wave visual based on distance traveled
            float scale = 1f + (age / lifetime) * 0.5f;
            transform.localScale = Vector3.one * scale;
        }

        protected virtual void CheckCollisions()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    Enemies.Enemy enemy = hit.GetComponent<Enemies.Enemy>();
                    if (enemy != null && enemy.IsAlive)
                    {
                        enemy.TakeDamage(damage);
                        OnEnemyHit(enemy);
                    }
                }
            }
        }

        protected virtual void OnEnemyHit(Enemies.Enemy enemy)
        {
            // Override in derived classes for special effects
        }

        protected virtual void DestroyWave()
        {
            Destroy(gameObject);
        }

        public void InterfereWith(WaveAttack otherWave)
        {
            // Calculate interference based on wave properties
            float distance = Vector2.Distance(transform.position, otherWave.transform.position);
            
            if (distance < radius * 2)
            {
                // Constructive interference - increase damage
                damage = Mathf.RoundToInt(damage * 1.5f);
                speed *= 1.2f;
                
                // Visual effect
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.Lerp(waveColor, Color.white, 0.3f);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}

