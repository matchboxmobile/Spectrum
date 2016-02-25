//-----------------------------------------------------------------------------
// <auto-generated> 
//   This code was generated by a tool. 
// 
//   Changes to this file may cause incorrect behavior and will be lost if  
//   the code is regenerated.
//
//   Tool: AllJoynCodeGenerator.exe
//
//   This tool is located in the Windows 10 SDK and the Windows 10 AllJoyn 
//   Visual Studio Extension in the Visual Studio Gallery.  
//
//   The generated code should be packaged in a Windows 10 C++/CX Runtime  
//   Component which can be consumed in any UWP-supported language using 
//   APIs that are available in Windows.Devices.AllJoyn.
//
//   Using AllJoynCodeGenerator - Invoke the following command with a valid 
//   Introspection XML file and a writable output directory:
//     AllJoynCodeGenerator -i <INPUT XML FILE> -o <OUTPUT DIRECTORY>
// </auto-generated>
//-----------------------------------------------------------------------------
#pragma once

namespace org { namespace allseen { namespace LSF { namespace LampParameters {

// This class, and the associated EventArgs classes, exist for the benefit of JavaScript developers who
// do not have the ability to implement ILampParametersService. Instead, LampParametersServiceEventAdapter
// provides the Interface implementation and exposes a set of compatible events to the developer.
public ref class LampParametersServiceEventAdapter sealed : [Windows::Foundation::Metadata::Default] ILampParametersService
{
public:
    // Method Invocation Events

    // Property Read Events
    event Windows::Foundation::TypedEventHandler<LampParametersServiceEventAdapter^, LampParametersGetVersionRequestedEventArgs^>^ GetVersionRequested;
    event Windows::Foundation::TypedEventHandler<LampParametersServiceEventAdapter^, LampParametersGetEnergy_Usage_MilliwattsRequestedEventArgs^>^ GetEnergy_Usage_MilliwattsRequested;
    event Windows::Foundation::TypedEventHandler<LampParametersServiceEventAdapter^, LampParametersGetBrightness_LumensRequestedEventArgs^>^ GetBrightness_LumensRequested;
    
    // Property Write Events

    // ILampParametersService Implementation

    virtual Windows::Foundation::IAsyncOperation<LampParametersGetVersionResult^>^ GetVersionAsync(_In_ Windows::Devices::AllJoyn::AllJoynMessageInfo^ info);
    virtual Windows::Foundation::IAsyncOperation<LampParametersGetEnergy_Usage_MilliwattsResult^>^ GetEnergy_Usage_MilliwattsAsync(_In_ Windows::Devices::AllJoyn::AllJoynMessageInfo^ info);
    virtual Windows::Foundation::IAsyncOperation<LampParametersGetBrightness_LumensResult^>^ GetBrightness_LumensAsync(_In_ Windows::Devices::AllJoyn::AllJoynMessageInfo^ info);

};

} } } } 
