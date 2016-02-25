﻿/*
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
    public class AdapterProperty : IAdapterProperty
    {
        public string Name { get; private set; }
        public string InterfaceHint { get; private set; }
        public IList<IAdapterAttribute> Attributes { get; private set; }

        internal AdapterProperty(string ObjectName, string IfHint)
        {
            this.Name = ObjectName;
            this.InterfaceHint = IfHint;

            try
            {
                this.Attributes = new List<IAdapterAttribute>();
            }
            catch (OutOfMemoryException ex)
            {
                Debug.WriteLine("Out of memory while trying to allocate adapter property parameter containers." + ex.Message);
                throw;
            }
        }

        internal AdapterProperty(AdapterProperty Other)
        {
            this.Name = Other.Name;
            this.InterfaceHint = Other.InterfaceHint;

            try
            {
                this.Attributes = new List<IAdapterAttribute>(Other.Attributes);
            }
            catch (OutOfMemoryException ex)
            {
                Debug.WriteLine("Out of memory while trying to allocate adapter property parameter containers." + ex.Message);
                throw;
            }
        }
    }
}
