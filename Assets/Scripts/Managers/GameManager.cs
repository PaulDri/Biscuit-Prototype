using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public AudioClip menuMusic;

    [Header("User Interface")]
    public Slider musicSlider;
    public Toggle muteToggle;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(menuMusic);

        musicSlider.value = AudioManager.Instance.GetMusicVolume();
        muteToggle.isOn = AudioManager.Instance.IsMusicMuted();

        musicSlider.onValueChanged.AddListener(OnSliderChanged);
        muteToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    public void ChangeScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    private void OnSliderChanged(float value)
    {
        if (muteToggle.isOn && value > 0.001f) muteToggle.isOn = false;

        AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnToggleChanged(bool isMuted)
    {
        AudioManager.Instance.ToggleMute(isMuted);

        if (isMuted) musicSlider.SetValueWithoutNotify(0f);
        else musicSlider.SetValueWithoutNotify(AudioManager.Instance.GetMusicVolume());
    }
    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
