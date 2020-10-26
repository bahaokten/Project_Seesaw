using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    public static string GetPersistentPath(string _path)
    {
        if (_path[0] != '\\')
        {
            return Application.persistentDataPath + '\\' + _path;
        }
        else
        {
            return Application.persistentDataPath + _path;
        }
    }

    public static void CreateDir(string _path)
    {
        string path = GetPersistentPath(_path);
        
        if (!Directory.Exists(path))
        {
            try
            {
                DirectoryInfo di = Directory.CreateDirectory(path);
                print("The directory was created successfully at " + Directory.GetCreationTime(path));
            }
            catch (Exception e)
            {
                print("The process failed: " + e.ToString());
            }
        }
    }

    public static void DeletePath(string _path)
    {
        string path = GetPersistentPath(_path);

        if (File.Exists(path))
        {
            File.Delete(path);
        } 
        else if (Directory.Exists(path))
        {
            Directory.Delete(path);
        }
    }

    public static void WriteToFile(string _path, List<string[]> _content, string seperator)
    {
        List<string> content = new List<string>();

        foreach (string [] sArray in _content)
        {
            string processed = "";
            foreach (string s in sArray)
            {
                processed += s + seperator;
            }

            processed = processed.Remove(processed.Length - 1, 1) + "\n";
            content.Add(processed);
        }

        WriteToFile(_path, content);
    }

    public static void WriteToFile(string _path, List<string> content)
    {
        string path = GetPersistentPath(_path);
        
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        File.WriteAllLines(path, content, Encoding.UTF8);

        print("Successfully wrote to file at " + path);
    }
}
