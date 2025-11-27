using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DIALOGUE;
using DIALOGUE.LogicalLines;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ConfigMenu : MenuPage
{
    public static ConfigMenu instance { get; private set; }

    [SerializeField] private GameObject[] panels;
    public UI_ITEMS ui;
    private GameObject activePanel;

    private VN_Configuration config => VN_Configuration.activeConfig;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        for(int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == 0);
        }

        activePanel = panels[0];

        SetAvailableResolutions();
        LoadConfig();
    }

    private void LoadConfig()
    {
        if(File.Exists(VN_Configuration.filePath))
        {
            VN_Configuration.activeConfig = FileManager.Load<VN_Configuration>(VN_Configuration.filePath, encrypt: VN_Configuration.ENCRYPTED);
        }
        else
        {
            VN_Configuration.activeConfig = new VN_Configuration();
        }

        VN_Configuration.activeConfig.Load();
    }

    private void OnApplicationQuit()
    {
        VN_Configuration.activeConfig.Save();
        VN_Configuration.activeConfig = null;
    }

    public void OpenPanel(string panelName)
    {
        Debug.Log("Opening panel: " + panelName);

        GameObject panel = panels.First(p => p.name.ToLower() == panelName.ToLower());

        if(panel == null)
        {
            Debug.LogWarning($"Could not find panel called {panelName} in config menu.");
            return;
        }

        if(activePanel != null && activePanel != panel)
        {
            activePanel.SetActive(false);
        }

        panel.SetActive(true);
        activePanel = panel;
    }

    private void SetAvailableResolutions()
    {
        Resolution[] resolutions = Screen.resolutions;
        List<string> options = new List<string>();

        for(int i = resolutions.Length - 1; i >= 0 ; i--)
        {
            options.Add($"{resolutions[i].width} x {resolutions[i].height}");
        }

        ui.resolutions.ClearOptions();
        ui.resolutions.AddOptions(options);
    }

    [System.Serializable]
    public class UI_ITEMS
    {
        [Header("Button Colors")]
        [SerializeField] private Color button_selectedColor = new Color(0.45f, 0f, 0f, 1f);
        [SerializeField] private Color button_unselectedColor = Color.white;

        [Header("Text Colors")]
        [SerializeField] private Color text_selectedColor = Color.white;
        [SerializeField] private Color text_unselectedColor = Color.white;

        [Header("General")]
        public Button fullscreen;
        public Button windowed;
        public TMP_Dropdown resolutions;
        public Button skippingContinue, skippingStop;
        public Slider architectSpeed, autoReaderSpeed;

        [Header("Audio")]
        public Slider musicVolume;
        public Slider sfxVolume;
        public Slider voiceVolume;
        public Color musicOnColor = new Color(0.45f, 0f, 0f, 1f);
        public Color musicOffColor = Color.white;
        public Sprite mutedSymbol;
        public Sprite unmutedSymbol;
        public Image musicMute;
        public Image sfxMute;
        public Image voiceMute;
        public Image musicFill;
        public Image sfxFill;
        public Image voicesFill;

        public void SetButtonColors(Button A, Button B, bool selectedA)
        {
            A.image.color = selectedA ? button_selectedColor : button_unselectedColor;
            B.image.color = selectedA ? button_unselectedColor : button_selectedColor;

            A.GetComponentInChildren<TextMeshProUGUI>().color = selectedA ? text_selectedColor : text_unselectedColor;
            B.GetComponentInChildren<TextMeshProUGUI>().color = selectedA ? text_unselectedColor : text_selectedColor;
        }
    }


    //UI callable functions
    public void SetDisplayToFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        ui.SetButtonColors(ui.fullscreen, ui.windowed, fullscreen);
    }

    public void SetDisplayResolution()
    {
        string resolution = ui.resolutions.captionText.text;
        string[] values = resolution.Split('x');

        if (int.TryParse(values[0], out int width) && int.TryParse(values[1], out int height))
        {
            Screen.SetResolution(width, height, Screen.fullScreen);
            config.display_resolution = resolution;
        }
        else
        {
            Debug.LogError($"Could not parse resolution values from {resolution}");
        }
    }

    public void SetContinueSkippingAfterChoice(bool continueSkipping)
    {
        config.continueSkippingAfterChoice = continueSkipping;
        ui.SetButtonColors(ui.skippingContinue,  ui.skippingStop, continueSkipping);
    }

    public void SetTextArchitectSpeed()
    {
        config.dialogueTextSpeed = ui.architectSpeed.value;

        if (DialogueSystem.instance != null) {
            DialogueSystem.instance.conversationManager.architect.speed = config.dialogueTextSpeed;
        }
    }
    public void SetAutoReaderSpeed()
    {
        config.dialogueAutoReadSpeed = ui.autoReaderSpeed.value;


        if (DialogueSystem.instance == null)
        {
            return;
        }

        AutoReader autoReader = DialogueSystem.instance.autoReader;

        if(autoReader != null)
        {
            autoReader.speed = config.dialogueAutoReadSpeed;
        }
    }

    public void SetMusicVolume()
    {
        config.musicVolume = ui.musicVolume.value;

        AudioManager.instance.SetMusicVolume(config.musicVolume, config.musicMute);

        ui.musicFill.color = config.musicMute ? ui.musicOffColor : ui.musicOnColor;

    }
    public void SetSFXVolume()
    {
        config.sfxVolume = ui.sfxVolume.value;

        AudioManager.instance.SetSFXVolume(config.sfxVolume, config.sfxMute);

        ui.sfxFill.color = config.sfxMute ? ui.musicOffColor : ui.musicOnColor;
    }
    public void SetVoicesVolume()
    {
        config.voicesVolume = ui.voiceVolume.value;

        AudioManager.instance.SetVoicesVolume(config.voicesVolume, config.voiceMute);

        ui.voicesFill.color = config.voiceMute ? ui.musicOffColor : ui.musicOnColor;
    }

    public void SetMusicMute()
    {
        config.musicMute = !config.musicMute;
        ui.musicVolume.fillRect.GetComponent<Image>().color = config.musicMute ? ui.musicOffColor : ui.musicOnColor;
        ui.musicMute.sprite = config.musicMute ? ui.mutedSymbol : ui.unmutedSymbol;

        AudioManager.instance.SetMusicVolume(config.musicVolume, config.musicMute);
    }

    public void SetSFXMute()
    {
        config.sfxMute = !config.sfxMute;
        ui.sfxVolume.fillRect.GetComponent<Image>().color = config.sfxMute ? ui.musicOffColor : ui.musicOnColor;
        ui.sfxMute.sprite = config.sfxMute ? ui.mutedSymbol : ui.unmutedSymbol;

        AudioManager.instance.SetSFXVolume(config.sfxVolume, config.sfxMute);

    }

    public void SetVoiceMute()
    {
        config.voiceMute = !config.voiceMute;
        ui.voiceVolume.fillRect.GetComponent<Image>().color = config.voiceMute ? ui.musicOffColor : ui.musicOnColor;
        ui.voiceMute.sprite = config.voiceMute ? ui.mutedSymbol : ui.unmutedSymbol;

        AudioManager.instance.SetVoicesVolume(config.voicesVolume, config.voiceMute);
    }
}
