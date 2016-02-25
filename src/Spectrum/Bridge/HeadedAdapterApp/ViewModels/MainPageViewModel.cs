using AdapterLib;
using BridgeRT;
using System;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace HeadedAdapterApp.ViewModels
{
    public class MainPageViewModel
    {
        private const string BULB_PREFIX = "Bulb";

        private DsbBridge dsbBridge;
        public string Status { get; set; }
        public ObservableCollection<Bulb> Bulbs { get; set; }
        private Adapter adapter;

        public MainPageViewModel()
        {
            Bulbs = new ObservableCollection<Bulb>();

            if (IsDesign)
            {
                Bulbs.Add(CreateBulb());
                Bulbs.Add(CreateBulb());
            }
            else
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                ThreadPool.RunAsync(new WorkItemHandler((IAsyncAction action) =>
                {
                    try
                    {
                        adapter = new Adapter();
                        dsbBridge = new DsbBridge(adapter);

                        var initResult = this.dsbBridge.Initialize();
                        if (initResult != 0)
                        {
                            throw new Exception("DSB Bridge initialization failed!");
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }));
            }
        }

        internal void Closing()
        {
            dsbBridge?.Shutdown();
        }

        private bool IsDesign
        {
            get
            {
                return Windows.ApplicationModel.DesignMode.DesignModeEnabled;
            }
        }

        private Bulb CreateBulb()
        {
            string name = $"{BULB_PREFIX} {Bulbs.Count + 1}";
            var lamp = adapter.CreateLamp(name);
            
            return new Bulb()
            {
                Lamp = lamp,
                Name = name,
                DisplayColor = new SolidColorBrush(Colors.White)
            };
        }

        internal void AddFakeBulb()
        {
            var bulb = CreateBulb();
            this.Bulbs.Add(bulb);
        }
    }
}
