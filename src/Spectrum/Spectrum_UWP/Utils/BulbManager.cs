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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ObserverPrototype.Utils
{
    public class BulbManager
    {
        private List<DisplayLamp> bulbs = new List<DisplayLamp>();
        private object bulbLock = new object();
        public event EventHandler<LampUpdatedEventArgs> LampUpdated;

        public static BulbManager Instance { get; private set; }

        static BulbManager()
        {
            Instance = new BulbManager();
        }

        public void AddBulb(DisplayLamp bulb)
        {
            DisplayLamp exists = GetLamp(bulb.about.AppId.ToString(), bulb.about.DeviceName);
            if (exists == null)
            {
                lock (bulbLock)
                {
                    bulbs.Add(bulb);
                }
            }
            else
            {
                lock (bulbLock)
                {
                    if (exists.consumer == null && bulb.consumer != null)
                    {
                        exists.consumer = bulb.consumer;
                    }

                    if (exists.DetailsConsumer == null && bulb.DetailsConsumer != null)
                    {
                        exists.DetailsConsumer = bulb.DetailsConsumer;
                    }
                }
            }

            FireUpdateBulb(bulb.about.AppId.ToString(), bulb.about.DeviceName);
        }

        public DisplayLamp GetLamp(string appId, string deviceName)
        {
            lock (bulbLock)
            {
                return bulbs.FirstOrDefault((b) => b.about.AppId.ToString() == appId && b.about.DeviceName == deviceName);
            }
        }
        

        public void RefreshAllBulbs()
        {
            if (LampUpdated != null)
            {
                lock (bulbLock)
                { 
                    foreach (DisplayLamp item in bulbs)
                    {
                        LampUpdated(this, new LampUpdatedEventArgs(item.about.AppId.ToString(), item.about.DeviceName));
                    }
                }
            }
        }

        public int NumberOfLamps()
        {
            lock (bulbLock)
            { 
                return bulbs.Count;
            }
        }
        
        public async Task UpdateBrightness(uint newValue, string id, string name)
        {
            uint brightness = HexHelper.PercentToHex((uint)newValue);
            await GetLamp(id, name)?.consumer?.SetBrightnessAsync(brightness);

            FireUpdateBulb(id, name);
        }

        public async Task UpdateOnOffState(bool newState, string id, string name)
        {
            await GetLamp(id, name)?.consumer?.SetOnOffAsync(newState);
            FireUpdateBulb(id, name);
        }
        
        public async Task UpdateTemprature(uint newValue, string id, string name)
        {
            await GetLamp(id, name)?.consumer?.SetColorTempAsync(newValue);
            FireUpdateBulb(id, name);
        }

        public async Task UpdateSaturation(double newValue, string id, string name)
        {
            if (newValue > 99)
            {
                newValue = 99;
            }
            uint sat = HexHelper.PercentToHex((uint)newValue);
            await GetLamp(id, name)?.consumer?.SetSaturationAsync(sat);
            FireUpdateBulb(id, name);
        }

        public async Task UpdateHue(uint percent, string id, string name)
        {
            uint ret = DegreeHelper.ConvertFromDegree(percent);
            await GetLamp(id, name)?.consumer?.SetHueAsync(ret);
            FireUpdateBulb(id, name);
        }

        private void FireUpdateBulb(string id, string name)
        {
            if (LampUpdated != null)
            {
                LampUpdated(this, new LampUpdatedEventArgs(id, name));
            }
        }

    }
}
