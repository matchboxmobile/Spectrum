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
using ObserverPrototype.ViewModels;
using org.allseen.LSF.LampDetails;
using org.allseen.LSF.LampState;
using System;
using System.Diagnostics;
using Windows.Devices.AllJoyn;

namespace ObserverPrototype.Utils
{
    public class MBWatcher
    {
        private static AllJoynBusAttachment bus;
        private static AllJoynBusAttachment bus1;

        private static LampDetailsWatcher detailsWatcher = null;
        private static LampStateWatcher stateWatcher = null;

        private MainPageViewModel _viewmodel;
        public void SetContext(MainPageViewModel viewmodel)
        {
            _viewmodel = viewmodel;
        }

        public MBWatcher()
        {
            if (bus == null)
            {
                bus = new AllJoynBusAttachment();
                bus.StateChanged += Bus_StateChanged;
                bus.CredentialsRequested += Bus_CredentialsRequested; 
            }

            if (bus1 == null)
            {
                bus1 = new AllJoynBusAttachment();
                bus.StateChanged += Bus_StateChanged;
                bus.CredentialsRequested += Bus_CredentialsRequested;
            }

            if (stateWatcher == null)
            {
                stateWatcher = new LampStateWatcher(bus);
                stateWatcher.Added += Watcher_Added;
                stateWatcher.Start();
            }

            if (detailsWatcher == null)
            {
                detailsWatcher = new LampDetailsWatcher(bus1);
                detailsWatcher.Added += Details_Added;
                detailsWatcher.Start();
            }
        }
        
        private void Bus_StateChanged(AllJoynBusAttachment sender, AllJoynBusAttachmentStateChangedEventArgs args)
        {
            Debug.WriteLine("Bus_StateChanged");
        }

        private void Bus_CredentialsRequested(AllJoynBusAttachment sender, AllJoynCredentialsRequestedEventArgs args)
        {
            Debug.WriteLine("Bus_CredentialsRequested");
        }

        private async void Details_Added(org.allseen.LSF.LampDetails.LampDetailsWatcher sender, AllJoynServiceInfo args)
        {
            var session = await LampDetailsConsumer.JoinSessionAsync(args, sender);

            if (session.Status == AllJoynStatus.Ok)
            {
                AllJoynAboutDataView about = await AllJoynAboutDataView.GetDataBySessionPortAsync(args.UniqueName, bus, args.SessionPort);
                if (about != null)
                {
                    LampDetailsGetColorResult lres = await session.Consumer.GetColorAsync();
                    DisplayLamp dl = new DisplayLamp(session.Consumer, about);
                    dl.isColourLamp = lres;
                    BulbManager.Instance.AddBulb(dl);
                }
            }
        }
        
        private async void Watcher_Added(LampStateWatcher sender, AllJoynServiceInfo args)
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
        }
        
        private async void FoundLight(LampStateConsumer consumer, AllJoynAboutDataView about)
        {
            consumer.Signals.LampStateChangedReceived += Signals_LampStateChangedReceived;

            DisplayLamp dl = new DisplayLamp(consumer, about);
            BulbManager.Instance.AddBulb(dl);

            var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
               App.MainMenu.Count = $"({BulbManager.Instance.NumberOfLamps()})"; ;
            });
        }
        
        private void Signals_LampStateChangedReceived(LampStateSignals sender, LampStateLampStateChangedReceivedEventArgs args)
        {
            Debug.WriteLine("Signals_LampStateChangedReceived");
        }
    }
}
