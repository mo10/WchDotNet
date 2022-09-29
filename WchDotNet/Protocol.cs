using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;

namespace WchDotNet
{
    public static class Command
    {
        public static byte[] Identify(byte device_id, byte device_type)
        {
            IEnumerable<byte> buf = new byte[] {
                (byte)Constants.Commands.IDENTIFY,
                0x12, 0x00,
                device_id, device_type,
            }.Concat(Encoding.ASCII.GetBytes("MCU ISP & WCH.CN"));

            return buf.ToArray();
        }
        public static byte[] IspEnd(byte reason)
        {
            return new byte[]
            {
                (byte)Constants.Commands.ISP_END,
                0x01, 0x00,
                reason
            };
        }
        public static byte[] IspKey(byte[] key)
        {
            IEnumerable<byte> buf = new byte[]
            {
                (byte)Constants.Commands.ISP_KEY,
            }.Concat(BitConverter.GetBytes((ushort)key.Length))
            .Concat(key);

            return buf.ToArray();
        }
        public static byte[] Erase(uint sectors)
        {
            IEnumerable<byte> buf = new byte[]
            {
                (byte)Constants.Commands.ERASE,
                0x04, 0x00,
            }.Concat(BitConverter.GetBytes(sectors));

            return buf.ToArray();
        }
        public static byte[] Program(uint address, byte padding, byte[] data)
        {
            ushort size = (ushort)(sizeof(uint) + sizeof(byte) + data.Length);
            byte[] size_raw = BitConverter.GetBytes(size);
            byte[] address_raw = BitConverter.GetBytes(address);

            IEnumerable<byte> buf = new byte[]
            {
                (byte)Constants.Commands.PROGRAM,
                size_raw[0], size_raw[1],
                address_raw[0], address_raw[1], address_raw[2], address_raw[3],
                padding
            }.Concat(BitConverter.GetBytes(size));

            return buf.ToArray();
        }
        public static byte[] Verify(uint address, byte padding, byte[] data)
        {
            ushort size = (ushort)(sizeof(uint) + sizeof(byte) + data.Length);
            byte[] size_raw = BitConverter.GetBytes(size);
            byte[] address_raw = BitConverter.GetBytes(address);

            IEnumerable<byte> buf = new byte[]
            {
                (byte)Constants.Commands.VERIFY,
                size_raw[0], size_raw[1],
                address_raw[0], address_raw[1], address_raw[2], address_raw[3],
                padding
            }.Concat(BitConverter.GetBytes(size));

            return buf.ToArray();
        }
        public static byte[] ReadConfig(byte bit_mask)
        {
            return new byte[]
            {
                (byte)Constants.Commands.READ_CONFIG,
                0x02,0x00,
                bit_mask, 0x00,
            };
        }
        public static byte[] WriteConfig(byte bit_mask, byte[] data)
        {
            ushort size = (ushort)(sizeof(byte) + 1 + data.Length);
            byte[] size_raw = BitConverter.GetBytes(size);

            IEnumerable<byte> buf = new byte[]
            {
                (byte)Constants.Commands.WRITE_CONFIG,
                size_raw[0], size_raw[1],
                bit_mask, 0x00,
            }.Concat(data);

            return buf.ToArray();
        }
        public static byte[] DataRead(uint address, ushort len)
        {
            ushort size = (ushort)(sizeof(uint) + sizeof(ushort));
            IEnumerable<byte> buf = new byte[]
            {
                (byte)Constants.Commands.WRITE_CONFIG,
            }.Concat(BitConverter.GetBytes(size))
            .Concat(BitConverter.GetBytes(address))
            .Concat(BitConverter.GetBytes(len));

            return buf.ToArray();
        }
    }
    public class Response
    {
        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        public struct IspInfo
        {
            public byte Command;
            public byte Unknown;
            public ushort Length;
        }
        public IspInfo Info;
        public byte[] Payload;
        public bool IsOK { get; private set; } = false;

        public static Response FromRaw(byte[] buf, int buf_len)
        {
            Response resp = new Response();
            var handle = GCHandle.Alloc(buf, GCHandleType.Pinned);
            try
            {
                resp.Info = (IspInfo)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(IspInfo));
                resp.Payload = new byte[buf_len - 4];
                Array.Copy(buf, 4, resp.Payload, 0, buf_len - 4);
            }
            finally {
                handle.Free();
            }

            if(resp.Payload.Length == resp.Info.Length)
                resp.IsOK = true;

            return resp;
        }
    }
}
