using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text fireSpeedText;
    [SerializeField] private TMP_Text bulletSpeedText;
    [SerializeField] private TMP_Text moveSpeedText;
    // [SerializeField] private Image fireCooldownBar;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Slider healthBar;
    // [SerializeField] private Slider waveProgressSlider;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverTimeText;
    [SerializeField] private TMP_Text gameOverScoreText;
    [SerializeField] private RectTransform howToRect;
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private CanvasGroup gameHUD;

    [Header("Audio")]
    [SerializeField] private AudioClip gameBG;
    [SerializeField] private AudioClip shootSFX;
    [SerializeField] private AudioClip enemyDieSFX;
    [SerializeField] private AudioClip damagePlayerSFX;
    [SerializeField] private AudioClip levelUpSFX;
    [SerializeField] private AudioClip levelUpCloseSFX;
    [SerializeField] private AudioClip gameOverSFX;
    [SerializeField] private AudioClip healthPickupSFX;

    RectTransform levelUpRect;

    // private int lastWaveIndex = -1;
    // private float scoreAtWaveStart = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        levelUpRect = levelUpPanel.GetComponent<RectTransform>();
    }

    private void Start()
    {
        EnemySpawner.Instance.gameObject.SetActive(false);

        if (gameHUD != null)
        {
            gameHUD.gameObject.SetActive(false);
            gameHUD.alpha = 0f;
        }

        if (gameBG != null)
        {
            background.gameObject.SetActive(false);
            Color color = background.color;
            color.a = 0f;
        }

        // Disable shooting
        Player.Instance.DisableShooting();

        UpdateHealthBar();
    }

    //TODO: Might refactor later so the functions would not be called every update (called only when necessary) 
    private void Update()
    {
        UpdateScoreDisplay();
        // UpdateWaveDisplay();
        // UpdateWaveBar();
        UpdateDamageDisplay();
        UpdateFireRateDisplay();
        UpdateMoveSpeedDisplay();
        // UpdateFireCooldownBar();
        UpdateBulletDisplay();
        // UpdateHealthBar();
        UpdateTimeText();

        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
    }

    public void UpdateWaveDisplay()
    {
        if (waveText != null)
        {
            waveText.text = $"Wave: {EnemySpawner.Instance.GetCurrentWaveIndex() + 1}";
            waveText.DOFade(0.1f, 0.1f)
                .SetLoops(8, LoopType.Yoyo)
                .OnComplete(() => waveText.DOFade(1f, 0.1f));
        }
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null) scoreText.text = $"{Player.Instance.CheckScore}";
    }

    private void UpdateDamageDisplay()
    {
        if (damageText != null) damageText.text = $"Damage: {Player.Instance.GetBulletDamage()}";
    }

    private void UpdateFireRateDisplay()
    {
        if (fireSpeedText != null) fireSpeedText.text = $"Fire Rate: {Player.Instance.GetTimeBetweenFiring():F2}s";
    }

    private void UpdateBulletDisplay()
    {
        if (bulletSpeedText != null) bulletSpeedText.text = $"Bullet Speed: {Player.Instance.GetBulletSpeed()}";
    }

    private void UpdateMoveSpeedDisplay()
    {
        if (moveSpeedText != null) moveSpeedText.text = $"Move Speed: {Player.Instance.GetMoveSpeed():F1}";
    }


    // private void UpdateWaveBar()
    // {
    //     if (waveProgressSlider != null)
    //     {
    //         int currentWaveIndex = EnemySpawner.Instance.GetCurrentWaveIndex();

    //         if (currentWaveIndex != lastWaveIndex)
    //         {
    //             lastWaveIndex = currentWaveIndex;
    //             scoreAtWaveStart = Player.Instance.CheckScore;
    //         }

    //         float currentScore = Player.Instance.CheckScore;
    //         float currentThreshold = EnemySpawner.Instance.GetCurrentWaveScoreThreshold();

    //         float progress = (currentScore - scoreAtWaveStart) / (currentThreshold - scoreAtWaveStart);
    //         waveProgressSlider.value = Mathf.Clamp01(progress);
    //     }
    // }

    // private void UpdateFireCooldownBar()
    // {
    //     if (fireCooldownBar != null)
    //     {
    //         float cooldownProgress = Player.Instance.GetFireCooldownProgress();
    //         fireCooldownBar.fillAmount = cooldownProgress;
    //         fireCooldownBar.color = Color.Lerp(Color.red, Color.green, cooldownProgress);
    //     }
    // }

    public void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.DOValue(Player.Instance.health, 0.3f)
                .SetEase(Ease.OutExpo);
        }
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

    public void CloseHowToPanel()
    {
        AudioManager.Instance.PlaySFX(levelUpCloseSFX);

        // Turn on enemy spawning
        EnemySpawner.Instance.gameObject.SetActive(true);

        ShowGameUI();
        AudioManager.Instance.PlayMusic(gameBG);

        // Enable player shooting
        Player.Instance.EnableShooting();
        Player.Instance.recordTime = true;

        Time.timeScale = 1f;

        howToRect.DOScaleY(0f, 0.5f)
        .From(1f)
        .SetUpdate(true)
        .SetEase(Ease.OutQuart)
        .OnComplete(() => howToRect.gameObject.SetActive(false));
    }

    public void LevelUpPanelOpen()
    {
        AudioManager.Instance.PlaySFX(levelUpSFX);

        levelUpRect.localScale = new Vector3(1f, 0f, 1f);
        levelUpPanel.SetActive(true);

        Button[] buttons = levelUpPanel.GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons) btn.interactable = false;

        levelUpRect.DOScaleY(1f, 0.5f)
            .From(0f)
            .SetUpdate(true)
            .SetEase(Ease.OutQuart);

        Time.timeScale = 0f;
        foreach (Button btn in buttons) btn.interactable = true;

    }

    public void ShowGameUI()
    {
        Sequence uiSequence = DOTween.Sequence().SetUpdate(true);
        
        gameHUD.gameObject.SetActive(true);
        uiSequence.Join(gameHUD.DOFade(1f, 5f).SetEase(Ease.OutQuart));
        
        background.gameObject.SetActive(true);
        uiSequence.Join(background.DOFade(1f, 2f).SetEase(Ease.OutQuart));
    }

    public void LevelUpPanelClose()
    {
        AudioManager.Instance.PlaySFX(levelUpCloseSFX);

        Button[] buttons = levelUpPanel.GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons) btn.interactable = false;

        levelUpRect.DOScaleY(0f, 0.5f)
            .From(1f)
            .SetUpdate(true)
            .SetEase(Ease.OutQuart)
            .OnComplete(() => levelUpPanel.SetActive(false));

        Time.timeScale = 1f;
    }

    public void ShowGameOverPanel(float survivalTime)
    {
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.sfxSource.clip = gameOverSFX;
        AudioManager.Instance.sfxSource.PlayDelayed(1f);

        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        string formattedTime = FormatTime(survivalTime);

        Debug.Log($"Game Over! Survival Time: {formattedTime} seconds");
        gameOverTimeText.text = $"Survival Time: {formattedTime}";
        gameOverScoreText.text = $"Score: {Player.Instance.CheckScore} points";
    }

    public void PlayerShootSFX() => AudioManager.Instance.PlaySFX(shootSFX);
    public void EnemyDieSFX() => AudioManager.Instance.PlaySFX(enemyDieSFX);
    public void DamagePlayerSFX() => AudioManager.Instance.PlaySFX(damagePlayerSFX);
    public void HealthPickupSFX() => AudioManager.Instance.PlaySFX(healthPickupSFX);

    public string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
