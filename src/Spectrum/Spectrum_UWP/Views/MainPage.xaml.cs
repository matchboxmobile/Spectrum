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
using ObserverPrototype.Views;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace ObserverPrototype
{
    public sealed partial class MainPage : Page
    {
        private const string TITLE = "Lights";
        private const string MANUFACTURER = "matchboxmobile";
        private const string TOGGLE_BUTTON_ON_STATE = "On";
        private const string TOGGLE_BUTTON_OFF_STATE = "Off";

        private MainPageViewModel viewmodel;
        public static MBWatcher mbWatcher;

        public MainPage()
        {
            this.InitializeComponent();

            App.MainMenu.Title = TITLE;

            this.viewmodel = this.DataContext as MainPageViewModel;

            if ( mbWatcher == null )
            {
                mbWatcher = new MBWatcher();
                mbWatcher.SetContext(this.viewmodel);
            }
        }

        private void ToggleButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                ToggleButton ts = (ToggleButton)sender;
                var vm = ts.DataContext as DisplayLampViewModel;
                if (vm != null)
                {
                    vm.OnOffState = !vm.OnOffState;
                    if (vm.OnOffState)
                    {
                        VisualStateManager.GoToState(ts, TOGGLE_BUTTON_ON_STATE, true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(ts, TOGGLE_BUTTON_OFF_STATE, true);
                    }
                }
            }
            catch (Exception) { }
        }
        
        private async void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FrameworkElement ts = (FrameworkElement)sender;
            var vm = ts.DataContext as DisplayLampViewModel;

            if (await vm.GetIsColor())
            {
                Frame.Navigate(typeof(DeviceBulbView), vm);
            }
            else
            {
                Frame.Navigate(typeof(TempratureDeviceView), vm);
            }
        }
    }
}
