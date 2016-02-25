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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using BridgeRT;
using Q42.HueApi;
using Windows.UI.Popups;
using Windows.UI.Core;
using AdapterLib.Interfaces;
using System.Diagnostics;

namespace AdapterLib
{
    public sealed class Adapter : IAdapter
    {
        private const uint ERROR_SUCCESS = 0;
        private const uint ERROR_INVALID_HANDLE = 6;

        // Device Arrival and Device Removal Signal Indices
        private const int DEVICE_ARRIVAL_SIGNAL_INDEX = 0;
        private const int DEVICE_ARRIVAL_SIGNAL_PARAM_INDEX = 0;
        private const int DEVICE_REMOVAL_SIGNAL_INDEX = 1;
        private const int DEVICE_REMOVAL_SIGNAL_PARAM_INDEX = 0;

        private const string VENDOR_NAME = "Matchbox Mobile";
        private const string ADAPTER_NAME = "Matchbox Hue Bridge";
        private const string ADAPTER_PREFIX = "com.";
        private const string EXPOSED_APPLICATION_NAME = "DeviceSystemBridge";
        private const string APLICATION_GUID = "{0x0e693ba3,0x827e,0x46c7,{0xb7,0xcd,0xf0,0xdf,0xf4,0xf6,0xc6,0xe8}}";
        private const string VERSION_NUMBER = "0.0.0.1";

        private const string DEVICE_NAME = "Matchbox Lamp";
        private const string VENDOR = "Matchbox";
        private const string MODEL = "Model 1";
        private const string VERSION = "1.0.0.0";
        private const string SERIAL_NUMBER = "1111111111111";
        private const string DESCRIPTION = "A Custom Philips Hue Bridge";
        private const string PRESS_BUTTON_MESSAGE = "Please press Link button on the Philips Bridge";

        public string Vendor { get; }

        public string AdapterName { get; }
        
        public string Version { get; }

        public string ExposedAdapterPrefix { get; }

        public string ExposedApplicationName { get; }

        public Guid ExposedApplicationGuid { get; }

        public IList<IAdapterSignal> Signals { get; }
        
        public Adapter()
        {
            Windows.ApplicationModel.Package package = Windows.ApplicationModel.Package.Current;
            Windows.ApplicationModel.PackageId packageId = package.Id;
            Windows.ApplicationModel.PackageVersion versionFromPkg = packageId.Version;

            this.Vendor = VENDOR_NAME;
            this.AdapterName = ADAPTER_NAME;

            this.ExposedAdapterPrefix =  ADAPTER_PREFIX + this.Vendor.ToLower();
            this.ExposedApplicationGuid = Guid.Parse(APLICATION_GUID);

            if (null != package && null != packageId)
            {
                this.ExposedApplicationName = packageId.Name;
                this.Version = versionFromPkg.Major.ToString() + "." +
                               versionFromPkg.Minor.ToString() + "." +
                               versionFromPkg.Revision.ToString() + "." +
                               versionFromPkg.Build.ToString();
            }
            else
            {
                this.ExposedApplicationName = EXPOSED_APPLICATION_NAME;
                this.Version = VERSION_NUMBER;
            }

            try
            {
                this.Signals = new List<IAdapterSignal>();
                this.devices = new List<IAdapterDevice>();
                this.signalListeners = new Dictionary<int, IList<SIGNAL_LISTENER_ENTRY>>();

                //Create Adapter Signals
                this.createSignals();
            }
            catch (OutOfMemoryException ex)
            {
                Debug.WriteLine("Out of memory while trying to allocate adapter value parameter containers." + ex.Message);
                throw;
            }
        }

        public uint SetConfiguration([ReadOnlyArray] byte[] ConfigurationData)
        {
            return ERROR_SUCCESS;
        }

        public uint GetConfiguration(out byte[] ConfigurationDataPtr)
        {
            ConfigurationDataPtr = null;

            return ERROR_SUCCESS;
        }
        
        private void CreateLamp(Light light)
        {
            var dev = new HueLampDevice(light);
            devices.Add(dev);
            NotifyDeviceArrival(dev);
        }

        public ILampDevice CreateLamp(string name)
        {
            var dev = new HueLampDevice(name);

            devices.Add(dev);
            NotifyDeviceArrival(dev);
            return dev;
        }

        public uint Initialize()
        {
            PhilipsHueController.Prompt += PhilipsHueController_Prompt;
            PhilipsHueController.CompletedInit += PhilipsHueController_CompletedInit;
            bool ret = PhilipsHueController.Init();
            return ERROR_SUCCESS;
        }


