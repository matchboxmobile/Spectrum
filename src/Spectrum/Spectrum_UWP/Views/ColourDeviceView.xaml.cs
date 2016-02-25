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

using ObserverPrototype.Utils;
using ObserverPrototype.ViewModels;
using System;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace ObserverPrototype.Views
{
    public sealed partial class DeviceBulbView : Page
    {
        private const string Title = "Colour";

        private ColourDeviceViewModel viewmodel;
        
        public DeviceBulbView()
        {
            this.InitializeComponent();
            this.viewmodel = new ColourDeviceViewModel(string.Empty, string.Empty);
            this.DataContext = viewmodel;

            BulbManager.Instance.LampUpdated += BulbManager_LampUpdated;
            
            this.colourPicker.Callback += ColourPicker_colourCallback;
            this.colourPicker.PowerCallback += ColourPicker_PowerCallback;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += DeviceBulbView_BackRequested;
        }

        private async void BulbManager_LampUpdated(object sender, LampUpdatedEventArgs e)
        {
            if (e.AppId == viewmodel.ID && e.DeviceName == viewmodel.Name)
            {
                await viewmodel.Update();
            }
        }

        private void ColourPicker_PowerCallback()
        {
            this.viewmodel.OnOffState = !this.viewmodel.OnOffState;
        }

        private void ColourPicker_colourCallback(double angle)
        {
            this.viewmodel.Hue = (uint)Math.Floor(angle); 
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                DisplayLampViewModel selected = e.Parameter as DisplayLampViewModel;
                if (selected != null)
                {
                    Debug.WriteLine(typeof(Frame));
                    App.MainMenu.Title = Title;
                    this.viewmodel.ID = selected.ID;
                    this.viewmodel.Name = selected.Name;
                    viewmodel.Update().Forget();
                }

                base.OnNavigatedTo(e);
            }
            catch (Exception)
            {
                if (Frame.CanGoBack)
                    Frame.GoBack();
            }
        }

        private void DeviceBulbView_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();

            e.Handled = true;
        }
    }
}
