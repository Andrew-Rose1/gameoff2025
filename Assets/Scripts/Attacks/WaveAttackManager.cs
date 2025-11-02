using UnityEngine;
using System.Collections.Generic;

namespace WaveMagicSurvivor.Attacks
{
    public class WaveAttackManager : MonoBehaviour
    {
        public static WaveAttackManager Instance { get; private set; }

        [Header("Auto Attack Settings")]
        [SerializeField] private WaveAttack waveAttackPrefab;
        [SerializeField] private float baseAttackInterval = 1f;
        [SerializeField] private int baseDamage = 10;
        [SerializeField] private float baseSpeed = 5f;
        [SerializeField] private int numberOfDirections = 4;

        [Header("Upgrades")]
        [SerializeField] private float attackSpeedMultiplier = 1f;
        [SerializeField] private float damageMultiplier = 1f;
        [SerializeField] private float speedMultiplier = 1f;
        [SerializeField] private int additionalWaves = 0;

        private float attackTimer = 0f;
        private Transform playerTransform;
        private List<WaveAttack> activeWaves = new List<WaveAttack>();

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
            CreateDefaultPrefab();
        }

        private void Update()
        {
            if (playerTransform == null)
                return;
                
            if (Core.GameManager.Instance == null || !Core.GameManager.Instance.IsGameActive)
                return;

            attackTimer += Time.deltaTime;
            float interval = baseAttackInterval / attackSpeedMultiplier;

            if (attackTimer >= interval)
            {
                attackTimer = 0f;
                FireWaveAttack();
            }

            CleanupDestroyedWaves();
        }

        private void FindPlayer()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        private void FireWaveAttack()
        {
            if (waveAttackPrefab == null || playerTransform == null) return;

            int wavesToFire = 1 + additionalWaves;
            int directions = numberOfDirections + (additionalWaves > 0 ? 2 : 0);

            for (int i = 0; i < wavesToFire; i++)
            {
                float angleStep = 360f / directions;
                
                for (int j = 0; j < directions; j++)
                {
                    float angle = (angleStep * j) + (angleStep * i / wavesToFire);
                    Vector2 direction = new Vector2(
                        Mathf.Cos(angle * Mathf.Deg2Rad),
                        Mathf.Sin(angle * Mathf.Deg2Rad)
                    );

                    CreateWave(direction);
                }
            }
        }

        private void CreateWave(Vector2 direction)
        {
            if (waveAttackPrefab == null || playerTransform == null) return;

            GameObject waveObj = Instantiate(waveAttackPrefab.gameObject, playerTransform.position, Quaternion.identity);
            WaveAttack wave = waveObj.GetComponent<WaveAttack>();
            
            if (wave != null)
            {
                wave.Initialize(
                    direction,
                    baseSpeed * speedMultiplier,
                    Mathf.RoundToInt(baseDamage * damageMultiplier)
                );
                activeWaves.Add(wave);
            }
        }

        private void CleanupDestroyedWaves()
        {
            activeWaves.RemoveAll(wave => wave == null);
        }

        // Upgrade methods
        public void UpgradeAttackSpeed(float multiplier)
        {
            attackSpeedMultiplier *= multiplier;
        }

        public void UpgradeDamage(float multiplier)
        {
            damageMultiplier *= multiplier;
        }

        public void UpgradeSpeed(float multiplier)
        {
            speedMultiplier *= multiplier;
        }

        public void AddWave()
        {
            additionalWaves++;
        }

        public void SetNumberOfDirections(int directions)
        {
            numberOfDirections = Mathf.Max(2, directions);
        }

        private void CreateDefaultPrefab()
        {
            if (waveAttackPrefab != null) return;

            GameObject defaultPrefab = new GameObject("WaveAttackPrefab");
            
            SpriteRenderer sr = defaultPrefab.AddComponent<SpriteRenderer>();
            sr.color = Color.cyan;
            sr.sortingOrder = 2;
            
            CircleCollider2D collider = defaultPrefab.AddComponent<CircleCollider2D>();
            collider.radius = 0.5f;
            collider.isTrigger = true;
            
            defaultPrefab.AddComponent<WaveAttack>();
            
            waveAttackPrefab = defaultPrefab.GetComponent<WaveAttack>();
            
            Debug.LogWarning("WaveAttackManager: No prefab assigned. Created default. Please assign a proper prefab in the inspector.");
        }
    }
}

