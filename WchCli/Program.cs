using System;
using WchDotNet;

namespace WchCli
{
    class Program
    {
        static void Main(string[] args)
        {
            ChipDB.LoadInternalChipFamilies();
            foreach (var chusb in WchIsp.GetDevices())
            {
                bool vailed = chusb.ReadChipInfo();
                Console.WriteLine($"vailed: {vailed}");
            }
            //var a = Response.FromRaw(new byte[] { 0xa1, 0x7f, 0x02, 0x00, 0x52, 0x11 });
            Console.WriteLine($"");
        }
    }
}
