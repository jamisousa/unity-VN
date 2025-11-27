using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VN_Configuration
{
    public const bool ENCRYPTED = false;
    public static VN_Configuration activeConfig;

    public static string filePath => $"{FilePaths.root}vnconfig.cfg";

    //General settings
    public bool display_fullscreen = true;
    public string display_resolution = "1920 x 1080";
    public bool continueSkippingAfterChoice = false;
    public float dialogueTextSpeed = 1f;
    public float dialogueAutoReadSpeed = 1f;

    //Audio settings
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public float voicesVolume = 1f;
    public bool musicMute = false;
    public bool sfxMute = false;
    public bool voiceMute = false;

    //Other settings
    public float historyLogScale = 1f;

    public void Load()
    {
        var ui = ConfigMenu.instance.ui;

        //apply general settings
        ConfigMenu.instance.SetDisplayToFullscreen(display_fullscreen);
        ui.SetButtonColors(ui.fullscreen, ui.windowed, display_fullscreen);

        //screen resolution
        int res_index = 0;
        for(int i = 0; i < ui.resolutions.options.Count; i++)
        {
            string resolution = ui.resolutions.options[i].text;
            if (resolution == display_resolution)
            {
                res_index = i;
                break;
            }
        }

        ui.resolutions.value = res_index;

        ui.SetButtonColors(ui.skippingContinue, ui.skippingStop, continueSkippingAfterChoice);
        ui.architectSpeed.value = dialogueTextSpeed;
        ui.autoReaderSpeed.value = dialogueAutoReadSpeed;
    }

    public void Save()
    {
        FileManager.Save(filePath, JsonUtility.ToJson(this), encrypt: ENCRYPTED);    
    }
}
