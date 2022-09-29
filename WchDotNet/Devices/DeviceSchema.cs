using System;
using System.Collections.Generic;
using System.Text;

namespace WchDotNet.Devices
{
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
    }
    public class ConfigRegister
    {
        public int offset { get; internal set; }
        public string name { get; internal set; }
        public string description { get; internal set; }
        public UInt32 reset { get; internal set; }
        public string type { get; internal set; }
        public Dictionary<string, string> explaination { get; internal set; }
        public RegisterField[] fields { get; internal set; }
    }
    public class RegisterField
    {
        public byte[] bit_range { get; internal set; }
        public string name { get; internal set; }
        public string description { get; internal set; }
        public Dictionary<string, string> explaination { get; internal set; }
    }
}
