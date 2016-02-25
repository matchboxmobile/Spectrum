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

using BridgeRT;
using Q42.HueApi;
using System;
using System.Threading.Tasks;

namespace AdapterLib
{
    internal class LightingService : ILSFHandler
    {
        private const uint COLOUR_RENDER_INDEX = 80;
        private const uint LAMP_BEAM_ANGLE = 160;
        private const uint LAMP_DETAILS_MAX_LUMINS = 600;
        private const uint LAMP_DETAILS_MAX_TEMP = 6500;

        public Q42.HueApi.Light light = null;
        private uint Native_Hue;
        private uint Native_Brightness;
        private uint Native_Saturation;
        private uint Native_Temperature;
        
        public bool LampDetails_Color
        {
            get { return true; }
        }

        public uint LampDetails_ColorRenderingIndex
        {
            get { return COLOUR_RENDER_INDEX; }
        }

        public bool LampDetails_Dimmable
        {
            get { return true; }
        }

        public bool LampDetails_HasEffects
        {
            get { return true; }
        }

        public uint LampDetails_IncandescentEquivalent { get; private set; }

        public uint LampDetails_LampBaseType { get; private set; }

        public uint LampDetails_LampBeamAngle
        {
            get { return LAMP_BEAM_ANGLE;  }
        }

        public string LampDetails_LampID
        {
            get { return light.Id; }
        }

        public uint LampDetails_LampType
        {
            get { return 0; }
        }

        public uint LampDetails_Make { get; private set; }

        public uint LampDetails_MaxLumens
        {
            get { return LAMP_DETAILS_MAX_LUMINS; }
        }

        public uint LampDetails_MaxTemperature
        {
            get { return LAMP_DETAILS_MAX_TEMP; }
        }

        public uint LampDetails_MaxVoltage { get; private set; }

        public uint LampDetails_MinTemperature { get; private set; }

        public uint LampDetails_MinVoltage { get; private set; }

        public uint LampDetails_Model { get; private set; }

        public uint LampDetails_Type { get; private set; }

        public bool LampDetails_VariableColorTemp
        {
            get { return true; }
        }

        public uint LampDetails_Version { get; private set; }

        public uint LampDetails_Wattage { get; private set; }

        public uint LampParameters_BrightnessLumens { get; private set; }

        public uint LampParameters_EnergyUsageMilliwatts { get; private set; }

        public uint LampParameters_Version { get; private set; }

        public uint[] LampService_LampFaults { get; private set; }

        public uint LampService_LampServiceVersion { get; private set; }

        public uint LampService_Version { get; private set; }

        public uint LampState_Brightness
        {
            get { return this.Native_Brightness; }
            set
            {
                this.Native_Brightness = value;
                value = value >> 24;
                PhilipsHueController.SetBrightness(light, value);
                light.State.Brightness = (byte)value;
            }
        }

        public uint LampState_ColorTemp
        {
            get { return Native_Temperature; }
            set
            {
                Native_Temperature = value;
                PhilipsHueController.SetColorTemp(light, value);
                light.State.ColorTemperature = (int)value;
            }
        }

        public async void SetupValues(Q42.HueApi.Light thelight)
        {
            light = thelight;

            if (light != null)
            {
                Native_Hue = (uint)light.State.Hue << 24;
                Native_Brightness = (uint)light.State.Brightness << 24;
                Native_Saturation = (uint)light.State.Saturation << 24;
                Native_Temperature = (uint)light.State.ColorTemperature << 24;

                light.State.Saturation = 0xFF;
                light.State.Brightness = 0xFF;

                byte r = 0; byte g = 0; byte b = 0;
                ColorFromHSV((double)125, (double)light.State.Saturation, (double)light.State.Brightness, out r, out g, out b);

                await Task.Delay(1500);
                PhilipsHueController.SetColorAndOn(light, (int)r, (int)g, (int)b);
            }
        }

        public uint LampState_Hue
        {
            get
            {
                return this.Native_Hue;
            }
            set
            {
                Native_Hue = value;

                value = (value >> 24) & 0X000000FF;
                
                light.State.Hue = (int)value;

                byte r = 0;
                byte g = 0;
                byte b = 0;

                ColorFromHSV((double)value, (double)light.State.Saturation, (double)light.State.Brightness, out r, out g, out b);
                PhilipsHueController.SetColor(light, (int)r, (int)g, (int)b);
            }
        }

