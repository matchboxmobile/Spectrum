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
    public sealed partial class TempratureDeviceView : Page
    {
        private const string Title = "Temperature";

        private TempratureDeviceViewModel viewmodel;

        public TempratureDeviceView()
        {
            this.InitializeComponent();

            this.viewmodel = new TempratureDeviceViewModel(string.Empty, string.Empty);
            this.DataContext = viewmodel;

            BulbManager.Instance.LampUpdated += BulbManager_LampUpdated;

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += TempDeviceView_BackRequested;

            this.tempPicker.Callback += TempPicker_Callback;
            this.tempPicker.PowerCallback += TempPicker_PowerCallback;
        }

        private async void BulbManager_LampUpdated(object sender, LampUpdatedEventArgs e)
        {
            if (e.AppId == viewmodel.ID && e.DeviceName == viewmodel.Name)
            {
                await viewmodel.Update();
            }
        }

        private void TempPicker_PowerCallback()
        {
            viewmodel.OnOffState = !this.viewmodel.OnOffState;
        }

        private void TempPicker_Callback(double angle)
        {
            this.viewmodel.UpdateTemperature(angle);
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
                    viewmodel.Update().Forget(); ;
                }

                base.OnNavigatedTo(e);
            }
            catch (Exception)
            {
                if (Frame.CanGoBack)
                    Frame.GoBack();
            }
        }

        private void TempDeviceView_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (this.Frame.CanGoBack)
                this.Frame.GoBack();

            e.Handled = true;
        }
    }
}
