using System;

namespace DankleTranslator
{
    public interface IDriver
    {
        public void Compile(string sourcePath, string objectFilePath);
        public string Dissassemble(string objectFilePath);
    }
}
