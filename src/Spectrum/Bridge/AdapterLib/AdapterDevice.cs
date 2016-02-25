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
using BridgeRT;
using System.Diagnostics;

namespace AdapterLib
{
    internal class AdapterDevice : IAdapterDevice, IAdapterDeviceLightingService, IAdapterDeviceControlPanel
    {
        public string Name { get; set; }
        public string Vendor { get; private set; }
        public string Model { get; private set; }
        public string Version { get; private set; }
        public string FirmwareVersion { get; private set; }
        public string SerialNumber { get; private set; }
        public string Description { get; private set; }
        public IList<IAdapterProperty> Properties { get; private set; }
        public IList<IAdapterMethod> Methods { get; private set; }
        public IList<IAdapterSignal> Signals { get; private set; }

        public IControlPanelHandler ControlPanelHandler
        {
            get
            {
                return null;
            }
        }

        public ILSFHandler LightingServiceHandler
        {
            get
            {
                return lsf;
            }
        }

        protected ILSFHandler lsf;
        public virtual IAdapterIcon Icon
        {
            get
            {
                return null;
            }
        }
        
        internal AdapterDevice(string Name, string VendorName, string Model, string Version, string SerialNumber, string Description)
        {
            this.Name = Name;
            this.Vendor = VendorName;
            this.Model = Model;
            this.Version = Version;
            this.FirmwareVersion = Version;
            this.SerialNumber = SerialNumber;
            this.Description = Description;

            try
            {
                this.Properties = new List<IAdapterProperty>();
                this.Methods = new List<IAdapterMethod>();
                this.Signals = new List<IAdapterSignal>();
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
        }

        internal AdapterDevice(AdapterDevice Other)
        {
            this.Name = Other.Name;
            this.Vendor = Other.Vendor;
            this.Model = Other.Model;
            this.Version = Other.Version;
            this.FirmwareVersion = Other.FirmwareVersion;
            this.SerialNumber = Other.SerialNumber;
            this.Description = Other.Description;

            try
            {
                this.Properties = new List<IAdapterProperty>(Other.Properties);
                this.Methods = new List<IAdapterMethod>(Other.Methods);
                this.Signals = new List<IAdapterSignal>(Other.Signals);
            }
            catch (OutOfMemoryException ex)
            {
                Debug.WriteLine("Out of memory while trying to allocate adapter device parameter containers." + ex.Message);
                throw;
            }
        }

        internal void AddChangeOfValueSignal(IAdapterProperty Property, IAdapterValue Attribute)
        {
            try
            {
                AdapterSignal covSignal = new AdapterSignal(Constants.CHANGE_OF_VALUE_SIGNAL);

                AdapterValue propertyHandle = new AdapterValue(Constants.COV__PROPERTY_HANDLE, Property);

                AdapterValue attrHandle = new AdapterValue(Constants.COV__ATTRIBUTE_HANDLE, Attribute);

                covSignal.Params.Add(propertyHandle);
                covSignal.Params.Add(attrHandle);

                this.Signals.Add(covSignal);
            }
            catch (OutOfMemoryException ex)
            {
                Debug.WriteLine("Out of memory while trying to change the value of the signal." + ex.Message);
                throw;
            }
        }
    }
}

