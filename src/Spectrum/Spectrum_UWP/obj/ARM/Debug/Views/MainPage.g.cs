﻿#pragma checksum "C:\Work\Jack\src\Spectrum\Spectrum_UWP\Views\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "1026D6996A51EE5CB75F3F5AC838FA83"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ObserverPrototype
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.ColourAlphaConverter = (global::ObserverPrototype.Converters.ColourAlphaConverter)(target);
                }
                break;
            case 2:
                {
                    global::Windows.UI.Xaml.Controls.StackPanel element2 = (global::Windows.UI.Xaml.Controls.StackPanel)(target);
                    #line 36 "..\..\..\Views\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.StackPanel)element2).Tapped += this.Grid_Tapped;
                    #line default
                }
                break;
            case 3:
                {
                    global::Windows.UI.Xaml.Controls.Primitives.ToggleButton element3 = (global::Windows.UI.Xaml.Controls.Primitives.ToggleButton)(target);
                    #line 60 "..\..\..\Views\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Primitives.ToggleButton)element3).Tapped += this.ToggleButton_Tapped;
                    #line default
                }
                break;
            case 4:
                {
                    this.BulbList = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 5:
                {
                    this.LampList = (global::Windows.UI.Xaml.Controls.ListView)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

