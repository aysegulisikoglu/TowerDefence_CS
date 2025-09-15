using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CaseUI : MonoBehaviour
{
    public static CaseUI Instance;

    [Header("HUD")]
    public TMP_Text waveText;
    public TMP_Text livesText;
    public TMP_Text resourcesText;
    public Slider playerHpSlider;
    public TMP_Text playerHpText;

    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    void Awake() { Instance = this; }

    void OnEnable()
    {
        TD.Spawner.OnWaveChanged += OnWaveChanged;
        TD.GameManager.OnLivesChanged += OnLivesChanged;
        TD.GameManager.OnResourcesChanged += OnResourcesChanged;
    }
    void OnDisable()
    {
        TD.Spawner.OnWaveChanged -= OnWaveChanged;
        TD.GameManager.OnLivesChanged -= OnLivesChanged;
        TD.GameManager.OnResourcesChanged -= OnResourcesChanged;
    }

    void OnWaveChanged(int wave) => waveText.text = $"Wave: {wave + 1}";
    void OnLivesChanged(int lives)
    {
        livesText.text = $"Lives: {lives}";
        if (lives <= 0) ShowGameOver();
    }
    void OnResourcesChanged(int res) => resourcesText.text = $"Resources: {res}";

    public void UpdatePlayerHP(float hp, float max)
    {
        if (playerHpSlider)
        {
            playerHpSlider.maxValue = max;
            playerHpSlider.value = Mathf.Max(0, hp);
        }
        if (playerHpText) playerHpText.text = $"HP: {Mathf.CeilToInt(hp)}/{Mathf.CeilToInt(max)}";
    }

    public void TogglePause()
    {
        bool open = !pausePanel.activeSelf;
        pausePanel.SetActive(open);
        TD.GameManager.Instance.SetTimeScale(open ? 0f : TD.GameManager.Instance.GameSpeed);
    }

    public void SetSpeed(float s)
    {
        TD.GameManager.Instance.SetGameSpeed(s);
    }

    public void ShowGameOver()
    {
        TD.GameManager.Instance.SetTimeScale(0f);
        if (gameOverPanel) gameOverPanel.SetActive(true);
    }
}
