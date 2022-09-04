using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using LibUsbDotNet.Main;
using LibUsbDotNet;

namespace wchisp
{
    static public class WCHISP
    {
        static public IEnumerable<WCHUsbDevice> GetDevices()
        {
            List<WCHUsbDevice> vs = new List<WCHUsbDevice>();

            foreach (UsbRegistry usbRegistry in UsbDevice.AllDevices)
            {
                if (usbRegistry.Vid == 0x4348 && usbRegistry.Pid == 0x55E0)
                    vs.Add(new WCHUsbDevice(usbRegistry));
            }

            return vs;
        }
    }
}
