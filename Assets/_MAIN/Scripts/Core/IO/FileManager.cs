using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class FileManager
{
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

    public static bool TryCreateDirectoryFromPath(string path)
    {
        if (Directory.Exists(path) || File.Exists(path))
        {
            return true;
        }

        if (path.Contains("."))
        {
            path = Path.GetDirectoryName(path);
            if (Directory.Exists(path)) {
                return true;
            }
        }

        if(path == string.Empty)
        {
            return false;
        }

        try
        {
            Directory.CreateDirectory(path);
            return true;
        }
        catch(System.Exception e)
        {
            Debug.Log($"Could not create directory {path} - {e}");
            return false;
        }
    }

    public static void Save(string filePath, string JSONData, bool encrypt = false)
    {
        if (!TryCreateDirectoryFromPath(filePath))
        {
            Debug.LogError($"Failed to save file {filePath}");
            return;
        }

        if (encrypt)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(JSONData);
            byte[] keyBytes = GenerateKey();
            byte[] encryptedBytes = XOR(dataBytes, keyBytes);

            File.WriteAllBytes(filePath, encryptedBytes);
        }
        else
        {
            StreamWriter sw = new StreamWriter(filePath);
            sw.Write(JSONData);
            sw.Close();
        }
    }

    public static T Load<T>(string filePath, bool encrypt = false)
    {
        if (File.Exists(filePath))
        {
            if (encrypt)
            {
                byte[] encryptedBytes = File.ReadAllBytes(filePath);
                byte[] keyBytes = GenerateKey();
                byte[] decryptedBytes = XOR(encryptedBytes, keyBytes);

                string decryptedString = Encoding.UTF8.GetString(decryptedBytes);

                return JsonUtility.FromJson<T>(decryptedString);
            }
            else
            {
                string JSONData = File.ReadAllLines(filePath)[0];
                return JsonUtility.FromJson<T>(JSONData);
            }

        }
        else
        {
            Debug.LogError($"Failed to load file {filePath} - file does not exist!");
            return default(T);
        }
    }

    private static byte[] XOR(byte[] input, byte[] key)
    {
        byte[] output = new byte[input.Length];

        for (int i = 0; i < input.Length; i++)
        {

            output[i] = (byte)(input[i] ^ key[i % key.Length]);

        }

        return output;
    }

    private static byte[] GenerateKey()
    {
        string seed = SystemInfo.deviceUniqueIdentifier;

        using (SHA256 sha = SHA256.Create())
        {
            return sha.ComputeHash(Encoding.UTF8.GetBytes(seed));
        }
    }


}
