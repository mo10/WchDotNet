using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Text;
using wchisp;
using wchisp.Devices;

namespace wchispcli
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach(var chusb in WCHISP.GetDevices())
            {
                bool vailed = chusb.ReadChipInfo();
                Console.WriteLine($"vailed: {vailed}");
            }
            //var a = Response.FromRaw(new byte[] { 0xa1, 0x7f, 0x02, 0x00, 0x52, 0x11 });
            Console.WriteLine("");
        }
    }
}
