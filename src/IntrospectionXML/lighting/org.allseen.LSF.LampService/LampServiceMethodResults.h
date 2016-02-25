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

namespace org { namespace allseen { namespace LSF { namespace LampService {

ref class LampServiceConsumer;

public ref class LampServiceClearLampFaultResult sealed
{
public:
    property int32 Status
    {
        int32 get() { return m_status; }
    internal:
        void set(_In_ int32 value) { m_status = value; }
    }

    property uint32 LampResponseCode
    {
        uint32 get() { return m_interfaceMemberLampResponseCode; }
    internal:
        void set(_In_ uint32 value) { m_interfaceMemberLampResponseCode = value; }
    }
    property uint32 LampFaultCode
    {
        uint32 get() { return m_interfaceMemberLampFaultCode; }
    internal:
        void set(_In_ uint32 value) { m_interfaceMemberLampFaultCode = value; }
    }
    
    static LampServiceClearLampFaultResult^ CreateSuccessResult(_In_ uint32 interfaceMemberLampResponseCode, _In_ uint32 interfaceMemberLampFaultCode)
    {
        auto result = ref new LampServiceClearLampFaultResult();
        result->Status = Windows::Devices::AllJoyn::AllJoynStatus::Ok;
        result->LampResponseCode = interfaceMemberLampResponseCode;
        result->LampFaultCode = interfaceMemberLampFaultCode;
        return result;
    }
    
    static LampServiceClearLampFaultResult^ CreateFailureResult(_In_ int32 status)
    {
        auto result = ref new LampServiceClearLampFaultResult();
        result->Status = status;
        return result;
    }

private:
    int32 m_status;
    uint32 m_interfaceMemberLampResponseCode;
    uint32 m_interfaceMemberLampFaultCode;
};

public ref class LampServiceJoinSessionResult sealed
{
public:
    property int32 Status
    {
        int32 get() { return m_status; }
    internal:
        void set(_In_ int32 value) { m_status = value; }
    }

    property LampServiceConsumer^ Consumer
    {
        LampServiceConsumer^ get() { return m_consumer; }
    internal:
        void set(_In_ LampServiceConsumer^ value) { m_consumer = value; }
    };

private:
    int32 m_status;
    LampServiceConsumer^ m_consumer;
};

public ref class LampServiceGetVersionResult sealed
{
public:
    property int32 Status
    {
        int32 get() { return m_status; }
    internal:
        void set(_In_ int32 value) { m_status = value; }
    }

    property uint32 Version
    {
        uint32 get() { return m_value; }
    internal:
        void set(_In_ uint32 value) { m_value = value; }
    }

    static LampServiceGetVersionResult^ CreateSuccessResult(_In_ uint32 value)
    {
        auto result = ref new LampServiceGetVersionResult();
        result->Status = Windows::Devices::AllJoyn::AllJoynStatus::Ok;
        result->Version = value;
        return result;
    }

    static LampServiceGetVersionResult^ CreateFailureResult(_In_ int32 status)
    {
        auto result = ref new LampServiceGetVersionResult();
        result->Status = status;
        return result;
    }

private:
    int32 m_status;
    uint32 m_value;
};

public ref class LampServiceGetLampServiceVersionResult sealed
{
public:
    property int32 Status
    {
        int32 get() { return m_status; }
    internal:
        void set(_In_ int32 value) { m_status = value; }
    }

    property uint32 LampServiceVersion
    {
        uint32 get() { return m_value; }
    internal:
        void set(_In_ uint32 value) { m_value = value; }
    }

    static LampServiceGetLampServiceVersionResult^ CreateSuccessResult(_In_ uint32 value)
    {
        auto result = ref new LampServiceGetLampServiceVersionResult();
        result->Status = Windows::Devices::AllJoyn::AllJoynStatus::Ok;
        result->LampServiceVersion = value;
        return result;
    }

    static LampServiceGetLampServiceVersionResult^ CreateFailureResult(_In_ int32 status)
    {
        auto result = ref new LampServiceGetLampServiceVersionResult();
        result->Status = status;
        return result;
    }

private:
    int32 m_status;
    uint32 m_value;
};

public ref class LampServiceGetLampFaultsResult sealed
{
public:
    property int32 Status
    {
        int32 get() { return m_status; }
    internal:
        void set(_In_ int32 value) { m_status = value; }
    }

    property Windows::Foundation::Collections::IVector<uint32>^ LampFaults
    {
        Windows::Foundation::Collections::IVector<uint32>^ get() { return m_value; }
    internal:
        void set(_In_ Windows::Foundation::Collections::IVector<uint32>^ value) { m_value = value; }
    }

    static LampServiceGetLampFaultsResult^ CreateSuccessResult(_In_ Windows::Foundation::Collections::IVector<uint32>^ value)
    {
        auto result = ref new LampServiceGetLampFaultsResult();
        result->Status = Windows::Devices::AllJoyn::AllJoynStatus::Ok;
        result->LampFaults = value;
        return result;
    }

    static LampServiceGetLampFaultsResult^ CreateFailureResult(_In_ int32 status)
    {
        auto result = ref new LampServiceGetLampFaultsResult();
        result->Status = status;
        return result;
    }

private:
    int32 m_status;
    Windows::Foundation::Collections::IVector<uint32>^ m_value;
};

} } } } 