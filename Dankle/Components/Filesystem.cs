using System;
using System.Text;

namespace Dankle.Components
{
    public enum FSMode
    {
        Read = 0,
        Write
    }

    public enum FSError
    {
        None = 0,
        NotFound,
        NotWriting
    }

    public class Filesystem : Component
    {
        public override string Name => "Filesystem";

        private readonly StringBuilder TextInput = new();
        private readonly string Basepath = Environment.CurrentDirectory + "/fs";
        private FSMode Mode = FSMode.Read;
        private byte[] Buffer = [];
        private FileStream? WriteFile = null;
        private int Index = 0;
        private FSError Error = FSError.None;

        public Filesystem(Computer computer, uint addr) : base(computer)
        {
            Computer.AddMemoryMapEntry(new MM(addr, this));
            Directory.CreateDirectory(Basepath);
        }

        private void FinishTextInput()
        {
            var text = TextInput.ToString();
			TextInput.Clear();

			text = text.Trim('"');

            if (text[0] != '@') text = Path.Join(Basepath, text);
            else text = text[1..];

			text = text.Trim('"');
            
			switch (Mode)
            {
                case FSMode.Read:
                    if (!File.Exists(text))
                    {
                        Error = FSError.NotFound;
                        return;
                    }
                    Buffer = File.ReadAllBytes(text);
                    Index = 0;
                    break;
                case FSMode.Write:
                    if (!Directory.Exists(Path.GetDirectoryName(text)))
                    {
                        Error = FSError.NotFound;
                        return;
                    }
                    WriteFile?.Close();
                    WriteFile = File.OpenWrite(text);
                    break;
                default:
                    throw new InvalidOperationException($"Invalid FS mode {Mode}");
            }

            Error = FSError.None;
        }

        public class MM(uint addr, Filesystem fs) : MemoryMapRegisters(addr)
        {
            public readonly Filesystem FS = fs;

            [WriteRegister(0)]
            public void SetMode(uint _, byte[] data)
            {
                FS.Mode = (FSMode)data[0];
            }

            [WriteRegister(1)]
            public void TextInput(uint _, byte[] data)
            {
                if (data[0] == 0) FS.FinishTextInput();
                else FS.TextInput.Append((char)data[0]);
            }

            [ReadRegister(2, 2)]
            public byte[] ReadBuffer(uint _)
            {
                if (FS.Index >= FS.Buffer.Length) return Utils.ToBytes<short>(-1);
                else return [0, FS.Buffer[FS.Index++]];
            }

            [WriteRegister(2, 2)]
            public void WriteBuffer(uint _, byte[] data)
            {
                if (FS.WriteFile is null)
                {
                    FS.Error = FSError.NotWriting;
                    return;
                }

                var inp = Utils.FromBytes<short>(data);
                if (inp == -1)
                {
                    FS.WriteFile.Close();
                    FS.WriteFile = null;
                }
                else FS.WriteFile.Write([data[0]]);
            }

            [ReadRegister(4, 4)]
            public byte[] ReadBufferSize(uint _)
            {
                return Utils.ToBytes(FS.Buffer.Length);
            }

            [WriteRegister(8, 4)]
            public void SetIndex(uint _, byte[] data)
            {
                FS.Index = Utils.FromBytes<int>(data);
            }

            [ReadRegister(12)]
            public byte[] ReadError(uint _)
            {
                return Utils.ToBytes((byte)FS.Error);
            }
        }
    }
}
