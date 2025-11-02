using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WaveMagicSurvivor.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Wave Settings")]
        [SerializeField] private float waveStartDelay = 2f;
        [SerializeField] private float timeBetweenWaves = 5f;
        [SerializeField] private int enemiesPerWaveBase = 5;
        [SerializeField] private float enemyIncreasePerWave = 1.5f;

        [Header("Game Boundaries")]
        [SerializeField] private float gameAreaRadius = 10f;

        private int currentWave = 0;
        private bool gameActive = false;
        private int enemiesAlive = 0;
        private float waveTimer = 0f;

        public int CurrentWave => currentWave;
        public bool IsGameActive => gameActive;
        public float GameAreaRadius => gameAreaRadius;
        public int EnemiesAlive => enemiesAlive;

        public System.Action<int> OnWaveStarted;
        public System.Action<int> OnWaveCompleted;
        public System.Action OnGameOver;
        public System.Action<int> OnEnemyCountChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            gameActive = true;
            currentWave = 0;
            StartCoroutine(WaveManager());
        }

        private IEnumerator WaveManager()
        {
            yield return new WaitForSeconds(waveStartDelay);

            while (gameActive)
            {
                currentWave++;
                OnWaveStarted?.Invoke(currentWave);

                int enemiesToSpawn = Mathf.RoundToInt(enemiesPerWaveBase * Mathf.Pow(enemyIncreasePerWave, currentWave - 1));
                Enemies.EnemySpawner.Instance.SpawnWave(enemiesToSpawn, currentWave);
                enemiesAlive = enemiesToSpawn;
                OnEnemyCountChanged?.Invoke(enemiesAlive);

                // Wait for all enemies to be defeated
                while (enemiesAlive > 0 && gameActive)
                {
                    yield return new WaitForSeconds(0.1f);
                }

                if (gameActive)
                {
                    OnWaveCompleted?.Invoke(currentWave);
                    yield return new WaitForSeconds(timeBetweenWaves);
                }
            }
        }

        public void EnemyDefeated()
        {
            enemiesAlive = Mathf.Max(0, enemiesAlive - 1);
            OnEnemyCountChanged?.Invoke(enemiesAlive);
        }

        public void GameOver()
        {
            gameActive = false;
            OnGameOver?.Invoke();
            StopAllCoroutines();
        }

        public bool IsInGameArea(Vector3 position)
        {
            return Vector3.Distance(position, Vector3.zero) <= gameAreaRadius;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(Vector3.zero, gameAreaRadius);
        }
    }
}

