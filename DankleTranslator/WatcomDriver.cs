using System;
using System.Diagnostics;

namespace DankleTranslator
{
    public class WatcomDriver(string wcc, string wdis) : IDriver
    {
        public readonly string WCC = wcc;
        public readonly string WDIS = wdis;

		public void Compile(string sourcePath, string objectFilePath)
		{
			var proc = new Process();
			proc.StartInfo.FileName = WCC;
			proc.StartInfo.Arguments = $"-0 -bt=dos -mh -s -za99 -ecc -i=../CTest -fo={objectFilePath} {sourcePath}";
			proc.StartInfo.RedirectStandardOutput = true;
			proc.Start();

			proc.WaitForExit();
            if (proc.ExitCode != 0) throw new Exception($"Failed to compile {sourcePath}");
		}

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
