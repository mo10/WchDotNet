using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wchisp
{
    public class WCHUsbDevice
    {
        public UsbRegistry UsbRegistry { get; private set; }

        private UsbDevice UsbDevice;
        private UsbEndpointReader UsbReader;
        private UsbEndpointWriter UsbWriter;

        public WCHUsbDevice(UsbRegistry usbRegistry)
        {
            this.UsbRegistry = usbRegistry;
        }

        public bool Open()
        {
            if (UsbDevice != null)
                return true;

            if (!UsbRegistry.Open(out UsbDevice))
            {
                return false;
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

            return true;
        }

        public void Close()
        {
            if (UsbReader != null && !UsbReader.IsDisposed)
                UsbReader.Dispose();
            if (UsbWriter != null && !UsbWriter.IsDisposed)
                UsbWriter.Dispose();
            if (UsbDevice != null && UsbDevice.IsOpen)
                UsbDevice.Close();

            UsbWriter = null;
            UsbReader = null;
            UsbDevice = null;
        }

        public Response Transfer(byte[] write_buf)
        {
            if (!this.Open())
                throw new Exception("Failed to open device from registry");

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

        public bool ReadChipInfo()
        {
            var buf = Command.Identify(0, 0);
            var resp = Transfer(buf);

            if (!resp.IsOK)
            {
                Console.WriteLine("Idenfity chip failed");
                return false;
            }

            var chip_db = new ChipDB();

            var chip = chip_db.FindChip(resp.Payload[0], resp.Payload[1]);
            Console.WriteLine($"Found chip: {chip.Name}");

            buf = Command.ReadConfig(Constants.CFG_MASK_ALL);
            resp = Transfer(buf);
            if (!resp.IsOK)
            {
                Console.WriteLine("read_config failed");
                return false;
            }
            Console.WriteLine($"read_config: {string.Join(", ", resp.Payload.Select(b => b.ToString("X2")))}");

            return true;
        }
    }
}
