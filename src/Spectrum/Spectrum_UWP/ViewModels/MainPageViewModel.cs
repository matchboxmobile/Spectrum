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

using ObserverPrototype.Models;
using ObserverPrototype.Utils;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ObserverPrototype.ViewModels
{
    public class MainPageViewModel : ViewmodelBase
    {
        private SemaphoreSlim bulbViewModelLock = new SemaphoreSlim(1);
        
        private ObservableCollection<DisplayLampViewModel> bulbs;
        public ObservableCollection<DisplayLampViewModel> Bulbs
        {
            get { return this.bulbs; }
            set { this.SetField(ref this.bulbs, value, () => this.Bulbs); }
        }

        public MainPageViewModel()
        {
            this.bulbs = new ObservableCollection<DisplayLampViewModel>();
            BulbManager.Instance.LampUpdated += BulbManager_LampUpdated;
            BulbManager.Instance.RefreshAllBulbs();
        }

        private async void BulbManager_LampUpdated(object sender, LampUpdatedEventArgs e)
        {
            await bulbViewModelLock.WaitAsync();

            try
            {
                DisplayLampViewModel found = this.Bulbs.FirstOrDefault(b => b.ID == e.AppId && b.Name == e.DeviceName);
                if (found != null)
                {
                    await found.Update();
                    return;
                }

                await AddNewBulb(e.AppId, e.DeviceName);
            }
            finally
            {
                bulbViewModelLock.Release();
            }
        }

        private async Task AddNewBulb(string lampId, string lampName)
        {
            DisplayLamp lamp = BulbManager.Instance.GetLamp(lampId, lampName);

            if (lamp == null)
                return;

            DisplayLampViewModel lampViewModel = new DisplayLampViewModel(lampId, lampName); 
            
            var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                this.Bulbs.Add(lampViewModel);
            });
        }
    }
}
