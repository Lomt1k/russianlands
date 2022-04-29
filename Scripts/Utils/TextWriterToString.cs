using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TextGameRPG.Scripts.Utils
{
    internal class TextWriterToString : TextWriter
    {
        public const int _updateTime = 100;

        private StringBuilder _outputBuilder;
        private bool _hasUpdates = false;

        public Action<string> onUpdate;

        public override Encoding Encoding => Encoding.UTF8;
        public string output { get; private set; } = string.Empty;

        public TextWriterToString(Action<string> onUpdate)
        {
            _outputBuilder = new StringBuilder();
            this.onUpdate = onUpdate;
            RefreshOutputAsync();
        }

        private async void RefreshOutputAsync()
        {
            while (true)
            {
                await Task.Delay(_updateTime);
                if (_hasUpdates)
                {
                    output = _outputBuilder.ToString();
                    _hasUpdates = false;
                    onUpdate(output);
                }
            }
        }

        public override void Write(char value)
        {
            base.Write(value);
            _outputBuilder.Append(value);
            _hasUpdates = true;
        }
        
    }
}
