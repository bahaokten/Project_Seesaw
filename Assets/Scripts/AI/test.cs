using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class test : MonoBehaviour
{
    void Start()
    {
        print(Application.persistentDataPath);
        string[] output = PythonExecutor.RunExecutor(Directory.GetCurrentDirectory() + "\\Assets\\Scripts\\AI\\Python\\BasicLearner.py", Application.persistentDataPath);
        foreach (string s in output)
            print(s);
    }

}
