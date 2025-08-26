using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public AudioClip menuMusic;

    [Header("User Interface")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle muteToggle;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(menuMusic);

        musicSlider.value = AudioManager.Instance.GetMusicVolume();
        sfxSlider.value = AudioManager.Instance.GetSFXVolume();
        muteToggle.isOn = AudioManager.Instance.IsMusicMuted();

        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
        muteToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    public void ChangeScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    private void OnMusicSliderChanged(float value)
    {
        if (muteToggle.isOn && value > 0.001f) muteToggle.isOn = false;
        AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnSFXSliderChanged(float value)
    {
        if (muteToggle.isOn && value > 0.001f) muteToggle.isOn = false;
        AudioManager.Instance.SetSFXVolume(value);
    }

    private void OnToggleChanged(bool isMuted)
    {
        AudioManager.Instance.ToggleMute(isMuted);

        if (isMuted)
        {
            musicSlider.SetValueWithoutNotify(0f);
            sfxSlider.SetValueWithoutNotify(0f);
        }
        else
        {
            musicSlider.SetValueWithoutNotify(AudioManager.Instance.GetMusicVolume());
            sfxSlider.SetValueWithoutNotify(AudioManager.Instance.GetSFXVolume());
        }
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
