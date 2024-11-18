using System;

namespace DankleTranslator
{
    public interface IDriver
    {
        public string Dissassemble(string objectFilePath);
    }
}
