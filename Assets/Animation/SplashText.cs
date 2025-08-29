using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class SplashText : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private TextMeshProUGUI splashText;
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private float animationDuration = 2f;
    [SerializeField] private float bounceHeight = 500f;
    [SerializeField] private float finalBounceScale = 1.2f;

    [Header("Subtitle Settings")]
    [SerializeField] private float subtitleDelay = 1f;

    [Header("Retro Effects")]
    [SerializeField] private bool usePixelPerfect = true;
    [SerializeField] private AudioClip splashSound;
    [SerializeField] private AudioClip bounceSound;

    [Header("Scene Transition")]
    [SerializeField] private float holdDuration = 1.5f;
    [SerializeField] private string nextSceneName = "MainMenu";

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private Color originalColor;

    void Start()
    {
        InitializeSplashScreen();
        PlayRetroSplashAnimation();
    }

    void PlayRetroSubtitlePopup()
    {
        Sequence subtitleSequence = DOTween.Sequence();

        if (subtitleText == null) return;
        subtitleSequence.Append(
            subtitleText.DOFade(1f, 0.5f)
                .SetDelay(subtitleDelay)
                .OnStart(() => AudioManager.Instance.PlaySFX(splashSound))
);
    }

    void InitializeSplashScreen()
    {
        if (splashText == null)
            splashText = GetComponent<TextMeshProUGUI>();

        originalPosition = splashText.transform.position;
        originalScale = splashText.transform.localScale;
        originalColor = splashText.color;

        Vector3 startPos = originalPosition;
        startPos.y += bounceHeight;
        splashText.transform.position = startPos;

        splashText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        splashText.transform.localScale = originalScale * 0.8f;

        subtitleText.alpha = 0f;

        if (usePixelPerfect && splashText.canvas != null) splashText.canvas.pixelPerfect = true;
    }

    void PlayRetroSplashAnimation()
    {
        Sequence splashSequence = DOTween.Sequence();

        splashSequence.Append(splashText.DOFade(1f, 0.1f));

        splashSequence.Append(
            splashText.transform.DOMoveY(originalPosition.y, animationDuration)
                .SetEase(Ease.OutBounce)
                .OnUpdate(() => {
                    if (usePixelPerfect)
                    {
                        Vector3 pos = splashText.transform.position;
                        pos.x = Mathf.Round(pos.x);
                        pos.y = Mathf.Round(pos.y);
                        splashText.transform.position = pos;
                    }
                })
        );

        splashSequence.Join(
            splashText.transform.DOScale(finalBounceScale, animationDuration * 0.3f)
                .OnPlay(() => AudioManager.Instance.PlaySFX(bounceSound))
                .SetEase(Ease.OutBack)
                .SetDelay(animationDuration * 0.7f)
        );

        splashSequence.Append(
            splashText.transform.DOScale(originalScale, animationDuration * 0.2f)
                .SetEase(Ease.InBack)
        );

        if (subtitleText != null)
        {
            splashSequence.AppendCallback(() => PlayRetroSubtitlePopup());
            splashSequence.AppendInterval(subtitleDelay);
        }

        splashSequence.AppendInterval(holdDuration);

        splashSequence.Append(splashText.DOFade(0f, 0.5f));

        if (subtitleText != null) splashSequence.Join(subtitleText.DOFade(0f, 0.5f));

        splashSequence.AppendInterval(holdDuration);

        splashSequence.OnComplete(() => {
            LoadNextScene();
        });
    }


    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName)) UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        else Debug.Log("Splash screen animation completed!");
    }

    void OnDestroy()
    {
        DOTween.Kill(splashText.transform);
        if (subtitleText != null)
            DOTween.Kill(subtitleText.transform);
    }
}