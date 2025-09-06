using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class FilePaths
{
    public static readonly string root = $"{Application.dataPath}/gameData/";

    // To log the value of 'root', use a method or constructor, not directly in the class body.
    // Example: static constructor
    static FilePaths()
    {
        Debug.Log(root);
    }
}
