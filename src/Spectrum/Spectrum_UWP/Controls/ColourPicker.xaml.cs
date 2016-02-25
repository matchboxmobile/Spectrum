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
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ObserverPrototype.Controls
{
    public sealed partial class ColourPicker : UserControl
    {
        private bool isDrawing = false;
        private int image_width;
        private int cordinanceX;
        private int cordinanceY;

        public int XValue { get; set; }
        public int YValue { get; set; }

        public event Action<double> Callback;
        public event Action PowerCallback;

        private Color[] ColourValues;
        
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        public SolidColorBrush MainColour
        {
            get { return (SolidColorBrush)GetValue(ColourProperty); }
            set
            {
                SetValue(ColourProperty, value);
            }
        }

        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register("Angle", typeof(double), typeof(ColourPicker), new PropertyMetadata(0, new PropertyChangedCallback(OnAngleChanged)));
        public static readonly DependencyProperty ColourProperty = DependencyProperty.Register("MainColour", typeof(SolidColorBrush), typeof(ColourPicker), new PropertyMetadata(null, new PropertyChangedCallback(OnColourChanged)));

        private static void OnColourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ColourPicker;
            control.powerButton.Foreground = (SolidColorBrush)e.NewValue;
        }
        
        private static void OnAngleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ColourPicker;
            control.UpdateAngle();
        }

        public ColourPicker()
        {
            this.InitializeComponent();

            this.PointerPressed += Spinner_PointerPressed;
            this.PointerMoved += Spinner_PointerMoved;
            this.PointerReleased += Spinner_PointerReleased;

            OpenImage();
        }

        private async void OpenImage()
        {
            WriteableBitmap image;

            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/ColourWheel.png", UriKind.Absolute));
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                image = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                image.SetSource(stream);
            }

            using (Stream pixcelStream = image.PixelBuffer.AsStream())
            {
                byte[] Pixles = new byte[image.PixelHeight * image.PixelWidth * 4];

                using (Stream pixcelArrayStream = new MemoryStream(Pixles))
                {
                    pixcelStream.CopyTo(pixcelArrayStream);
                }

                ColourValues = new Color[image.PixelWidth * image.PixelHeight];

                for (int i = 0; i < Pixles.Length; i += 4)
                {
                    byte alpha = Pixles[i + 3];
                    byte red = Pixles[i + 2];
                    byte green = Pixles[i + 1];
                    byte blue = Pixles[i];

                    Color colour = Color.FromArgb(alpha, red, green, blue);
                    ColourValues[i / 4] = colour;
                }
            }

            this.image_width = image.PixelWidth;
            image = null;
        }

        private void Spinner_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            this.isDrawing = false;

            if (ColourValues != null)
            {
                if (this.Callback != null)
                {
                    this.Callback(this.Angle);
                }
            }
        }
        
        private void Spinner_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isDrawing)
            {
                const double distanceFromEdgeScaler = 0.95;
                const int minPixelsFromEdge = 5;

                PointerPoint MouseOnWheel = e.GetCurrentPoint(this.colourWheel);

                double colourWheelCenterX = this.colourWheel.ActualWidth / 2;
                double colourWheelCenterY = this.colourWheel.ActualHeight / 2;

                Point vectorToMouse = new Point(MouseOnWheel.Position.X - colourWheelCenterX, MouseOnWheel.Position.Y - colourWheelCenterY);

                double length = Math.Sqrt(vectorToMouse.X * vectorToMouse.X + vectorToMouse.Y * vectorToMouse.Y);

                Point VectorToMouseNormalised = new Point(length > 0 ? (vectorToMouse.X) / length : 0, length > 0 ? (vectorToMouse.Y) / length : 0);

                length = Math.Sqrt(VectorToMouseNormalised.X * VectorToMouseNormalised.X + VectorToMouseNormalised.Y * VectorToMouseNormalised.Y);

                int halfX = image_width / 2;

                int minX = (int)(halfX + (VectorToMouseNormalised.X * halfX * distanceFromEdgeScaler));
                int minY = (int)(halfX + (VectorToMouseNormalised.Y * halfX * distanceFromEdgeScaler));

                cordinanceX = Math.Min(Math.Max(minX, minPixelsFromEdge), image_width - minPixelsFromEdge);
                cordinanceY = Math.Min(Math.Max(minY, minPixelsFromEdge), image_width - minPixelsFromEdge);

                const int yAxisUp = -1;

                double radians = Math.Atan2(VectorToMouseNormalised.Y, VectorToMouseNormalised.X) - Math.Atan2(yAxisUp, 0);

                this.Angle = (radians * 180) / Math.PI;
                this.Angle = Angle % 360;
                if (this.Angle < 0) this.Angle += 360;
                UpdateAngle();
            }
        }

        private void UpdateAngle()
        {
            RotateTransform myRotateTransform = new RotateTransform();
            myRotateTransform.Angle = Angle;
            this.spinner.RenderTransform = myRotateTransform;
        }
        
        private void Spinner_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            this.isDrawing = true;
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
