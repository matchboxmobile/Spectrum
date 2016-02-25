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

namespace AdapterLib
{
    public class AdapterAttribute : IAdapterAttribute
    {
        public IAdapterValue Value { get; private set; }
        public E_ACCESS_TYPE Access { get; set; }
        public IDictionary<string, string> Annotations { get; private set; }
        public SignalBehavior COVBehavior { get; set; }

        internal AdapterAttribute(string ObjectName, object DefaultData, E_ACCESS_TYPE access = E_ACCESS_TYPE.ACCESS_READ)
        {
            try
            {
                this.Value = new AdapterValue(ObjectName, DefaultData);
                this.Annotations = new Dictionary<string, string>();
                this.Access = access;
                this.COVBehavior = SignalBehavior.Never;
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
        }

        internal AdapterAttribute(AdapterAttribute Other)
        {
            this.Value = Other.Value;
            this.Annotations = Other.Annotations;
            this.Access = Other.Access;
            this.COVBehavior = Other.COVBehavior;
        }
    }
}
