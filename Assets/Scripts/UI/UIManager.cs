using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace WaveMagicSurvivor.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private Slider healthSlider;
        [SerializeField] private TextMeshProUGUI healthText;

        [Header("Wave Info")]
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private TextMeshProUGUI enemiesRemainingText;

        [Header("Game Over")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI finalWaveText;

        private Player.PlayerController player;
        private Core.GameManager gameManager;

        private void Start()
        {
            FindReferences();
            SetupHealthBar();
            HideGameOverPanel();
            
            // Subscribe to events
            if (player != null)
            {
                player.OnHealthChanged += UpdateHealthBar;
                player.OnPlayerDeath += ShowGameOver;
            }

            if (gameManager != null)
            {
                gameManager.OnWaveStarted += UpdateWaveInfo;
                gameManager.OnEnemyCountChanged += UpdateEnemyCount;
                gameManager.OnGameOver += ShowGameOver;
            }
        }

        private void FindReferences()
        {
            if (player == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                    player = playerObj.GetComponent<Player.PlayerController>();
            }

            if (gameManager == null)
            {
                gameManager = Core.GameManager.Instance;
            }
        }

        private void SetupHealthBar()
        {
            if (healthSlider != null && player != null)
            {
                healthSlider.maxValue = player.MaxHealth;
                healthSlider.value = player.CurrentHealth;
            }
        }

        private void UpdateHealthBar(int current, int max)
        {
            if (healthSlider != null)
            {
                healthSlider.value = current;
                healthSlider.maxValue = max;
            }

            if (healthText != null)
            {
                healthText.text = $"{current} / {max}";
            }
        }

        private void UpdateWaveInfo(int waveNumber)
        {
            if (waveText != null)
            {
                waveText.text = $"Wave {waveNumber}";
            }
        }

        private void UpdateEnemyCount(int count)
        {
            if (enemiesRemainingText != null)
            {
                enemiesRemainingText.text = $"Enemies: {count}";
            }
        }

        private void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                
                if (finalWaveText != null && gameManager != null)
                {
                    finalWaveText.text = $"You Survived {gameManager.CurrentWave} Waves!";
                }
            }
        }

        private void HideGameOverPanel()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (player != null)
            {
                player.OnHealthChanged -= UpdateHealthBar;
                player.OnPlayerDeath -= ShowGameOver;
            }

            if (gameManager != null)
            {
                gameManager.OnWaveStarted -= UpdateWaveInfo;
                gameManager.OnEnemyCountChanged -= UpdateEnemyCount;
                gameManager.OnGameOver -= ShowGameOver;
            }
        }
    }
}