        public static int hexToPercent(uint val)
        {
            byte v = (byte)val;
            decimal d = v;
            int percentage = (int)((d / 255) * 100);
            return percentage;
        }

        public static void ColorFromHSV(double hue, double saturation, double value, out byte r, out byte g, out byte b)
        {
            hue = convertToDegree((uint)hue);

            saturation = (double)hexToPercent((uint)saturation);
            value = (double)hexToPercent((uint)value);

            saturation /= 100;
            value /= 100;

            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
            {
                r = (byte)v; g = (byte)t; b = (byte)p;
            }
            else if (hi == 1)
            {
                r = (byte)q; g = (byte)v; b = (byte)p;
            }
            else if (hi == 2)
            {
                r = (byte)p; g = (byte)v; b = (byte)t;
            }
            else if (hi == 3)
            {
                r = (byte)p; g = (byte)q; b = (byte)v;
            }
            else if (hi == 4)
            {
                r = (byte)t; g = (byte)p; b = (byte)v;
            }
            else
            {
                r = (byte)v; g = (byte)p; b = (byte)q;
            }
        }

        public static uint convertToDegree(uint hue)
        {
            uint degrees = (uint)((double)(hue * 360) / 256);
            return degrees;
        }

        private AdapterSignal LampStateChanged = new AdapterSignal(Constants.LAMP_STATE_CHANGED_SIGNAL_NAME);

        public IAdapterSignal LampState_LampStateChanged
        {
            get
            { return LampStateChanged; }
        }

        public bool LampState_OnOff
        {
            get
            {
                return light.State.On;
            }

            set
            {
                PhilipsHueController.SetOn(light, value);
                light.State.On = value;
            }
        }

        public uint LampState_Saturation
        {
            get
            {
                return Native_Saturation;
            }
            set
            {
                Native_Saturation = value;
                value = value >> 24;
                PhilipsHueController.SetSaturation(light, value);
                light.State.Saturation = (int)value;
            }
        }

        public uint LampState_Version
        {
            get
            {
                return 0;
            }
        }

        public uint ClearLampFault(uint InLampFaultCode, out uint LampResponseCode, out uint OutLampFaultCode)
        {
            InLampFaultCode = 0;
            LampResponseCode = 0;
            OutLampFaultCode = 0;
            return 0;
        }

        public uint LampState_ApplyPulseEffect(BridgeRT.State FromState, BridgeRT.State ToState, uint Period, uint Duration, uint NumPulses, ulong Timestamp, out uint LampResponseCode)
        {
            LampResponseCode = 0;
            DoPulse(light, FromState, ToState, Period, Duration, NumPulses);
            return 0;
        }

        private async void DoPulse(Light light, BridgeRT.State FromState, BridgeRT.State ToState, uint Period, uint Duration, uint NumPulses)
        {
            for (int i = 0; i < NumPulses; i++)
            {
                SetLamp(light, FromState, Period);
                await Task.Delay((int)(Period + Duration));

                SetLamp(light, ToState, Period);
                await Task.Delay((int)(Period + Duration));
            }
        }

        private void SetLamp(Light light, BridgeRT.State thestate, uint Period)
        {
            var command = new LightCommand();
            command.TransitionTime = TimeSpan.FromMilliseconds(Period);
            command.On = thestate.IsOn;

            uint saturation = thestate.Saturation >> 24;
            uint brightness = thestate.Brightness >> 24;
            int hue = (int)thestate.Hue >> 24;
            hue &= 0X000000FF;

            byte r = 0; byte g = 0; byte b = 0;
            ColorFromHSV((double)hue, (double)saturation, (double)brightness, out r, out g, out b);
            PhilipsHueController.SetColorAndOnOff(light, (int)r, (int)g, (int)b, thestate.IsOn);
        }

        public uint TransitionLampState(ulong Timestamp, BridgeRT.State ToState, uint Period, out uint LampResponseCode)
        {
            LampResponseCode = 0;

            SetLamp(light, ToState, Period);

            this.Native_Saturation = ToState.Saturation;
            this.Native_Brightness = ToState.Brightness;
            this.Native_Hue = ToState.Hue;
            this.LampState_OnOff = ToState.IsOn;
            return 0;
        }
    }
}