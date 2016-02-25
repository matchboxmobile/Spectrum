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

using BridgeRT;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AdapterLib
{
    public class AdapterMethod : IAdapterMethod
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public IList<IAdapterValue> InputParams { get; set; }
        public IList<IAdapterValue> OutputParams { get; private set; }
        public int HResult { get; private set; }

        internal AdapterMethod(string ObjectName, string Description, int ReturnValue)
        {
            this.Name = ObjectName;
            this.Description = Description;
            this.HResult = ReturnValue;

            try
            {
                this.InputParams = new List<IAdapterValue>();
                this.OutputParams = new List<IAdapterValue>();
            }
            catch (OutOfMemoryException ex)
            {
                Debug.WriteLine("Out of memory while trying to allocate adapter method parameter containers." + ex.Message);
                throw;
            }
        }

        internal AdapterMethod(AdapterMethod Other)
        {
            this.Name = Other.Name;
            this.Description = Other.Description;
            this.HResult = Other.HResult;

            try
            {
                this.InputParams = new List<IAdapterValue>(Other.InputParams);
                this.OutputParams = new List<IAdapterValue>(Other.OutputParams);
            }
            catch (OutOfMemoryException ex)
            {
                Debug.WriteLine("Out of memory while trying to allocate adapter method parameter containers." + ex.Message);
                throw;
            }
        }

        internal void SetResult(int ReturnValue)
        {
            this.HResult = ReturnValue;
        }
    }
}