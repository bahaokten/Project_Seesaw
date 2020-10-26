using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMLAI : BaseAI
{
    protected static readonly string AI_PYTHON_LOC = "\\Assets\\Scripts\\AI\\Python\\";

    protected string pythonProgName = "";
    protected string pythonIn = "";
    
    //Data to train AI on
    protected List<string[]> MLData;

    protected string[] RunMLModel()
    {
        FileManager.WriteToFile(pythonIn, MLData, ",");
        string[] output = PythonExecutor.RunExecutor(AI_PYTHON_LOC + pythonProgName, Application.persistentDataPath + pythonIn);
        return output;
    }

    protected override void Initialize()
    {
        MLData = new List<string[]>();
        pythonIn = player.ToString() + "MLAI.txt";

        print(player.ToString());
    }
}
