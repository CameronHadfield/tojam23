using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AudioType // your custom enumeration
{
    BGM,
    SFX,
    Menu
};

public class AudioManager : MonoBehaviour
{
    public float defaultVolume = 0.5f;

    // Delegates that will be invoked on audio change.
    public delegate void OnBGMChange(float newAudio);
    public event OnBGMChange BGMChange;

    public delegate void OnSFXhange(float newAudio);
    public event OnSFXhange SFXChange;

    public delegate void OnMenuChange(float newAudio);
    public event OnMenuChange MenuChange;

    [Header("Sliders")]
    // Volume sliders
    [SerializeField]
    private Slider sliderBGM;

    [SerializeField]
    private Slider sliderSFX;

    [SerializeField]
    private Slider sliderMenu;

    [Header("Misc")]
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject settingsButton;

    [SerializeField]
    private bool hideUntilPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        if (hideUntilPaused)
        {
            HideButton();
            PauseMenuManager pmm = FindObjectOfType<PauseMenuManager>();
            if (pmm)
            {
                pmm.GamePaused += ShowButton;
                pmm.GamePaused += ShowPanel;
                pmm.GameResumed += HideButton;
                pmm.GameResumed += ClosePanel;
                pmm.GameResumed += HidePanel;
            }
        }

        // Set saved values on initialization
        sliderBGM.value = DataManager.audioBGM;
        sliderSFX.value = DataManager.audioSFX;
        sliderMenu.value = DataManager.audioMenu;

        StartCoroutine(LateStart(0.5f));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        BGMChange?.Invoke(sliderBGM.value);
        SFXChange?.Invoke(sliderSFX.value);
        MenuChange?.Invoke(sliderMenu.value);
    }

    public void ShowButton()
    {
        settingsButton.SetActive(true);
    }

    public void HideButton()
    {
        settingsButton.SetActive(false);
    }

    public void TogglePanel()
    {
        animator.SetBool("Opened", !animator.GetBool("Opened"));
    }

    public void ClosePanel()
    {
        animator.SetBool("Opened", false);
    }

    public void ShowPanel()
    {
        GetComponent<CanvasGroup>().interactable = true;
        GetComponent<CanvasGroup>().alpha = 1f;
    }

    public void HidePanel()
    {
        GetComponent<CanvasGroup>().interactable = false;
        GetComponent<CanvasGroup>().alpha = 0f;
    }

    public void SetBGMChanges()
    {
        BGMChange?.Invoke(sliderBGM.value);
        DataManager.audioBGM = sliderBGM.value;
    }

    public void SetSFXChanges()
    {
        SFXChange?.Invoke(sliderSFX.value);
        DataManager.audioSFX = sliderSFX.value;
    }

    public void SetMenuChanges()
    {
        MenuChange?.Invoke(sliderMenu.value);
        DataManager.audioMenu = sliderMenu.value;
    }

    public void SetAllAudioChanges()
    {
        SetBGMChanges();
        SetSFXChanges();
        SetMenuChanges();
    }
}
