using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using LibUsbDotNet.Main;
using LibUsbDotNet;

namespace WchDotNet
{
    static public class WchIsp
    {
        static public IEnumerable<WchDevice> GetDevices()
        {
            List<WchDevice> vs = new List<WchDevice>();

            foreach (UsbRegistry usbRegistry in UsbDevice.AllDevices)
            {
                if (usbRegistry.Vid == 0x4348 && usbRegistry.Pid == 0x55E0)
                    vs.Add(new WchDevice(usbRegistry));
            }

            return vs;
        }
    }
}
