using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wchisp.Devices;

namespace wchisp
{
    public class ChipDB
    {
        private List<IWCHDevice> DeviceTypes = new List<IWCHDevice>();

        public ChipDB()
        {
            var type = typeof(IWCHDevice);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);

            foreach(var t in types)
            {
                DeviceTypes.Add((IWCHDevice)Activator.CreateInstance(t));
            }
        }

        public WCHDeviceVariant FindChip(byte chip_id, byte device_type)
        {
            var family = DeviceTypes.Find(f => f.DeviceType == device_type);
            if (family == null)
            {
                Console.WriteLine($"Device type of 0x{device_type:02x} not found");
                return null;
            }

            var chip_variant = family.variants.ToList().Find(c => c.ChipId == chip_id || (c.AltChipIds?.Contains(chip_id) ?? false));
            if (chip_variant == null)
            {
                Console.WriteLine($"Cannot find chip with id 0x{chip_id:02x} device_type 0x{device_type:02x}");
                return null;
            }

            return chip_variant;
        }
    }
}
