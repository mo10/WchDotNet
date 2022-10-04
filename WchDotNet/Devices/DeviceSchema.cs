using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;

namespace WchDotNet.Devices
{
    /// <summary>
    /// MCU Family
    /// </summary>
    public class ChipFamily
    {
        public string name { get; internal set; }
        public byte mcu_type { get; internal set; }
        public byte device_type { get; internal set; }
        public bool support_usb { get; internal set; }
        public bool support_serial { get; internal set; }
        public bool support_net { get; internal set; }
        public string description { get; internal set; }
        public Chip[] variants { get; internal set; }
        public ConfigRegister[] config_registers { get; internal set; }
    }

    /// <summary>
    /// Represents an MCU chip
    /// </summary>
    public class Chip
    {
        public string name { get; internal set; }
        public byte chip_id { get; internal set; }
        public IEnumerable<byte> alt_chip_ids { get; internal set; }
        public byte mcu_type { get; internal set; }
        public byte device_type { get; internal set; }
        public UInt32 flash_size { get; internal set; }
        public UInt32 eeprom_size { get; internal set; }
        public UInt32 eeprom_start_addr { get; internal set; }
        public bool support_usb
        {
            get
            {
                if (is_set_support_usb)
                    return _support_usb;
                return family.support_usb;
            }
            internal set
            {
                is_set_support_usb = true;
                _support_usb = value;
            }
        }
        public bool support_serial
        {
            get
            {
                if (is_set_support_serial)
                    return _support_serial;
                return family.support_serial;
            }
            internal set
            {
                is_set_support_serial = true;
                _support_serial = value;
            }
        }
        public bool support_net
        {
            get
            {
                if (is_set_support_net)
                    return _support_net;
                return family.support_net;
            }
            internal set
            {
                is_set_support_net = true;
                _support_net = value;
            }
        }
        public ConfigRegister[] config_registers
        {
            get
            {
                if (_config_registers == null || _config_registers.Length == 0)
                    return family.config_registers;
                return _config_registers;
            }
            internal set
            {
                _config_registers = value;
            }
        }

        /* Not fill by yaml */
        public ChipFamily family { get; internal set; }
        private bool is_set_support_usb = false;
        private bool is_set_support_serial = false;
        private bool is_set_support_net = false;

        private bool _support_usb = false;
        private bool _support_serial = false;
        private bool _support_net = false;
        private ConfigRegister[] _config_registers = null;

        /// <summary>
        /// DeviceType = ChipSeries = SerialNumber = McuType + 0x10
        /// </summary>
        internal byte _device_type
        {
            get
            {
                return (byte)(mcu_type + 0x10);
            }
        }
        /// <summary>
        /// Used when erasing 1K sectors
        /// </summary>
        public uint min_erase_sector_number
        {
            get
            {
                if (_device_type == 0x10)
                    return 4;
                return 8;
            }
        }
        /// <summary>
        /// Used when calculating XOR key
        /// </summary>
        public uint uid_size
        {
            get
            {
                if (_device_type == 0x11)
                    return 4;
                return 8;
            }
        }
        /// <summary>
        /// Code flash protect support
        /// </summary>
        public bool support_code_flash_protect
        {
            get
            {
                return new byte[] { 0x14, 0x15, 0x17, 0x18, 0x19, 0x20 }.Contains(_device_type);
            }
        }

        public override string ToString()
        {
            return $"{name}[0x{chip_id:x02}{_device_type:x02}]";
        }
    }

    /// <summary>
    /// A u32 config register, with reset values.
    ///
    /// The reset value is NOT the value of the register when the device is reset,
    /// but the value of the register when the device is in the flash-able mode.
    ///
    /// Read in LE mode.
    /// </summary>
    public class ConfigRegister
    {
        public int offset { get; internal set; }
        public string name { get; internal set; }
        public string description { get; internal set; }
        public UInt32 reset { get; internal set; }
        public string type { get; internal set; }
        public IDictionary<string, string> explaination { get; set; }
        public RegisterField[] fields { get; internal set; }
    }

    /// <summary>
    /// A range of bits in a register, with a name and a description
    /// </summary>
    public class RegisterField
    {
        public int[] bit_range { get; internal set; }
        public string name { get; internal set; }
        public string description { get; internal set; }
        public IDictionary<string, string> explaination { get; internal set; }
    }
}
