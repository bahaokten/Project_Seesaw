using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

public class PythonExecutor
{
    public static string[] RunExecutor(string progToRun, string inFile, string outFile = null)
    {
        progToRun = Directory.GetCurrentDirectory() + progToRun;

        char[] splitter = { '\n' };

        Process proc = new Process();
        proc.StartInfo.FileName = GlobalVars.PYTHON_LOC;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.CreateNoWindow = false;

        proc.StartInfo.Arguments = string.Concat(progToRun, " ", inFile, " ", outFile);
        proc.Start();
        StreamReader sReader = proc.StandardOutput;
        string[] output = sReader.ReadToEnd().Split(splitter);

        proc.WaitForExit();
        return output;
    }
}
