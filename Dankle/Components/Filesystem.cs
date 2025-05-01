using System;
using System.Text;

namespace Dankle.Components
{
    public enum FSMode
    {
        Read,
        Write
    }

    public class Filesystem : Component
    {
        public override string Name => "Filesystem";

        private readonly StringBuilder TextInput = new();
        private FSMode Mode = FSMode.Read;
        private byte[] Buffer = [];
        private int Index = 0;

        public Filesystem(Computer computer, uint addr) : base(computer)
        {
            Computer.AddMemoryMapEntry(new MM(addr, this));
        }

        private void FinishTextInput()
        {
            var text = TextInput.ToString();

            switch (Mode)
            {
                case FSMode.Read:
                    Buffer = File.ReadAllBytes(text);
                    Index = 0;
                    break;
                case FSMode.Write:
                    break;
                default:
                    throw new InvalidOperationException($"Invalid FS mode {Mode}");
            }

            TextInput.Clear();
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
                else FS.TextInput.Append(data[0]);
            }

            [ReadRegister(2, 2)]
            public byte[] ReadBuffer(uint _)
            {
                if (FS.Index >= FS.Buffer.Length) return Utils.ToBytes<short>(-1);
                else return [0, FS.Buffer[FS.Index++]];
            }

            [WriteRegister(4, 4)]
            public void SetIndex(uint _, byte[] data)
            {
                FS.Index = Utils.FromBytes<int>(data);
            }
        }
    }
}
