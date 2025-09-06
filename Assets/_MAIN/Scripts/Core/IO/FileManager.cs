using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileManager
{

    //This function will read a text file from the specified path
    public static List<string> ReadTextFile(string path, bool includeBlankLines = true)
    {

        if (!path.StartsWith("/"))
        {
            path = FilePaths.root + path;
        }

        List<string> lines = new List<string>();

        try
        {
            using (StreamReader sr = new StreamReader(path)) {
            
                while(!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (includeBlankLines || !string.IsNullOrWhiteSpace(line))
                    {
                        lines.Add(line);
                    }
                }

            }
        }catch(FileNotFoundException ex)
        {
            Debug.LogError($"File not found: {path}\n{ex}");
        }

        return lines;
    }

    //This function will read a TextAsset from the Resources folder
    public static List<string> ReadTextAsset(string path, bool includeBlankLines = true)
    {
        TextAsset asset = Resources.Load<TextAsset>(path);

        if (asset == null)
        {
           Debug.LogError($"TextAsset not found at {path}");
            return null;
        }

        return ReadTextAsset(asset, includeBlankLines);

    }

    public static List<string> ReadTextAsset(TextAsset asset, bool includeBlankLines = true)
    {

        List<string> lines = new List<string>();

        using (StringReader sr = new StringReader(asset.text))
        {
            //this lets us peek at the next character without consuming it
            while (sr.Peek() > -1)
            {
                string line = sr.ReadLine();
                if (includeBlankLines || !string.IsNullOrWhiteSpace(line))
                {
                    lines.Add(line);
                }
            }
        }

        return lines;
    }

}
