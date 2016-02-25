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
    public class AdapterSignal : IAdapterSignal
    {
        public string Name { get; private set; }
        public IList<IAdapterValue> Params { get; private set; }

        internal AdapterSignal(string ObjectName)
        {
            this.Name = ObjectName;

            try
            {
                this.Params = new List<IAdapterValue>();
            }
            catch (OutOfMemoryException ex)
            {
                Debug.WriteLine("Out of memory while trying to allocate adapter signal parameter containers." + ex.Message);
                throw;
            }
        }

        internal AdapterSignal(AdapterSignal Other)
        {
            this.Name = Other.Name;

            try
            {
                this.Params = new List<IAdapterValue>(Other.Params);
            }
            catch (OutOfMemoryException ex)
            {
                Debug.WriteLine("Out of memory while trying to allocate adapter signal parameter containers." + ex.Message);
                throw;
            }
        }

        internal void AddParam(string name)
        {
            SignalParameter param = new SignalParameter(name);
            Params.Add(param);
        }
    }
}