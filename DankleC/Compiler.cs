using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Antlr4.Runtime;
using Dankle;
using DankleC.ASTObjects;
using DankleC.IR;
using ShellProgressBar;

namespace DankleC
{
    public class Compiler
    {
        public ICharStream? Stream { get; private set; }
        private ProgramNode? _AST;
        public ProgramNode AST { get => _AST ?? throw new InvalidOperationException(); }
        public IRBuilder? IR { get; private set; }

        public Compiler ReadFileAndPreprocess(string path, ProgressBar? pb = null, params string[] preprocessArgs)
        {
            if (pb is not null) pb.Message = $"Compiling {Path.GetRelativePath(Environment.CurrentDirectory, path)}...";
            return ReadText(Preprocess(path, preprocessArgs));
        }
        public Compiler ReadFile(string path)
        {
            Stream = new AntlrFileStream(path);
            return this;
        }

        public Compiler ReadTextAndPreprocess(string program)
        {
            var filename = $"tmp_{Guid.NewGuid()}.c";
            File.WriteAllText(filename, program);

            try
            {
                var newProg = Preprocess(filename);
                return ReadText(newProg);
            }
            finally
            {
                File.Delete(filename);
            }
        }

        public Compiler ReadText(string program)
        {
            Stream = new AntlrInputStream(program);
            return this;
        }

        public Compiler GenAST(ProgramNode.Settings? settings = null)
        {
            settings ??= new();

            var lexer = new CLexer(Stream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new CParser(tokenStream);
            var visitor = new DankleCVisitor();
            _AST = (ProgramNode)visitor.Visit(parser.root());
            _AST.Optimize(settings.Value);
            return this;
        }

        public Compiler GenIR(bool debug = false)
        {
            IR = new(AST, debug);
            IR.Build();
            return this;
        }

        public string GenAssembly(ProgressBar? pb = null)
        {
            var res = new CodeGen(IR ?? throw new InvalidOperationException()).Compile(null, pb);
            pb?.Tick();
            return res;
        }

        public string Compile(ProgressBar? pb = null) => GenAST().GenIR().GenAssembly(pb);

        public static Dictionary<string, string> CompileLibC(ProgressBar? pb = null)
        {
            var libc = new Dictionary<string, string>();

            foreach (var i in LibC)
            {
                var file = Path.Join(GetExecutableFolder(), "libc", "src", i);
                libc[file] = new Compiler().ReadFileAndPreprocess(file, pb).Compile(pb);
            }

            return libc;
        }

        public static Dictionary<string, string> CompileAll(List<string> paths, ProgressBar? pb = null)
        {
            var comp = new Dictionary<string, string>();

            foreach (var i in paths)
            {
                comp[i] = new Compiler().ReadFileAndPreprocess(i, pb).Compile(pb);
            }

            return comp;
        }

        public static string Preprocess(string filepath, params string[] args)
        {
            var output = ExecuteProgram(GetPreprocessorPath(), $"{filepath.Replace("\\", "/")} -I\"{Path.Join(GetExecutableFolder(), "libc", "include").Replace("\\", "/")}\" {string.Join(" ", args)}");
            if (output.Item2.Trim() != "") throw new Exception($"Error preprocessing file \"{filepath}\":\n{output.Item2}");
            return output.Item1;
        }

        public static string GetExecutableFolder() => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new FileNotFoundException("Could not get path");

        public static string GetPreprocessorPath() => Path.Join(GetExecutableFolder(), "simplecpp", RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "simplecpp" : "simplecpp.exe");

        public static (string, string) ExecuteProgram(string program, string args)
        {
            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.FileName = program;
            p.StartInfo.Arguments = args;

			var stdout = new StringBuilder();
			var stderr = new StringBuilder();

			using (var outputWaitHandle = new AutoResetEvent(false))
			using (var errorWaitHandle = new AutoResetEvent(false))
			{
				p.OutputDataReceived += (sender, e) => {
					if (e.Data == null)
					{
						outputWaitHandle.Set();
					}
					else
					{
						stdout.AppendLine(e.Data);
					}
				};
				p.ErrorDataReceived += (sender, e) =>
				{
					if (e.Data == null)
					{
						errorWaitHandle.Set();
					}
					else
					{
						stderr.AppendLine(e.Data);
					}
				};

				p.Start();

				p.BeginOutputReadLine();
				p.BeginErrorReadLine();

                p.WaitForExit();

				//if (p.WaitForExit(timeout) && outputWaitHandle.WaitOne(timeout) && errorWaitHandle.WaitOne(timeout))
				//{
				//}
				//else
				//{
				//}
			}

			return (stdout.ToString(), stderr.ToString());
        }

        public static readonly string[] LibC = ["dankle.c", "printf.c", "string.c", "stdlib.c", "filesystem.c"];
        public static readonly string[] DankleOS = [.. new List<string> { "main.c", "commands.c" }.Select(i => "../../../../DankleOS/src/" + i)];
    }
}
