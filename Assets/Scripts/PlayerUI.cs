using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text moveSpeedText;
    [SerializeField] private TMP_Text fireSpeedText;
    [SerializeField] private TMP_Text canFireText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Image fireCooldownBar;

    private void Update()
    {
        if (Player.Instance == null) return;

        UpdateMoveSpeedDisplay();
        UpdateFireRateDisplay();
        UpdateCanFireDisplay();
        UpdateScoreDisplay();
        UpdateFireCooldownBar();
    }

    private void UpdateMoveSpeedDisplay()
    {
        if (moveSpeedText != null)
        {
            moveSpeedText.text = $"Move Speed: {Player.Instance.GetMoveSpeed():F1}";
        }
    }

    private void UpdateFireRateDisplay()
    {
        if (fireSpeedText != null)
        {
            fireSpeedText.text = $"Fire Rate: {Player.Instance.GetTimeBetweenFiring():F2}s";
        }
    }

    private void UpdateCanFireDisplay()
    {
        if (canFireText != null)
        {
            canFireText.text = $"Can Fire: {(Player.Instance.CanFire() ? "YES" : "NO")}";
            canFireText.color = Player.Instance.CanFire() ? Color.green : Color.red;
        }
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {Player.Instance.CheckScore}";
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
}
