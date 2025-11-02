using UnityEngine;
using System.Collections;

namespace WaveMagicSurvivor.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        public static EnemySpawner Instance { get; private set; }

        [Header("Spawn Settings")]
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private float spawnDelay = 0.5f;
        [SerializeField] private float minSpawnDistance = 8f;
        [SerializeField] private float maxSpawnDistance = 12f;
        [SerializeField] private float minDistanceFromPlayer = 5f;

        private Transform playerTransform;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            FindPlayer();
            
            // Create default enemy prefab if none assigned
            if (enemyPrefab == null)
            {
                CreateDefaultEnemyPrefab();
            }
        }

        private void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        public void SpawnWave(int enemyCount, int waveNumber)
        {
            StartCoroutine(SpawnWaveCoroutine(enemyCount, waveNumber));
        }

        private IEnumerator SpawnWaveCoroutine(int enemyCount, int waveNumber)
        {
            FindPlayer();

            for (int i = 0; i < enemyCount; i++)
            {
                SpawnEnemy(waveNumber);
                yield return new WaitForSeconds(spawnDelay);
            }
        }

        private void SpawnEnemy(int waveNumber)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            
            // Scale enemy stats based on wave number (optional)
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null && waveNumber > 1)
            {
                // Could modify enemy stats here if needed
            }
        }

        private Vector3 GetRandomSpawnPosition()
        {
            Vector3 spawnPos;
            int attempts = 0;
            float gameRadius = Core.GameManager.Instance != null 
                ? Core.GameManager.Instance.GameAreaRadius 
                : 10f;

            do
            {
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
                spawnPos = new Vector3(
                    Mathf.Cos(angle) * distance,
                    Mathf.Sin(angle) * distance,
                    0
                );
                attempts++;
            }
            while (attempts < 20 && 
                   playerTransform != null && 
                   Vector3.Distance(spawnPos, playerTransform.position) < minDistanceFromPlayer);

            return spawnPos;
        }

        private void CreateDefaultEnemyPrefab()
        {
            // Create a simple default enemy prefab
            GameObject defaultEnemy = new GameObject("DefaultEnemy");
            defaultEnemy.tag = "Enemy";
            
            // Add sprite renderer with a simple colored sprite
            SpriteRenderer sr = defaultEnemy.AddComponent<SpriteRenderer>();
            sr.color = Color.red;
            sr.sortingOrder = 1;
            
            // Create a simple square sprite programmatically
            Texture2D texture = new Texture2D(32, 32);
            Color[] colors = new Color[32 * 32];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = Color.red;
            texture.SetPixels(colors);
            texture.Apply();
            sr.sprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);

            // Add components
            defaultEnemy.AddComponent<Rigidbody2D>();
            CircleCollider2D collider = defaultEnemy.AddComponent<CircleCollider2D>();
            collider.radius = 0.4f;
            collider.isTrigger = true;
            defaultEnemy.AddComponent<Enemy>();

            enemyPrefab = defaultEnemy;
            
            Debug.LogWarning("EnemySpawner: No enemy prefab assigned. Created default enemy. Please assign a proper prefab in the inspector.");
        }
    }
}

