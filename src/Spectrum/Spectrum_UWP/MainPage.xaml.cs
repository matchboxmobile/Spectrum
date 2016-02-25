using org.allseen.LSF.LampState;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.AllJoyn;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.UI.Popups;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
//https://github.com/ms-iot/internetradio/tree/master/InternetRadioDevice
//https://msdn.microsoft.com/library/windows/apps/windows.devices.alljoyn.aspx



namespace ObserverPrototype
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Windows.Devices.AllJoyn.AllJoynBusAttachment bus;
        //org.allseen.LSF.LampState.LampStateConsumer cons;
        org.allseen.LSF.LampDetails.LampDetailsWatcher det = null;
        org.allseen.LSF.LampState.LampStateWatcher watcher = null;

        DispatcherTimer TimerForPhilipsBuldUpdate;
        TimeSpan StartTime = new TimeSpan(0, 0, 4);
        TimeSpan UpdateTime = new TimeSpan(0, 0, 1);
        bool bstarted = true;

        public MainPage()
        {
            this.InitializeComponent();

            bus = new Windows.Devices.AllJoyn.AllJoynBusAttachment();
            bus.StateChanged += Bus_StateChanged;
            bus.CredentialsRequested += Bus_CredentialsRequested;


            //     det = new org.allseen.LSF.LampDetails.LampDetailsWatcher(bus);
            //     det.Added += Det_Added;
            //     det.Start();

            watcher = new org.allseen.LSF.LampState.LampStateWatcher(bus);
            watcher.Added += Watcher_Added;
            watcher.Start();

            TimerForPhilipsBuldUpdate = new DispatcherTimer();
            TimerForPhilipsBuldUpdate.Interval = StartTime;
            TimerForPhilipsBuldUpdate.Tick += TimerForPhilipsBuldUpdate_Tick;
            TimerForPhilipsBuldUpdate.Start();

        }

        private async void TimerForPhilipsBuldUpdate_Tick(object sender, object e)
        {
            try
            {
                foreach (DisplayLamp dl in bulbs)
                {
                    if (dl.IsWaiting || bstarted)
                    {
                        dl.IsWaiting = false;
                        bool ret = await updateLamp(dl);
                        updateBulbDisplay(dl);
                    }
                }
            }
            catch (Exception ex) { }
            bstarted = false;
            TimerForPhilipsBuldUpdate.Stop();
        }



        private void Bus_CredentialsRequested(AllJoynBusAttachment sender, AllJoynCredentialsRequestedEventArgs args)
        {
             Debug.WriteLine("Bus_CredentialsRequested");
        }

        private async void Det_Added(org.allseen.LSF.LampDetails.LampDetailsWatcher sender, AllJoynServiceInfo args)
        {
            AllJoynAboutDataView about = await AllJoynAboutDataView.GetDataBySessionPortAsync(args.UniqueName, bus, args.SessionPort);
            var appName = about.AppName;
            var devName = about.DeviceName;

            var vm = this.DataContext as ViewModels.MainPageViewModel;

            vm.AddBulb(devName, about.DeviceId);

            if (watcher == null)
            {
                watcher = new org.allseen.LSF.LampState.LampStateWatcher(bus);
                watcher.Added += Watcher_Added;
                watcher.Start();
            }

        }

        private async void Watcher_Added(org.allseen.LSF.LampState.LampStateWatcher sender, Windows.Devices.AllJoyn.AllJoynServiceInfo args)
        {
            Debug.Write("Watcher_Added");

            string name = args.UniqueName;

            var session = await LampStateConsumer.JoinSessionAsync(args, sender);

            if (session.Status == AllJoynStatus.Ok)
            {
                AllJoynAboutDataView about = await AllJoynAboutDataView.GetDataBySessionPortAsync(args.UniqueName, bus, args.SessionPort);
                if (about != null)
                {
                    FoundLight(session.Consumer, about);
                }
            }
            else
            {
                if (session.Status == AllJoynStatus.AuthenticationFailed)
                {
                    Debug.WriteLine("");
                }
                if (session.Status == AllJoynStatus.ConnectionRefused)
                {
                    Debug.WriteLine("");
                }
                if (session.Status == AllJoynStatus.Fail)
                {
                    Debug.WriteLine("");
                }
                if (session.Status == AllJoynStatus.InsufficientSecurity)
                {
                    Debug.WriteLine("");
                }
                if (session.Status == AllJoynStatus.OperationTimedOut)
                {
                    Debug.WriteLine("");
                }
                if (session.Status == AllJoynStatus.OtherEndClosed)
                {
                    Debug.WriteLine("");
                }
                if (session.Status == AllJoynStatus.SslConnectFailed)
                {
                    Debug.WriteLine("");
                }
                if (session.Status == AllJoynStatus.SslIdentityVerificationFailed)
                {
                    Debug.WriteLine("");
                }
            }
        }
        List<DisplayLamp> bulbs = new List<DisplayLamp>();
        private async void FoundLight(LampStateConsumer consumer, AllJoynAboutDataView about)
        {

            //     consumer.Signals.LampStateChanged()

            foreach (DisplayLamp d in bulbs)
            {
                if (about.DeviceId.Equals(d.about.DeviceId) && about.DeviceName.Equals(d.about.DeviceId))
                {
                    return;
                }
            }

            consumer.Signals.LampStateChangedReceived += Signals_LampStateChangedReceived;

            DisplayLamp dl = new DisplayLamp(consumer, about);
            bulbs.Add(dl);

            bool ret = await updateLamp(dl);
            await updateLamp(dl);

            var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                var vm = this.DataContext as ViewModels.MainPageViewModel;
                vm.AddBulb(about.DeviceName, about.DeviceId);

                updateBulbDisplay(dl);

            });
        }

        private async void Signals_LampStateChangedReceived(LampStateSignals sender, LampStateLampStateChangedReceivedEventArgs args)
        {
            Debug.Write("Signals_LampStateChangedReceived");

            string LampId = args.LampID;
            AllJoynMessageInfo mInfo = args.MessageInfo;

            foreach (DisplayLamp b in bulbs)
            {
                if (b.about.DeviceId.Equals(LampId))
                {
                    Debug.Write("Signals_LampStateChangedReceived  found");
                    bool ret = await updateLamp(b);
                    updateBulbDisplay(b);
                    return;
                }
            }
            Debug.Write("Signals_LampStateChangedReceived NOT found");
        }

        private async void updateBulbDisplay(DisplayLamp b)
        {
            var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
            {
                var vm = this.DataContext as ViewModels.MainPageViewModel;
                ViewModels.AJViewBulb ajb = vm.Update(b.about.DeviceId, b.about.DeviceName);
                if (ajb != null)
                {

                    ajb.Brightness = (byte)DisplayLamp.hexToPercent(b.Brightness.Brightness);
                    ajb.UintBrush = b.Hue.Hue;
                    ajb.Saturation = (uint)DisplayLamp.hexToPercent(b.Saturation.Saturation);
                    ajb.ColorTemperature = b.ColorTemp.ColorTemp;

                    ajb.IsBulbOn = b.OnOff.OnOff;
                }
            });

        }
     
        private async Task<bool> updateLamp(DisplayLamp dl)
        {
            try
            {
                dl.OnOff = await dl.consumer.GetOnOffAsync();
                dl.Hue = await dl.consumer.GetHueAsync();
                dl.Brightness = await dl.consumer.GetBrightnessAsync();
                dl.Saturation = await dl.consumer.GetSaturationAsync();
                dl.ColorTemp = await dl.consumer.GetColorTempAsync();
            }
            catch (Exception ex)
            {
                Debug.Write("updateLamp " + ex.Message);
            }
            return true;
        }


        private void Bus_StateChanged(Windows.Devices.AllJoyn.AllJoynBusAttachment sender, Windows.Devices.AllJoyn.AllJoynBusAttachmentStateChangedEventArgs args)
        {
            Debug.WriteLine("Bus_StateChanged");
        }

        private void LampList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            int i = 0;
            i++;
            if (LampList.SelectedItem != null)
            {
                ViewModels.AJViewBulb ajb = (ViewModels.AJViewBulb)LampList.SelectedItem;
            }
        }

        private async void OnOff_Toggled(object sender, RoutedEventArgs e)
        {
            try
            {
                ToggleSwitch ts = (ToggleSwitch)e.OriginalSource;
                var vm = ts.DataContext as ViewModels.AJViewBulb;
                if (vm != null)
                {
                    foreach (DisplayLamp d in bulbs)
                    {
                        if (vm.ID.Equals(d.about.DeviceId) && vm.Name.Equals(d.about.DeviceName))
                        {
                            await d.consumer.SetOnOffAsync(ts.IsOn);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        public static bool heuwait = false;
        private async void Hue_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider sl1 = sender as Slider;
            var vm = sl1.DataContext as ViewModels.AJViewBulb;
            if (vm == null)
            {
                return;
            }
            try
            {
                if (vm != null)
                {
                    foreach (DisplayLamp d in bulbs)
                    {
                        if (vm.ID.Equals(d.about.DeviceId) && vm.Name.Equals(d.about.DeviceName))
                        {
                            if (heuwait)
                            { return; }

                            heuwait = true;
                            uint percent = (uint)e.NewValue;
                            uint ret = ViewModels.AJViewBulb.convertFromDegree(percent);
                            LampStateSetHueResult res = (LampStateSetHueResult)await d.consumer.SetHueAsync(ret);
                            setTimerIfPhilips(d);
                            heuwait = false;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        public static bool brightnesswait = false;
        private async void Brightness_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider sl1 = sender as Slider;
            var vm = sl1.DataContext as ViewModels.AJViewBulb;
            if (vm == null)
            {
                return;
            }
            try
            {
                if (vm != null)
                {
                    foreach (DisplayLamp d in bulbs)
                    {
                        if (vm.ID.Equals(d.about.DeviceId) && vm.Name.Equals(d.about.DeviceName))
                        {
                            if (brightnesswait)
                            { return; }

                            brightnesswait = true;
                            uint brightness = DisplayLamp.PercentToHex((uint)e.NewValue);
                            LampStateSetBrightnessResult res = (LampStateSetBrightnessResult)await d.consumer.SetBrightnessAsync(brightness);
                            setTimerIfPhilips(d);
                            brightnesswait = false;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        public static bool saturationwait = false;
        private async void Saturation_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider sl1 = sender as Slider;
            var vm = sl1.DataContext as ViewModels.AJViewBulb;
            if (vm == null)
            {
                return;
            }
            try
            {
                if (vm != null)
                {
                    foreach (DisplayLamp d in bulbs)
                    {
                        if (vm.ID.Equals(d.about.DeviceId) && vm.Name.Equals(d.about.DeviceName))
                        {
                            if (saturationwait)
                            { return; }

                            saturationwait = true;
                            uint saturation = DisplayLamp.PercentToHex((uint)e.NewValue);
                            d.SetSaturation = (uint)e.NewValue;

                            LampStateSetSaturationResult res = (LampStateSetSaturationResult)await d.consumer.SetSaturationAsync(saturation);
                            setTimerIfPhilips(d);
                            saturationwait = false;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        public static bool temperaturewait = false;
        private async void Temerature_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Slider sl1 = sender as Slider;
            var vm = sl1.DataContext as ViewModels.AJViewBulb;
            if (vm == null)
            {
                return;
            }
            try
            {
                if (vm != null)
                {
                    foreach (DisplayLamp d in bulbs)
                    {
                        if (vm.ID.Equals(d.about.DeviceId) && vm.Name.Equals(d.about.DeviceName))
                        {
                            if (temperaturewait)
                            { return; }

                            temperaturewait = true;
                            uint temperature = (uint)e.NewValue;

                            if (d.SetSaturation < 99)
                            {// cant set temperature if saturation is 100%

                                LampStateSetColorTempResult res = (LampStateSetColorTempResult)await d.consumer.SetColorTempAsync(temperature);
                                setTimerIfPhilips(d);
                            }
                            else
                            {
                                var messageDialog = new MessageDialog("Cannot set Colour Temperature if Saturation is 100%");
                                await messageDialog.ShowAsync();
                                messageDialog = null;
                            }
                            temperaturewait = false;
                            return;
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        private void setTimerIfPhilips(DisplayLamp d)
        {
            // 'cause we don't have an automatical update callback from our alljoyn emulation at the moment
            if (d.about.Manufacturer.Equals("MatchboxMobile"))
            {
                TimerForPhilipsBuldUpdate.Interval = UpdateTime;
                TimerForPhilipsBuldUpdate.Start();
                d.IsWaiting = true;
            }
        }
}

    public class DisplayLamp
    {
        public LampStateConsumer consumer;
        public AllJoynAboutDataView about;

        public LampStateGetBrightnessResult Brightness { get; set;}
        public LampStateGetHueResult Hue { get; set; }
        public LampStateGetOnOffResult OnOff { get; set; }
        public LampStateGetSaturationResult Saturation { get; set; }
        public LampStateGetColorTempResult ColorTemp { get; set; }
        public uint SetSaturation = 0;
        public bool IsWaiting = false;
        public DisplayLamp(LampStateConsumer consumer, AllJoynAboutDataView about)
        {
            this.consumer = consumer;
            this.about = about;
        }

        public static int hexToPercent(uint val)
        {
            byte v = (byte)((val & 0xff000000) >> 24);
            decimal d = v;
            int percentage = (int) ((d / 255) * 100);
            return percentage;    
        }

        public static uint PercentToHex(uint val)
        {
            decimal dd = (decimal)val;
            decimal d = (dd /100) * 255;
            uint sh = (uint)d;
            sh <<= 24;
            return sh;
        }
    }
}
