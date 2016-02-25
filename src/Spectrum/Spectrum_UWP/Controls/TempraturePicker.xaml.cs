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
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace ObserverPrototype.Controls
{
    public sealed partial class TempraturePicker : UserControl
    {
        private const string ANGLE_PROPERTY = "Angle";

        private bool isDrawing = false;

        public event Action<double> Callback;
        public event Action PowerCallback;

        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }
        
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(ANGLE_PROPERTY, typeof(double), typeof(TempraturePicker), new PropertyMetadata(0, new PropertyChangedCallback(OnAngleChanged)));
        
        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TempraturePicker;
            control.UpdateAngle();
        }

        private void UpdateAngle()
        {
            RotateTransform myRotateTransform = new RotateTransform();
            myRotateTransform.Angle = Angle;
            this.spinner.RenderTransform = myRotateTransform;
        }
        
        public TempraturePicker()
        {
            this.InitializeComponent();

            this.PointerPressed += Spinner_PointerPressed;
            this.PointerMoved += Spinner_PointerMoved;
            this.PointerReleased += Spinner_PointerReleased;
        }

        private void Spinner_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            this.isDrawing = true;
        }

        private void Spinner_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isDrawing)
            {
                PointerPoint MouseOnWheel = e.GetCurrentPoint(this.colourWheel);

                double colourWheelCenterX = this.colourWheel.ActualWidth / 2;
                double colourWheelCenterY = this.colourWheel.ActualHeight / 2;

                Point vectorToMouse = new Point(MouseOnWheel.Position.X - colourWheelCenterX, MouseOnWheel.Position.Y - colourWheelCenterY);

                double length = Math.Sqrt(vectorToMouse.X * vectorToMouse.X + vectorToMouse.Y * vectorToMouse.Y);

                Point VectorToMouseNormalised = new Point(length > 0 ? (vectorToMouse.X) / length : 0, length > 0 ? (vectorToMouse.Y) / length : 0);

                length = Math.Sqrt(VectorToMouseNormalised.X * VectorToMouseNormalised.X + VectorToMouseNormalised.Y * VectorToMouseNormalised.Y);

                const int yAxisUp = -1;

                double radians = Math.Atan2(VectorToMouseNormalised.Y, VectorToMouseNormalised.X) - Math.Atan2(yAxisUp, 0);

                this.Angle = (radians * 180) / Math.PI;
                this.Angle = Angle % 360;
                if (this.Angle < 0) this.Angle += 360;
                UpdateAngle();
            }
        }

        private void Spinner_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            this.isDrawing = false;

            if (this.Callback != null)
            {
                this.Callback(this.Angle);
            }
        }

        private void power_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.PowerCallback != null)
            {
                this.PowerCallback();
            }
            e.Handled = true;
        }
    }
}
