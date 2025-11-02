using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace WaveMagicSurvivor.UI
{
    [System.Serializable]
    public class UpgradeOption
    {
        public string name;
        public string description;
        public System.Action onSelect;
    }

    public class UpgradeSystem : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject upgradePanel;
        [SerializeField] private Transform upgradeButtonParent;
        [SerializeField] private GameObject upgradeButtonPrefab;
        [SerializeField] private TextMeshProUGUI upgradeTitleText;

        [Header("Upgrade Options")]
        [SerializeField] private int upgradesPerWave = 3;

        private List<UpgradeOption> availableUpgrades = new List<UpgradeOption>();

        private void Start()
        {
            if (upgradePanel != null)
                upgradePanel.SetActive(false);

            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnWaveCompleted += ShowUpgrades;
            }
        }

        private void ShowUpgrades(int waveNumber)
        {
            if (upgradePanel == null) return;

            GenerateUpgradeOptions();
            upgradePanel.SetActive(true);
            Time.timeScale = 0f; // Pause game
        }

        private void GenerateUpgradeOptions()
        {
            availableUpgrades.Clear();
            
            // Clear existing buttons
            if (upgradeButtonParent != null)
            {
                foreach (Transform child in upgradeButtonParent)
                {
                    Destroy(child.gameObject);
                }
            }

            // Generate upgrade options
            availableUpgrades.Add(new UpgradeOption
            {
                name = "Attack Speed",
                description = "Faster wave attacks",
                onSelect = () => { Attacks.WaveAttackManager.Instance?.UpgradeAttackSpeed(1.2f); }
            });

            availableUpgrades.Add(new UpgradeOption
            {
                name = "Damage Up",
                description = "+20% wave damage",
                onSelect = () => { Attacks.WaveAttackManager.Instance?.UpgradeDamage(1.2f); }
            });

            availableUpgrades.Add(new UpgradeOption
            {
                name = "Speed Boost",
                description = "Waves move 20% faster",
                onSelect = () => { Attacks.WaveAttackManager.Instance?.UpgradeSpeed(1.2f); }
            });

            availableUpgrades.Add(new UpgradeOption
            {
                name = "Extra Wave",
                description = "Fire additional waves",
                onSelect = () => { Attacks.WaveAttackManager.Instance?.AddWave(); }
            });

            availableUpgrades.Add(new UpgradeOption
            {
                name = "Multi-Direction",
                description = "Fire in more directions",
                onSelect = () => 
                { 
                    if (Attacks.WaveAttackManager.Instance != null)
                    {
                        Attacks.WaveAttackManager.Instance.SetNumberOfDirections(6);
                    }
                }
            });

            // Shuffle and pick random upgrades
            for (int i = availableUpgrades.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                UpgradeOption temp = availableUpgrades[i];
                availableUpgrades[i] = availableUpgrades[randomIndex];
                availableUpgrades[randomIndex] = temp;
            }

            // Create buttons for selected upgrades
            int count = Mathf.Min(upgradesPerWave, availableUpgrades.Count);
            for (int i = 0; i < count; i++)
            {
                CreateUpgradeButton(availableUpgrades[i]);
            }
        }

        private void CreateUpgradeButton(UpgradeOption upgrade)
        {
            if (upgradeButtonPrefab == null || upgradeButtonParent == null) return;

            GameObject buttonObj = Instantiate(upgradeButtonPrefab, upgradeButtonParent);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI[] texts = buttonObj.GetComponentsInChildren<TextMeshProUGUI>();

            if (texts.Length >= 2)
            {
                texts[0].text = upgrade.name;
                texts[1].text = upgrade.description;
            }

            if (button != null)
            {
                button.onClick.AddListener(() => SelectUpgrade(upgrade));
            }
        }

        private void SelectUpgrade(UpgradeOption upgrade)
        {
            upgrade.onSelect?.Invoke();
            CloseUpgradePanel();
        }

        public void CloseUpgradePanel()
        {
            if (upgradePanel != null)
                upgradePanel.SetActive(false);

            Time.timeScale = 1f; // Resume game
        }

        private void OnDestroy()
        {
            if (Core.GameManager.Instance != null)
            {
                Core.GameManager.Instance.OnWaveCompleted -= ShowUpgrades;
            }
        }
    }
}

