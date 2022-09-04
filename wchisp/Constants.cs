using System;
using System.Collections.Generic;
using System.Text;

namespace wchisp
{
    /// <summary>
    /// Constants about protocol and devices.
    /// </summary>
    public static class Constants
    {
        public static readonly uint MAX_PACKET_SIZE = 64;
        public static readonly uint SECTOR_SIZE = 1024;
        /// <summary>
        /// All readable and writable registers.
        /// 
        /// - `RDPR`: Read Protection
        /// - `USER`: User Config Byte (normally in Register Map datasheet)
        /// - `WPR`:  Write Protection Mask, 1=unprotected, 0=protected
        ///
        /// | BYTE0  | BYTE1  | BYTE2  | BYTE3  |
        /// |--------|--------|--------|--------|
        /// | RDPR   | nRDPR  | USER   | nUSER  |
        /// | DATA0  | nDATA0 | DATA1  | nDATA1 |
        /// | WPR0   | WPR1   | WPR2   | WPR3   |
        /// </summary>
        public static readonly byte CFG_MASK_RDPR_USER_DATA_WPR = 0x07;
        /// <summary>
        /// Bootloader version, in the format of `[0x00, major, minor, 0x00]`
        /// </summary>
        public static readonly byte CFG_MASK_BTVER = 0x08;
        /// <summary>
        /// Device Unique ID
        /// </summary>
        public static readonly byte CFG_MASK_UID = 0x10;
        /// <summary>
        /// All mask bits of CFGs
        /// </summary>
        public static readonly byte CFG_MASK_ALL = 0x1f;

        public enum Commands : byte
        {
            IDENTIFY = 0xa1,
            ISP_END = 0xa2,
            ISP_KEY = 0xa3,
            ERASE= 0xa4,
            PROGRAM = 0xa5,
            VERIFY = 0xa6,
            READ_CONFIG = 0xa7,
            WRITE_CONFIG = 0xa8,
            DATA_ERASE = 0xa9,
            DATA_PROGRAM = 0xaa,
            DATA_READ = 0xab,
            WRITE_OTP = 0xc3,
            READ_OTP = 0xc4,
            SET_BAUD = 0xc5,
        }
    }
}
