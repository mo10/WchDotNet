using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WchDotNet.Devices;

namespace WchDotNet
{
    public class WchDevice : IDisposable
    {
        private UsbDevice UsbDevice;
        private UsbEndpointReader UsbReader;
        private UsbEndpointWriter UsbWriter;

        public WchDevice(UsbRegistry usbRegistry)
        {

            if (!usbRegistry.Open(out UsbDevice))
            {
                throw new Exception("Cannot open device");
            }

            IUsbDevice wholeUsbDevice = UsbDevice as IUsbDevice;
            if (!ReferenceEquals(wholeUsbDevice, null))
            {
                // This is a "whole" USB device. Before it can be used, 
                // the desired configuration and interface must be selected.

                // Select config #1
                wholeUsbDevice.SetConfiguration(1);

                // Claim interface #0.
                wholeUsbDevice.ClaimInterface(0);
            }

            // open read endpoint 2.
            UsbReader = UsbDevice.OpenEndpointReader(ReadEndpointID.Ep02);
            // open write endpoint 2.
            UsbWriter = UsbDevice.OpenEndpointWriter(WriteEndpointID.Ep02);

        }

        public void Dispose()
        {
            if (UsbReader != null && !UsbReader.IsDisposed)
                UsbReader.Dispose();
            UsbReader = null;
            if (UsbWriter != null && !UsbWriter.IsDisposed)
                UsbWriter.Dispose();
            UsbWriter = null;
            if (UsbDevice != null && UsbDevice.IsOpen)
                UsbDevice.Close();
            UsbDevice = null;
        }

        public Response Transfer(byte[] write_buf)
        {
            if (UsbWriter.Write(write_buf, 1000, out _) != ErrorCode.None)
                goto Failed;

            byte[] read_buf = new byte[1024];
            int buf_len;
            if (UsbReader.Read(read_buf, 1000, out buf_len) != ErrorCode.None)
                goto Failed;

            return Response.FromRaw(read_buf, buf_len);
        Failed:
            throw new Exception(UsbDevice.LastErrorString);
        }

        public Chip GetChip()
        {
            byte[] buffer = Command.Identify(0, 0);
            Response response = Transfer(buffer);

            if (!response.IsOK)
                throw new Exception("Failed to idenfity chip");

            return ChipDB.FindChip(response.Payload[0], response.Payload[1]);
        }

        public byte[] ReadConfig()
        {
            byte[] buffer = Command.ReadConfig(Constants.CFG_MASK_ALL);
            Response response = Transfer(buffer);

            if (!response.IsOK)
                throw new Exception("Failed to read config from chip");

            return response.Payload;
        }

        public bool ReadChipInfo()
        {
            var chip = GetChip();
            var buffer = ReadConfig();
            Console.WriteLine($"read_config: {string.Join(", ", buffer.Select(b => b.ToString("X2")))}");
            return true;
        }

    }
}
