using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PlayButton : MonoBehaviour
{
    Button playButton;
    CanvasGroup buttonCanvas;
    public CanvasGroup screenFader;
    public AudioClip playSound;
    public string nextSceneName = "GameScene";

    void Start()
    {
        playButton = GetComponent<Button>();
        buttonCanvas = GetComponent<CanvasGroup>();
        playButton.onClick.AddListener(OnPlayClicked);
    }

    void OnPlayClicked()
    {
        playButton.interactable = false;

        Sequence seq = DOTween.Sequence().SetUpdate(true);

        seq.Append(
            buttonCanvas.DOFade(0f, 0.1f)
            .OnStart(() => AudioManager.Instance.PlaySFX(playSound))
        );
        
        seq.Append(buttonCanvas.DOFade(1f, 0.1f));
        seq.Append(buttonCanvas.DOFade(0f, 0.1f));
        seq.Append(buttonCanvas.DOFade(1f, 0.1f));

        seq.Append(screenFader.DOFade(1f, 1.5f));

        seq.AppendInterval(1f);
        seq.OnComplete(() => SceneManager.LoadScene(nextSceneName));
    }
}
