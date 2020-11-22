using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dot6502App.Viewmodel
{
    public class StatusBarViewModel : BindableBase
    {
        private int _fps;
        public int FPS
        {
            get => _fps;
            set => SetProperty(ref _fps, value);
        }

        private int _ips;
        public int IPS
        {
            get => _ips;
            set => SetProperty(ref _ips, value);
        }

        private DateTime lastUpdate = DateTime.Now;
        private int frameCounter = 0;
        private int instructionCounter = 0;

        internal void Frame()
        {
            frameCounter++;
        }

        internal void Instruction()
        {
            instructionCounter++;
            var duration = DateTime.Now - lastUpdate;
            if (duration.TotalSeconds > 1)
            {
                FPS = (int)(frameCounter / duration.TotalSeconds);
                IPS = (int)(instructionCounter / duration.TotalSeconds);
                lastUpdate = DateTime.Now;
                frameCounter = 0;
                instructionCounter = 0;
            }
        }
    }
}
