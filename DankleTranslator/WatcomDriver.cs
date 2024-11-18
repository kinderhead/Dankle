using System;
using System.Diagnostics;

namespace DankleTranslator
{
    public class WatcomDriver(string wdis) : IDriver
    {
        public readonly string WDIS = wdis;

        public string Dissassemble(string objectFilePath)
        {
            var proc = new Process();
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.FileName = WDIS;
            proc.StartInfo.Arguments = $"{objectFilePath} -fp -fi -a";
            proc.Start();

            var output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            return output;
        }
    }
}
