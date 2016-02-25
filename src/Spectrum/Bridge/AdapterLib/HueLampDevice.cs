/*
The MIT License(MIT)

Copyright(c) 2015 Matchbox Mobile

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using AdapterLib.Interfaces;
using BridgeRT;
using System;

namespace AdapterLib
{
    internal class HueLampDevice : AdapterDevice, ILampDevice
    {
        private const string PHILIPS = "(Philips)";
        private const string BULB_NAME = "Matchbox Bulb";
        private const string VENDOR_NAME = "Matchbox";
        private const string MODEL = "Model";
        private const string VERSION = "1.0";
        private const string SERIAL_NUMBER = "11111";
        private const string DESCRIPTION = "Test";
        private const bool IS_FAKE = true;

        private AdapterIcon adapterIcon;
        public override IAdapterIcon Icon
        {
            get
            {
                return adapterIcon;
            }
        }

        public event EventHandler<bool> OnOffChanged;

        public HueLampDevice(Q42.HueApi.Light light) : this(false)
        {
            ((LightingService)this.lsf).SetupValues(light);
            this.Name = $"{light.Name} {PHILIPS}";
        }

        public HueLampDevice(string name) : this(IS_FAKE)
        {
            this.Name = name;
        }

        public HueLampDevice(bool isFake) : base(BULB_NAME, VENDOR_NAME, MODEL, VERSION, SERIAL_NUMBER, DESCRIPTION)
        {
            this.adapterIcon = new AdapterIcon();
            if (isFake)
            {
                this.lsf = new FakeLightingService();
                ((FakeLightingService)this.lsf).OnOffChanged += HueLampDevice_OnOffChanged;
            }
            else
                this.lsf = new LightingService();
        }

        private void HueLampDevice_OnOffChanged(object sender, bool obj)
        {
            this.OnOffChanged(sender, obj);
        }
    }
}