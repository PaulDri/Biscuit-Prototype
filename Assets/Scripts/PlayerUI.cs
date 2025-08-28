using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text moveSpeedText;
    [SerializeField] private TMP_Text fireSpeedText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private Image fireCooldownBar;
    // [SerializeField] private Image waveProgressBar;
    [SerializeField] private Slider waveProgressSlider;
    [SerializeField] private GameObject pauseMenuUI;

    private int lastWaveIndex = -1;
    private float scoreAtWaveStart = 0f;

    private void Update()
    {
        if (Player.Instance == null) return;

        UpdateMoveSpeedDisplay();
        UpdateFireRateDisplay();
        UpdateScoreDisplay();
        UpdateFireCooldownBar();
        UpdateWaveDisplay();
        UpdateWaveBar();

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


    public void TogglePause()
    {
        if (Time.timeScale == 1) Pause();
        else Resume();
    }

     public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
    }
    
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
    }
    
    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
