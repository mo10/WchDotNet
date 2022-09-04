using System;
using System.Collections.Generic;
using System.Text;

namespace wchisp.Devices
{
    class CH55x : IWCHDevice
    {
        public string Name => "CH55x Series";

        public byte MCUType => 1;

        public byte DeviceType => 0x11;

        public bool IsSupportUSB => true;

        public bool IsSupportSerial => true;

        public bool IsSupportNet => false;

        public string Description => "CH55x (E8051) Series";

        public WCHDeviceRegister[] ConfigRegisters => new WCHDeviceRegister[]
        {
            new WCHDeviceRegister()
            {
                Offset = 0x00,
                Name = "REVERSED",
                Description ="Reversed 32-bit word",
                Reset = 0xFFFFFFFF,
            },
            new WCHDeviceRegister()
            {
                Offset = 0x04,
                Name = "WPROTECT",
                Description ="Reversed 32-bit word",
                Reset = 0xFFFFFFFF,
                Fields = new WCHDeviceRegisterField[]
                {
                    new WCHDeviceRegisterField()
                    {
                        BitRange = new byte[]{0, 0},
                        Name = "NO_KEY_SERIAL_DOWNLOAD",
                        Description = "Turn on No-key serial port download",
                        Explaination = new Dictionary<byte, string>()
                        {
                            {1, "Enable"},
                            {0, "Disable"}
                        }
                    },
                    new WCHDeviceRegisterField()
                    {
                        BitRange = new byte[]{1, 1},
                        Name = "DOWNLOAD_CFG",
                        Explaination = new Dictionary<byte, string>()
                        {
                            {1, "P4.6 / P15 / P3.6(Default set)"},
                            {0, "P5.1 / P51 / P1.5"}
                        }
                    }
                }
            },
            new WCHDeviceRegister()
            {
                Offset = 0x08,
                Name = "GLOBAL_CFG",
                Reset = 0xFFFF4EFF,
                Fields = new WCHDeviceRegisterField[]
                {
                    new WCHDeviceRegisterField()
                    {
                        BitRange = new byte[]{15, 15},
                        Name = "CODE_PROTECT",
                        Explaination = new Dictionary<byte, string>()
                        {
                            {0, "Forbid code & data protection"},
                            {1, "Readable"}
                        }
                    },
                    new WCHDeviceRegisterField()
                    {
                        BitRange = new byte[]{14, 14},
                        Name = "NO_BOOT_LOAD",
                        Explaination = new Dictionary<byte, string>()
                        {
                            {0, "Boot from 0x0000 Application"},
                            {1, "Boot from 0xf400 Bootloader"}
                        }
                    },
                    new WCHDeviceRegisterField()
                    {
                        BitRange = new byte[]{13, 13},
                        Name = "EN_LONG_RESET",
                        Explaination = new Dictionary<byte, string>()
                        {
                            {0, "Short reset"},
                            {1, "Wide reset, add 87ms reset time"}
                        }
                    },
                    new WCHDeviceRegisterField()
                    {
                        BitRange = new byte[]{12, 12},
                        Name = "XT_OSC_STRONG",
                        Explaination = new Dictionary<byte, string>()
                        {
                            {0, "Standard"},
                            {1, "Enhanced"}
                        }
                    },
                    new WCHDeviceRegisterField()
                    {
                        BitRange = new byte[]{11, 11},
                        Name = "EN_P5.7_RESET",
                        Explaination = new Dictionary<byte, string>()
                        {
                            {0, "Forbid"},
                            {1, "Enable reset"}
                        }
                    },
                    new WCHDeviceRegisterField()
                    {
                        BitRange = new byte[]{10, 10},
                        Name = "EN_P0_PULLUP",
                        Explaination = new Dictionary<byte, string>()
                        {
                            {0, "Forbid"},
                            {1, "Enable"}
                        }
                    },
                    new WCHDeviceRegisterField()
                    {
                        BitRange = new byte[]{9, 8},
                        Name = "RESERVED",
                        Explaination = new Dictionary<byte, string>()
                        {
                            {0b10, "Default"},
                            {0xff, "Enable"}
                        }
                    },
                    new WCHDeviceRegisterField()
                    {
                        BitRange = new byte[]{7, 0},
                        Name = "RESERVED",
                        Explaination = new Dictionary<byte, string>()
                        {
                            {0b11111111, "Default"},
                            {0xff, "Error"}
                        }
                    }
                }
            },
        };

        public WCHDeviceVariant[] variants => new WCHDeviceVariant[]
        {
            new WCHDeviceVariant()
            {
                Name = "CH551",
                ChipId = 0x51,
                FlashSize = 10 * 1024,
                EEPROMSize = 128,
                EEPROMStartAddr = 0x2800
            },
            new WCHDeviceVariant()
            {
                Name = "CH552",
                ChipId = 0x52,
                FlashSize = 14 * 1024,
                EEPROMSize = 128,
                EEPROMStartAddr = 14 * 1024
            },
            new WCHDeviceVariant()
            {
                Name = "CH554",
                ChipId = 0x54,
                FlashSize = 14 * 1024,
                EEPROMSize = 128,
                EEPROMStartAddr = 14 * 1024
            },
            new WCHDeviceVariant()
            {
                Name = "CH555",
                ChipId = 0x55,
                FlashSize = 61440,
                EEPROMSize = 1024,
                EEPROMStartAddr = 61440
            },
            new WCHDeviceVariant()
            {
                Name = "CH556",
                ChipId = 0x56,
                FlashSize = 61440,
                EEPROMSize = 1024,
                EEPROMStartAddr = 61440
            },
            new WCHDeviceVariant()
            {
                Name = "CH557",
                ChipId = 0x57,
                FlashSize = 61440,
                EEPROMSize = 1024,
                EEPROMStartAddr = 61440
            },
            new WCHDeviceVariant()
            {
                Name = "CH558",
                ChipId = 0x58,
                FlashSize = 60 * 1024,
                EEPROMSize = 5 * 1024,
                EEPROMStartAddr = 0xF000
            },
            new WCHDeviceVariant()
            {
                Name = "CH559",
                ChipId = 0x59,
                FlashSize = 1024,
                EEPROMSize = 5 * 1024,
                EEPROMStartAddr = 60 * 1024
            }
        };
    }
}
