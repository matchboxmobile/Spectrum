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
using Windows.UI;

namespace ObserverPrototype.Utils
{
    public static class ColourHelper
    {
        public static Color ColorFromHSV(double hue, double saturation, double brightness)
        {
            hue = DegreeHelper.ConvertToDegree((uint)hue);
            saturation /= 100;
            brightness /= 100;

            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            brightness = brightness * 255;
            int v = (int)Convert.ToInt64(brightness);
            int p = (int)Convert.ToInt64(brightness * (1 - saturation));
            int q = (int)Convert.ToInt64(brightness * (1 - f * saturation));
            int t = (int)Convert.ToInt64(brightness * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, (byte)v, (byte)t, (byte)p);
            else if (hi == 1)
                return Color.FromArgb(255, (byte)q, (byte)v, (byte)p);
            else if (hi == 2)
                return Color.FromArgb(255, (byte)p, (byte)v, (byte)t);
            else if (hi == 3)
                return Color.FromArgb(255, (byte)p, (byte)q, (byte)v);
            else if (hi == 4)
                return Color.FromArgb(255, (byte)t, (byte)p, (byte)v);
            else
                return Color.FromArgb(255, (byte)v, (byte)p, (byte)q);
        }
    }
}
