using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text moveSpeedText;
    [SerializeField] private TMP_Text fireSpeedText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text bulletSpeedText;
    [SerializeField] private Image fireCooldownBar;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider waveProgressSlider;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverTimeText;
    [SerializeField] private TMP_Text gameOverScoreText;


    private int lastWaveIndex = -1;
    private float scoreAtWaveStart = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Update()
    {
        UpdateMoveSpeedDisplay();
        UpdateFireRateDisplay();
        UpdateScoreDisplay();
        UpdateFireCooldownBar();
        UpdateBulletDisplay();
        UpdateWaveDisplay();
        UpdateWaveBar();
        UpdateHealthBar();
        UpdateTimeText();

        // TODO: Gamitin yung bagong Input system ng Unity
        // if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }

    private void UpdateMoveSpeedDisplay()
    {
        if (moveSpeedText != null) moveSpeedText.text = $"Move Speed: {Player.Instance.GetMoveSpeed():F1}";
    }

    private void UpdateFireRateDisplay()
    {
        if (fireSpeedText != null) fireSpeedText.text = $"Fire Rate: {Player.Instance.GetTimeBetweenFiring():F2}s";
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null) scoreText.text = $"Score: {Player.Instance.CheckScore}";
    }

    private void UpdateBulletDisplay()
    {
        if (bulletSpeedText != null) bulletSpeedText.text = $"Bullet Speed: {Player.Instance.GetBulletSpeed()}";
    }

    private void UpdateWaveDisplay()
    {
        if (waveText != null) waveText.text = $"Wave: {EnemySpawner.Instance.GetCurrentWaveIndex() + 1}";
    }


    private void UpdateWaveBar()
    {
        if (waveProgressSlider != null)
        {
            int currentWaveIndex = EnemySpawner.Instance.GetCurrentWaveIndex();

            if (currentWaveIndex != lastWaveIndex)
            {
                lastWaveIndex = currentWaveIndex;
                scoreAtWaveStart = Player.Instance.CheckScore;
            }

            float currentScore = Player.Instance.CheckScore;
            float currentThreshold = EnemySpawner.Instance.GetCurrentWaveScoreThreshold();

            float progress = (currentScore - scoreAtWaveStart) / (currentThreshold - scoreAtWaveStart);
            waveProgressSlider.value = Mathf.Clamp01(progress);
        }
    }

    private void UpdateFireCooldownBar()
    {
        if (fireCooldownBar != null)
        {
            float cooldownProgress = Player.Instance.GetFireCooldownProgress();
            fireCooldownBar.fillAmount = cooldownProgress;
            fireCooldownBar.color = Color.Lerp(Color.red, Color.green, cooldownProgress);
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null) healthBar.value = Player.Instance.health;
    }

    public void UpdateTimeText()
    {
        if (timeText != null) timeText.text = FormatTime(Player.Instance.GetSurvivalTime());
    }

    public void TogglePause()
    {
        if (Time.timeScale == 1f) Pause();
        else Resume();
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ChangeScene(int index)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(index);
    }

    public void LevelUpPanelOpen()
    {
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void LevelUpPanelClose()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ShowGameOverPanel(float survivalTime)
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        string formattedTime = FormatTime(survivalTime);

        Debug.Log($"Game Over! Survival Time: {formattedTime} seconds");
        gameOverTimeText.text = $"Survival Time: {formattedTime}";
        gameOverScoreText.text = $"Score: {Player.Instance.CheckScore} points";
    }

    public string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
