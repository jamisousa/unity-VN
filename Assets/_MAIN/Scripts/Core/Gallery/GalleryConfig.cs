using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GalleryConfig 
{
    public static GalleryConfig activeConfig;
    public const bool ENCRYPTED = false;

    public static string filePath => $"{FilePaths.root}gallery.vng";
    public List<string> unlockedImages = new List<string>();


    public static void Load()
    {
        if (File.Exists(filePath))
        {
            activeConfig = FileManager.Load<GalleryConfig>(filePath, encrypt: ENCRYPTED);
        }
        else
        {
            activeConfig = new GalleryConfig();
        }
    }

    public static void Save() => FileManager.Save(filePath, JsonUtility.ToJson(activeConfig), encrypt: ENCRYPTED);

    public static void Erase()
    {
       if(activeConfig == null)
        {
            activeConfig = new GalleryConfig();
        }

        activeConfig.unlockedImages = new List<string>();

        Save();
    }

    public static void UnlockImage(string imageName)
    {
        if(activeConfig == null)
        {
            Load();
        }

        if(!activeConfig.unlockedImages.Contains(imageName))
        {
            activeConfig.unlockedImages.Add(imageName);
            Save();
        }
    }

    public static bool IsImageUnlocked(string imageName)
    {
        if(activeConfig == null)
        {
            Load();
        }
        return activeConfig.unlockedImages.Contains(imageName);
    }

}
