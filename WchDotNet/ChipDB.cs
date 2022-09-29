using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using WchDotNet.Devices;

namespace WchDotNet
{
    public static class ChipDB
    {
        private static Dictionary<string, ChipFamily> ChipFamilies = new Dictionary<string, ChipFamily>();

        public static void Clear()
        {
            ChipFamilies.Clear();
        }

        private static byte[] ReadEmbeddedFile(string file)
        {
            var assembly = Assembly.GetExecutingAssembly();
            byte[] buffer;
            using (var stream = assembly.GetManifestResourceStream(file))
            {
                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
            }
            return buffer;
        }

        /// <summary>
        /// Load all chip family descriptions from internal
        /// </summary>
        public static void LoadInternalChipFamilies()
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (var file in assembly.GetManifestResourceNames())
            {

                if (!file.StartsWith($"{assembly.GetName().Name}.Devices"))
                {
                    continue;
                }
                var yaml = Encoding.UTF8.GetString(ReadEmbeddedFile(file));
                LoadChipFamily(yaml);
            }
        }

        /// <summary>
        /// Load or update chip family description from string of yaml
        /// </summary>
        /// <param name="yaml"></param>
        public static void LoadChipFamily(string yaml)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .WithTypeConverter(new DeviceYamlConverter())
                .Build();

            var chipFamily = deserializer.Deserialize<ChipFamily>(yaml);
            if (ChipFamilies.ContainsKey(chipFamily.name))
            {
                // Overwrite exist
                ChipFamilies[chipFamily.name] = chipFamily;
            }
            else
            {
                ChipFamilies.Add(chipFamily.name, chipFamily);
            }
            PatchChips(chipFamily);
        }

        private static void PatchChips(ChipFamily chipFamily)
        {
            foreach(Chip chip in chipFamily.variants)
            {
                chip.family = chipFamily;
                chip.mcu_type = chipFamily.mcu_type;
                chip.device_type = chipFamily.device_type;
            }
        }

        /// <summary>
        /// Find a chip by chip id and chip family device type
        /// </summary>
        /// <param name="chip_id"></param>
        /// <param name="device_type"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Chip FindChip(byte chip_id, byte device_type)
        {
            var chipFamily = ChipFamilies.First(o => o.Value.device_type == device_type).Value;
            if (chipFamily == null)
            {
                throw new Exception($"Cannot find a chip family with device_type: 0x{device_type:02x}");
            }
            var variants = chipFamily.variants;

            var chip = variants.First(o => o.chip_id == chip_id || (o.alt_chip_ids?.Contains(chip_id) ?? false));
            if (chip == null)
            {
                throw new Exception($"The chip family '{chipFamily.name}' does not contain a chip with chip_id: 0x{device_type:02x}");
            }
            return chip;
        }

        /// <summary>
        /// Return all loaded chip families
        /// </summary>
        /// <returns></returns>
        public static IDictionary<string, ChipFamily> GetChipFamilies()
        {
            return ChipFamilies;
        }
    }
}
