using LibUsbDotNet;
using LibUsbDotNet.Main;
using System;
using System.Collections.Generic;
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

        public bool IsVailed()
        {
            if (!this.Open())
                throw new Exception("Failed to open device from registry");

            var identify = Command.Identify(0, 0);
            if (UsbWriter.Write(identify, 1000, out _) != ErrorCode.None)
                goto Failed;
            byte[] buf = new byte[1024];
            int bufLen;
            if (UsbReader.Read(buf, 1000, out bufLen) != ErrorCode.None)
                goto Failed;
            Console.WriteLine($"bufLen: {bufLen}");
        Failed:
            throw new Exception(UsbDevice.LastErrorString);
        }
    }
}