        private bool PhilipsHueController_Prompt()
        {
            var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            bool ret = false;
            dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
                var messageDialog = new MessageDialog(PRESS_BUTTON_MESSAGE);
                await messageDialog.ShowAsync();
                PhilipsHueController.Init();
            });
            return true;
        }

        private void PhilipsHueController_CompletedInit()
        {
            foreach (var light in PhilipsHueController.cachedLights)
            {
                CreateLamp(light);
            }
        }

        private async void PhilipsHueController_PromptLinkButton()
        {
            var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
                var messageDialog = new MessageDialog(PRESS_BUTTON_MESSAGE);
                await messageDialog.ShowAsync();
                messageDialog = null;
                await Task.Delay(1000);
            });
        }

        public uint Shutdown()
        {
            return ERROR_SUCCESS;
        }

        public uint EnumDevices(ENUM_DEVICES_OPTIONS Options, out IList<IAdapterDevice> DeviceListPtr, out IAdapterIoRequest RequestPtr)
        {
            RequestPtr = null;

            try
            {
                DeviceListPtr = new List<IAdapterDevice>(this.devices);
            }
            catch (OutOfMemoryException ex)
            {
                Debug.WriteLine("Out of memory while trying to allocate DeviceListPtr." + ex.Message);
                throw;
            }

            return ERROR_SUCCESS;
        }

        public uint GetProperty(IAdapterProperty Property, out IAdapterIoRequest RequestPtr)
        {
            RequestPtr = null;
            return ERROR_SUCCESS;
        }

        public uint SetProperty(IAdapterProperty Property, out IAdapterIoRequest RequestPtr)
        {
            RequestPtr = null;
            return ERROR_SUCCESS;
        }

        public uint GetPropertyValue(IAdapterProperty Property, string AttributeName, out IAdapterValue ValuePtr, out IAdapterIoRequest RequestPtr)
        {
            ValuePtr = null;
            RequestPtr = null;

            foreach (var attribute in ((AdapterProperty)Property).Attributes)
            {
                if (attribute.Value.Name == AttributeName)
                {
                    ValuePtr = attribute.Value;
                    return ERROR_SUCCESS;
                }
            }

            return ERROR_INVALID_HANDLE;
        }

        public uint SetPropertyValue(IAdapterProperty Property, IAdapterValue Value, out IAdapterIoRequest RequestPtr)
        {
            RequestPtr = null;
            
            foreach (var attribute in ((AdapterProperty)Property).Attributes)
            {
                if (attribute.Value.Name == Value.Name)
                {
                    attribute.Value.Data = Value.Data;
                    return ERROR_SUCCESS;
                }
            }

            return ERROR_INVALID_HANDLE;
        }

        public uint CallMethod(IAdapterMethod Method, out IAdapterIoRequest RequestPtr)
        {
            RequestPtr = null;
            return ERROR_SUCCESS;
        }

        public uint RegisterSignalListener(IAdapterSignal Signal, IAdapterSignalListener Listener, object ListenerContext)
        {
            if (Signal == null || Listener == null)
            {
                return ERROR_INVALID_HANDLE;
            }

            int signalHashCode = Signal.GetHashCode();

            SIGNAL_LISTENER_ENTRY newEntry;
            newEntry.Signal = Signal;
            newEntry.Listener = Listener;
            newEntry.Context = ListenerContext;

            lock (this.signalListeners)
            {
                if (this.signalListeners.ContainsKey(signalHashCode))
                {
                    this.signalListeners[signalHashCode].Add(newEntry);
                }
                else
                {
                    IList<SIGNAL_LISTENER_ENTRY> newEntryList;

                    try
                    {
                        newEntryList = new List<SIGNAL_LISTENER_ENTRY>();
                    }
                    catch (OutOfMemoryException)
                    {
                        throw;
                    }

                    newEntryList.Add(newEntry);
                    this.signalListeners.Add(signalHashCode, newEntryList);
                }
            }

            return ERROR_SUCCESS;
        }

        public uint UnregisterSignalListener(IAdapterSignal Signal, IAdapterSignalListener Listener)
        {
            return ERROR_SUCCESS;
        }

        public uint NotifySignalListener(IAdapterSignal Signal)
        {
            if (Signal == null)
            {
                return ERROR_INVALID_HANDLE;
            }

            int signalHashCode = Signal.GetHashCode();

            lock (this.signalListeners)
            {
                IList<SIGNAL_LISTENER_ENTRY> listenerList = this.signalListeners[signalHashCode];
                foreach (SIGNAL_LISTENER_ENTRY entry in listenerList)
                {
                    IAdapterSignalListener listener = entry.Listener;
                    object listenerContext = entry.Context;
                    listener.AdapterSignalHandler(Signal, listenerContext);
                }
            }

            return ERROR_SUCCESS;
        }

        public uint NotifyDeviceArrival(IAdapterDevice Device)
        {
            if (Device == null)
            {
                return ERROR_INVALID_HANDLE;
            }

            IAdapterSignal deviceArrivalSignal = this.Signals[DEVICE_ARRIVAL_SIGNAL_INDEX];
            IAdapterValue signalParam = deviceArrivalSignal.Params[DEVICE_ARRIVAL_SIGNAL_PARAM_INDEX];
            signalParam.Data = Device;
            this.NotifySignalListener(deviceArrivalSignal);

            return ERROR_SUCCESS;
        }

        public uint NotifyDeviceRemoval(IAdapterDevice Device)
        {
            if (Device == null)
            {
                return ERROR_INVALID_HANDLE;
            }

            IAdapterSignal deviceRemovalSignal = this.Signals[DEVICE_REMOVAL_SIGNAL_INDEX];
            IAdapterValue signalParam = deviceRemovalSignal.Params[DEVICE_REMOVAL_SIGNAL_PARAM_INDEX];
            signalParam.Data = Device;
            this.NotifySignalListener(deviceRemovalSignal);

            return ERROR_SUCCESS;
        }

        private void createSignals()
        {
            try
            {
                // Device Arrival Signal
                AdapterSignal deviceArrivalSignal = new AdapterSignal(Constants.DEVICE_ARRIVAL_SIGNAL);
                AdapterValue deviceHandle_arrival = new AdapterValue(Constants.DEVICE_ARRIVAL__DEVICE_HANDLE, null);
                deviceArrivalSignal.Params.Add(deviceHandle_arrival);

                // Device Removal Signal
                AdapterSignal deviceRemovalSignal = new AdapterSignal(Constants.DEVICE_REMOVAL_SIGNAL);
                AdapterValue deviceHandle_removal = new AdapterValue(Constants.DEVICE_REMOVAL__DEVICE_HANDLE, null);
                deviceRemovalSignal.Params.Add(deviceHandle_removal);

                // Add Signals to the Adapter Signals
                this.Signals.Add(deviceArrivalSignal);
                this.Signals.Add(deviceRemovalSignal);

                // change of value signal
                AdapterSignal changeOfAttributeValue = new AdapterSignal(Constants.CHANGE_OF_VALUE_SIGNAL);
                changeOfAttributeValue.AddParam(Constants.COV__PROPERTY_HANDLE);
                changeOfAttributeValue.AddParam(Constants.COV__ATTRIBUTE_HANDLE);
                this.Signals.Add(changeOfAttributeValue);

            }
            catch (OutOfMemoryException ex)
            {
                Debug.WriteLine("Out of memory while trying to create signals." + ex.Message);
                throw;
            }
        }
        
        internal void SignalChangeOfAttributeValue(IAdapterDevice device, IAdapterProperty property, IAdapterAttribute attribute)
        {
            // find change of value signal of that end point (end point == bridgeRT device)

            var covSignal = device.Signals.OfType<AdapterSignal>().FirstOrDefault(s => s.Name == Constants.CHANGE_OF_VALUE_SIGNAL);
            if (covSignal == null)
            {
                // no change of value signal
                return;
            }

            // set property and attribute param of COV signal
            // note that 
            // - ZCL cluster correspond to BridgeRT property 
            // - ZCL attribute correspond to BridgeRT attribute 
            var param = covSignal.Params.FirstOrDefault(p => p.Name == Constants.COV__PROPERTY_HANDLE);
            if (param == null)
            {
                // signal doesn't have the expected parameter
                return;
            }
            param.Data = property;

            param = covSignal.Params.FirstOrDefault(p => p.Name == Constants.COV__ATTRIBUTE_HANDLE);
            if (param == null)
            {
                // signal doesn't have the expected parameter
                return;
            }
            param.Data = attribute.Value;

            // signal change of value to BridgeRT
            NotifySignalListener(covSignal);
        }

        private struct SIGNAL_LISTENER_ENTRY
        {
            // The signal object
            internal IAdapterSignal Signal;

            // The listener object
            internal IAdapterSignalListener Listener;

            // The listener context that will be passed to the signal handler
            internal object Context;
        }

        // List of Devices
        private IList<IAdapterDevice> devices;
        public IList<IAdapterDevice> Devices
        {
            get { return devices; }
        }

        // A map of signal handle (object's hash code) and related listener entry
        private Dictionary<int, IList<SIGNAL_LISTENER_ENTRY>> signalListeners;
    }
}
