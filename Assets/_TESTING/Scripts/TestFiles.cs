using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFiles : MonoBehaviour
{

    //private string fileName = "testFile.txt";
    [SerializeField] private TextAsset fileName;

    void Start()
    {
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        //List<string> lines = FileManager.ReadTextFile(fileName, false);

        List<string> lines = FileManager.ReadTextAsset(fileName, false);


        if (lines == null)
        {
            Debug.LogError($"File {fileName} not found or reading error");
            yield break;
        }

        foreach (string line in lines)
        {
            //Debug.Log(line);
        }

        yield return null;
    }

}
