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

using Q42.HueApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;


namespace AdapterLib
{
    internal class PhilipsHueController
    {
        private const string ERROR_MESSAGE = "Link button not pressed";
        private const string APP_KEY = "AppKey";
        private const string APP_NAME = "app123";
        private const string DEVICE_NAME = "dev123";

        //Delay to allow coms to happen and allow Philips to respond
        static int PhilipsDelay = 500; 

        static Q42.HueApi.LocalHueClient client;
        static string ipAddress = null;
  
        public static List<Light> cachedLights = new List<Light>();
        public static event Action CompletedInit;

        public delegate bool PromptUser();
        public static event PromptUser Prompt;

        private static bool promptLinkButton = false;
        private static MessageDialog messageDialog = null;

        public static bool Init()
        {
            RunInt();
            return true;
        }

        private static string AppKey
        {
            get
            {
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                return (string)localSettings.Values[APP_KEY];
            }
            set
            {
                ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values[APP_KEY] = value;
            }
        }


        private static async void RunInt()
        {
            try
            {
                await localInit();
            }
            catch { }
        }

        private static async Task<bool> localInit()
        {
            ipAddress = await SetUpIP();

            if (ipAddress == null)
            {
                return true;
            }

            if (AppKey == null)
            {
                client = new Q42.HueApi.LocalHueClient(ipAddress);
                try
                {
                    await Task.Delay(PhilipsDelay);
                    var res = await client.RegisterAsync(APP_NAME, DEVICE_NAME);
                    if (res == null)
                    {
                        await Task.Delay(PhilipsDelay);
                        bool ret = await localInit();
                        return ret;
                    }

                    AppKey = res;
                    client.Initialize(res);
                }
                catch (Exception err)
                {
                    if (err.Message == ERROR_MESSAGE)
                    {
                        if (messageDialog != null)
                            return false;

                        if (!promptLinkButton)
                        {
                            promptLinkButton = true;
                            bool ret = Prompt.Invoke();
                            if (ret == true)
                            {
                                await Task.Delay(PhilipsDelay + PhilipsDelay);
                                ret = await localInit();
                            }
                            promptLinkButton = false;
                        }
                        return true;
                    }
                }
            }
            else
            {
                client = new Q42.HueApi.LocalHueClient(ipAddress, AppKey);
            }

            if (await setUpLamps())
            {
                CompletedInit();
            }

            return true;
        }
        
        private static async Task<string> SetUpIP()
        {
            string lipAddress = ipAddress;
            
            if (lipAddress == null)
            {
                Q42.HueApi.HttpBridgeLocator b = new Q42.HueApi.HttpBridgeLocator();
                var result = await b.LocateBridgesAsync(TimeSpan.FromSeconds(10));
                if (result != null)
                {
                    lipAddress = result.FirstOrDefault();
                }
            }

            if (lipAddress == null)
            {
                try
                {
                    Q42.HueApi.Interfaces.IBridgeLocator locator = new Q42.HueApi.WinRT.SSDPBridgeLocator();
                    var result = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(15));
                    if (result != null)
                    {
                        lipAddress = result.FirstOrDefault();
                    }
                }
                catch { }
            }

            return lipAddress;
        }

        private static async Task<bool> setUpLamps()
        {
            bool ret = true;
            try
            {
                var allLights = await client.GetLightsAsync();
                cachedLights.AddRange(allLights);
            }
            catch (Exception er)
            {
                if (!client.IsInitialized)
                {
                    await Task.Delay(PhilipsDelay);
                    ret = await localInit();
                }
                else
                {
                    return false;
                }
            }
            return ret;
        }

        internal static async void SetSaturation(Light light, uint value)
        {
            var cmd = new LightCommand();
            cmd.Saturation = (int)(value);
            await SendCommand(cmd, light.Id);
        }

        internal static async void SetBrightness(Light light, uint value)
        {
            var cmd = new LightCommand();
            cmd.Brightness = (byte)(value);
            await SendCommand(cmd, light.Id);
        }

        internal static async void SetColorTemp(Light light, uint value)
        {
            var cmd = new LightCommand();
            cmd.ColorTemperature = (int)value;
            await SendCommand(cmd, light.Id);
        }

        internal static async void SetColor(Light light, int r, int g, int b)
        {
            var cmd = new LightCommand();
            string sred = r.ToString("X2");
            string sgreen = g.ToString("X2");
            string sblue = b.ToString("X2");

            cmd.SetColor(sred + sgreen + sblue);
            await SendCommand(cmd, light.Id);
        }

        internal static async void SetColorAndOn(Light light, int r, int g, int b)
        {
            var cmd = new LightCommand();
            string sred = r.ToString("X2");
            string sgreen = g.ToString("X2");
            string sblue = b.ToString("X2");

            cmd.TurnOn();

            cmd.SetColor(sred + sgreen + sblue);
            await SendCommand(cmd, light.Id);
        }

        internal static async void SetColorAndOnOff(Light light, int r, int g, int b, bool onoff)
        {
            var cmd = new LightCommand();
            string sred = r.ToString("X2");
            string sgreen = g.ToString("X2");
            string sblue = b.ToString("X2");

            if (onoff)
            {
                cmd.TurnOn();
            }
            else
            {
                cmd.TurnOff();
            }

            cmd.SetColor(sred + sgreen + sblue);
            await SendCommand(cmd, light.Id);
        }


        internal static async void SetHue(Light light, uint value)
        {
            var cmd = new LightCommand();
            cmd.Hue = (int)(value);
            await SendCommand(cmd, light.Id);
        }

        internal static async void SetOn(Light light, bool value)
        {
            var cmd = new LightCommand();
            if (value == true)
            {
                cmd.TurnOn();
            }
            else
            {
                cmd.TurnOff();
            }
            await SendCommand(cmd, light.Id);
        }

        internal static async Task<bool> SendCommand(LightCommand cmd, string Id)
        {
            try
            {
                await client.SendCommandAsync(cmd, new string[] { Id });
                return true;
            }
            catch { }
            return false;
        }
    }
}
