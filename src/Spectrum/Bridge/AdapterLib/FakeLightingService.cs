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
using System;

namespace AdapterLib
{
    public sealed class FakeLightingService : ILSFHandler, ILightingService
    {
        public event EventHandler<bool> OnOffChanged;
        public bool LampDetails_Color
        {
            get { return false; }
        }

        public uint LampDetails_ColorRenderingIndex
        {
            get { return 0; }
        }

        public bool LampDetails_Dimmable
        {
            get { return false; }
        }

        public bool LampDetails_HasEffects
        {
            get { return false; }
        }

        public uint LampDetails_IncandescentEquivalent
        {
            get { return 0; }
        }

        public uint LampDetails_LampBaseType
        {
            get { return 0; }
        }

        public uint LampDetails_LampBeamAngle
        {
            get { return 0; }
        }

        public string LampDetails_LampID
        {
            get { return "0"; }
        }

        public uint LampDetails_LampType
        {
            get { return 0; }
        }

        public uint LampDetails_Make
        {
            get { return 0; }
        }

        public uint LampDetails_MaxLumens
        {
            get { return 0; }
        }

        public uint LampDetails_MaxTemperature
        {
            get { return 0; }
        }

        public uint LampDetails_MaxVoltage
        {
            get { return 0; }
        }

        public uint LampDetails_MinTemperature
        {
            get { return 0; }
        }

        public uint LampDetails_MinVoltage
        {
            get { return 0; }
        }

        public uint LampDetails_Model
        {
            get { return 0; }
        }

        public uint LampDetails_Type
        {
            get { return 0; }
        }

        public bool LampDetails_VariableColorTemp
        {
            get { return false; }
        }

        public uint LampDetails_Version
        {
            get { return 0; }
        }

        public uint LampDetails_Wattage
        {
            get { return 0; }
        }

        public uint LampParameters_BrightnessLumens
        {
            get { return 0; }
        }

        public uint LampParameters_EnergyUsageMilliwatts
        {
            get { return 0; }
        }

        public uint LampParameters_Version
        {
            get { return 0; }
        }

        public uint[] LampService_LampFaults
        {
            get { return null; }
        }

        public uint LampService_LampServiceVersion
        {
            get { return 0; }
        }

        public uint LampService_Version
        {
            get { return 0; }
        }

        private uint lampState_Brightness;
        public uint LampState_Brightness
        {
            get { return lampState_Brightness; }

            set { lampState_Brightness = value; }
        }

        private uint lampState_colorTemp;
        public uint LampState_ColorTemp
        {
            get { return lampState_colorTemp; }
            set { lampState_colorTemp = value; }
        }

        private uint lampState_hue;
        public uint LampState_Hue
        {
            get { return lampState_hue; }

            set { lampState_hue = value; }
        }

        private AdapterSignal LampStateChanged = new AdapterSignal(Constants.LAMP_STATE_CHANGED_SIGNAL_NAME);

        public IAdapterSignal LampState_LampStateChanged
        {
            get { return LampStateChanged; }
        }

        bool lampState_OnOff;
        public bool LampState_OnOff
        {
            get { return lampState_OnOff; }

            set
            {
                lampState_OnOff = value;
                OnOffChanged(this, lampState_OnOff);
            }
        }

        uint sat;
        public uint LampState_Saturation
        {
            get { return sat; }

            set { sat = value; }
        }

        public uint LampState_Version
        {
            get { return 0; }
        }

        public uint ClearLampFault(uint InLampFaultCode, out uint LampResponseCode, out uint OutLampFaultCode)
        {
            LampResponseCode = OutLampFaultCode = 0;
            return 0;
        }

        public uint LampState_ApplyPulseEffect(BridgeRT.State FromState, BridgeRT.State ToState, uint Period, uint Duration, uint NumPulses, ulong Timestamp, out uint LampResponseCode)
        {
            LampResponseCode = 0;
            return 0;
        }

        public uint TransitionLampState(ulong Timestamp, BridgeRT.State NewState, uint TransitionPeriod, out uint LampResponseCode)
        {
            LampResponseCode = 0;
            return 0;
        }
    }
}