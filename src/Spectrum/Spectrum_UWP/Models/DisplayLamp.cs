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

using org.allseen.LSF.LampDetails;
using org.allseen.LSF.LampState;
using Windows.Devices.AllJoyn;

namespace ObserverPrototype.Models
{
    public class DisplayLamp
    {
        public LampStateConsumer consumer;
        public AllJoynAboutDataView about;
        public LampDetailsConsumer DetailsConsumer { get; set;}
        public LampStateGetBrightnessResult Brightness { get; set; }
        public LampStateGetHueResult Hue { get; set; }
        public LampStateGetOnOffResult OnOff { get; set; }
        public LampStateGetSaturationResult Saturation { get; set; }
        public LampStateGetColorTempResult ColorTemp { get; set; }
        public LampDetailsGetColorResult isColourLamp { get; set; }

        public uint SetSaturation = 0;
        public bool IsWaiting = false;
        
        public DisplayLamp(LampStateConsumer consumer, AllJoynAboutDataView about)
        {
            this.consumer = consumer;
            this.about = about;
        }

        public DisplayLamp(LampDetailsConsumer consumer, AllJoynAboutDataView about)
        {
            this.DetailsConsumer = consumer;
            this.about = about;
        }

    }
}