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

namespace ObserverPrototype.ViewModels
{
    public class TempratureDeviceViewModel : BulbBase
    {
        private const string TEMP_PROPERTY = "Temperature";
        private const string ANGLE_PROPERTY = "Angle";

        private const int MAX_DEGREES = 360;

        private double percent;

        private int angle;
        public int Angle
        {
            get { return angle; }
            set { this.SetField(ref this.angle, value, () => this.Angle); }
        }

        public TempratureDeviceViewModel(string id, string name): base(id, name)
        {
            this.PropertyChanged += ColourDeviceViewModel_PropertyChanged;
        }

        private void ColourDeviceViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == TEMP_PROPERTY)
            {
                this.angle = (int)(this.Temperature / (uint.MaxValue / MAX_DEGREES));
                this.OnPropertyChanged(ANGLE_PROPERTY);
            }
        }

        internal void UpdateTemperature(double angle)
        {
            if (angle != this.Angle)
            {
                this.percent = (angle / MAX_DEGREES);
                this.Temperature = (uint)(percent * uint.MaxValue);
            }
        }
    
        public double RescaleFactor(double factor, double min, double max)
        {
            return (factor * (max - min)) + min;
        }
    }
}
