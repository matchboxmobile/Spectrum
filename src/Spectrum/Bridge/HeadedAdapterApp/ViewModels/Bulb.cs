using AdapterLib.Interfaces;
using System;
using System.ComponentModel;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;

namespace HeadedAdapterApp.ViewModels
{
    public class Bulb : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public bool IsOn { get; set; }

        private SolidColorBrush displayColor;
        public SolidColorBrush DisplayColor
        {
            get { return displayColor; }
            set
            {
                displayColor = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("DisplayColor"));
            }
        }

        private ILampDevice lamp;
        public ILampDevice Lamp
        {
            get { return lamp; }
            set
            {
                if (lamp != null)
                {
                    lamp.OnOffChanged -= Lamp_OnOffChanged;
                }

                lamp = value;
                lamp.OnOffChanged += Lamp_OnOffChanged;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void Lamp_OnOffChanged(object sender, bool obj)
        {
            var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (obj)
                {
                    this.DisplayColor = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    this.DisplayColor = new SolidColorBrush(Colors.Transparent);
                }
            });
        }
    }
}