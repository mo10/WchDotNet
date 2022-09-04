using System;
using System.Collections.Generic;
using System.Text;

namespace wchisp.Devices
{

    class WCHDeviceRegisterField
    {
        /// <summary>
        /// Inclusive range, [MSB, LSB]
        /// </summary>
        public byte[] BitRange= null;
        public string Name = string.Empty;
        public string Description = string.Empty;
        public Dictionary<byte, string> Explaination = null;
    }
    class WCHDeviceRegister
    {
        public byte Offset = 0;
        public string Name = string.Empty;
        public string Description = string.Empty;
        /// <summary>
        /// A u32 value, used to reset chip config, like unprotect
        /// </summary>
        public uint Reset = 0;
        public WCHDeviceRegisterField[] Fields = null;
    }
    class WCHDeviceVariant
    {
        public string Name = string.Empty;
        public byte ChipId = 0;
        public byte[] AltChipIds = null;
        public uint FlashSize = 0;
        public uint EEPROMSize = 0;
        public uint EEPROMStartAddr = 0;
        public bool IsSupportUSB = false;
        public bool IsSupportSerial = false;
        public bool IsSupportNet = false;
    }

    interface IWCHDevice
    {
        string Name { get; }
        byte MCUType { get; }
        /// <summary>
        /// Normally this is mcy_type + 0x10
        /// </summary>
        byte DeviceType { get; }
        bool IsSupportUSB { get; }
        bool IsSupportSerial { get; }
        bool IsSupportNet { get; }
        string Description { get; }
        WCHDeviceRegister[] ConfigRegisters { get; }
        WCHDeviceVariant[] variants { get; }
    }
}
