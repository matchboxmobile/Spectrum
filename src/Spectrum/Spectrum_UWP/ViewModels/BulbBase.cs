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

using ObserverPrototype.Models;
using ObserverPrototype.Utils;
using org.allseen.LSF.LampDetails;
using org.allseen.LSF.LampState;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace ObserverPrototype.ViewModels
{
    public class BulbBase : ViewmodelBase
    {
        private const string HUE_PROPERTY = "Hue";
        private const string TEMP_PROPERTY = "Temperature";
        private const string BRIGHT_PROPERTY = "Brightness";
        private const string ON_OFF_PROPERTY = "OnOffState";

        public string ID { get; set; }

        private uint hue;
        public uint Hue
        {
            get { return hue; }
            set
            {
                hue = value;
                OnPropertyChanged(HUE_PROPERTY);

                BulbManager.Instance.UpdateHue(value, ID, name).Forget();
            }
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            set { this.SetField(ref this.name, value, () => this.Name); }
        }

        private SolidColorBrush displayColour;
        public SolidColorBrush DisplayColour
        {
            get { return this.displayColour; }
            set { this.SetField(ref this.displayColour, value, () => this.DisplayColour); }
        }

        private uint brightness;
        public uint Brightness
        {
            get { return this.brightness; }
            set
            {
                this.SetField(ref this.brightness, value, () => this.Brightness);
                BulbManager.Instance.UpdateBrightness(value, this.ID, this.Name).Forget();
            }
        }

        private uint temperature;
        public uint Temperature
        {
            get { return this.temperature; }
            set
            {
                temperature = value;

                OnPropertyChanged(TEMP_PROPERTY);
                BulbManager.Instance.UpdateTemprature(value, this.ID, this.Name).Forget();
            }
        }


        private bool onOffState;
        public bool OnOffState
        {
            get { return this.onOffState; }
            set
            {
                this.SetField(ref this.onOffState, value, () => this.OnOffState);
                BulbManager.Instance.UpdateOnOffState(value, ID, name).Forget();
            }
        }

        private uint saturation;
        public uint Saturation
        {
            get { return saturation; }
            set
            {
                saturation = value;
                BulbManager.Instance.UpdateSaturation(value, ID, name).Forget();
            }
        }

        public async Task<bool> GetIsColor()
        {
            var lampLocal = lamp;

            if (lampLocal.DetailsConsumer == null)
            {
                return false;
            }

            LampDetailsGetColorResult result = await lampLocal.DetailsConsumer.GetColorAsync();
            return result != null && result.Color;
        }

        private DisplayLamp lamp => BulbManager.Instance.GetLamp(this.ID, this.Name);

        public BulbBase(string id, string name)
        {
            this.ID = id;
            this.Name = name;
            Update();
        }

        public async Task Update()
        {
            if (lamp == null)
                return;

            if (lamp.consumer == null)
                return;

            LampStateGetHueResult hueResult = await this.lamp.consumer.GetHueAsync();
            LampStateGetSaturationResult saturationResult = await this.lamp.consumer.GetSaturationAsync();
            LampStateGetBrightnessResult brigtnessResult = await this.lamp.consumer.GetBrightnessAsync();
            LampStateGetColorTempResult tempResult = await this.lamp.consumer.GetColorTempAsync();

            LampStateGetOnOffResult onOffResult = await this.lamp.consumer.GetOnOffAsync();

            var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                this.hue = HexHelper.ConvertHue(hueResult.Hue);
                this.OnPropertyChanged(HUE_PROPERTY);

                this.saturation = (byte)HexHelper.hexToPercent(saturationResult.Saturation);

                this.onOffState = onOffResult.OnOff;
                this.OnPropertyChanged(ON_OFF_PROPERTY);

                var brightness = (byte)HexHelper.hexToPercent(brigtnessResult.Brightness);
                if (this.brightness != brightness)
                {
                    this.brightness = brightness;
                    this.OnPropertyChanged(BRIGHT_PROPERTY);
                }
                this.temperature = tempResult.ColorTemp;
                this.OnPropertyChanged(TEMP_PROPERTY);

                this.DisplayColour = new SolidColorBrush(ColourHelper.ColorFromHSV(this.Hue, this.saturation, this.brightness));
            });
        }
    }
}
