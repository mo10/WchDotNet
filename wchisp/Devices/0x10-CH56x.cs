using System;
using System.Collections.Generic;
using System.Text;

namespace wchisp.Devices
{
    class CH56x : IWCHDevice
    {
        public string Name => "CH56x Series";

        public byte MCUType => 0;

        public byte DeviceType => 0x10;

        public bool IsSupportUSB => false;

        public bool IsSupportSerial => true;

        public bool IsSupportNet => false;

        public string Description => "CH56x Series, RISC-V3A (CH569/CH565), ARM9 like (CH563/CH561), RISC (CH566/CH567/CH568) NDS32? (CH568)";

        public WCHDeviceRegister[] ConfigRegisters => null;

        public WCHDeviceVariant[] variants => new WCHDeviceVariant[]
        {
            new WCHDeviceVariant()
            {
                Name ="CH561",
                ChipId = 0x61,
                AltChipIds = new byte[]{0x46},
                FlashSize = 46,
                EEPROMSize = 28,
                IsSupportUSB = false,
                IsSupportNet = true
            },
            new WCHDeviceVariant()
            {
                Name ="CH563",
                ChipId = 0x63,
                AltChipIds = new byte[]{0x42, 0x43, 0x44, 0x45},
                FlashSize = 224,
                EEPROMSize = 28,
                IsSupportUSB = true,
                IsSupportNet = true
            },
            new WCHDeviceVariant()
            {
                Name ="CH565",
                ChipId = 0x65,
                FlashSize = 448,
                EEPROMSize = 32,
                IsSupportUSB = true,
                IsSupportNet = false,
                EEPROMStartAddr = 0
            },
            new WCHDeviceVariant()
            {
                Name ="CH566",
                ChipId = 0x66,
                FlashSize = 64,
                EEPROMSize = 32,
                IsSupportUSB = true,
                IsSupportNet = false,
                EEPROMStartAddr = 0
            },
            new WCHDeviceVariant()
            {
                Name ="CH567",
                ChipId = 0x67,
                FlashSize = 192,
                EEPROMSize = 32,
                IsSupportUSB = true,
                IsSupportNet = false
            },
            new WCHDeviceVariant()
            {
                Name ="CH568",
                ChipId = 0x68,
                FlashSize = 192,
                EEPROMSize = 32,
                IsSupportUSB = true,
                IsSupportNet = false
            },
            new WCHDeviceVariant()
            {
                Name ="CH569",
                ChipId = 0x69,
                FlashSize = 448,
                EEPROMSize = 32,
                IsSupportUSB = true,
                IsSupportNet = false
            }
        };
    }
}
