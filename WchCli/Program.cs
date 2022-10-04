using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WchDotNet;

namespace WchCli
{
    class Program
    {
        // <command, (desc, args<name, desc>, options<name, desc>, callback)>
        static readonly Dictionary<string, (string,  Func<string[], WchDevice, bool>)> SubCommands = new Dictionary<string, (string,  Func<string[], WchDevice, bool>)>
        {
            {"config" ,
                ("Config CFG register",  Command_Config)
            },
            {"eeprom" ,("Read EEPROM", Command_Config) },
            {"erase" ,("Erase flash", Command_Config) },
            {"flash" ,("Download to code flash and reset", Command_Flash) },
            {"help" ,("Print this message or the help of the given subcommand(s)", null) },
            {"info" ,("Get info about current connected chip", Command_Info) },
            {"reset" ,("Reset the target connected", Command_Reset) },
            {"unprotect" ,("Remove code flash protect(RDPR and WPR) and reset", Command_UnProtect) },
            {"verify" ,("Verify flash content", Command_Config) },
        };


        static void PrintHelp(bool less)
        {
            if (less)
            {
                Console.WriteLine("USAGE:");
                Console.WriteLine($"    {AppDomain.CurrentDomain.FriendlyName} <SUBCOMMAND>");
                Console.WriteLine();
                Console.WriteLine("For more information try --help");
                return;
            }

            Console.WriteLine("Command-line implementation of the WCHISPTool in .Net");
            Console.WriteLine("Built-in yaml from https://github.com/ch32-rs/wchisp");
            Console.WriteLine("");
            Console.WriteLine("USAGE:");
            Console.WriteLine($"    {AppDomain.CurrentDomain.FriendlyName} <SUBCOMMAND>");
            Console.WriteLine();
            Console.WriteLine("OPTIONS:");
            Console.WriteLine("    -h, --help    Print help information");
            Console.WriteLine();
            Console.WriteLine("SUBCOMMANDS:");
            foreach(var cmd in SubCommands)
                Console.WriteLine("    {0,-15} {1}", cmd.Key, cmd.Value.Item1);
            Console.WriteLine();
        }

        static int Main(string[] args)
        {
            if (args.Length == 0
                || args[0] == "-h"
                || args[0] == "--help"
                || args[0] == "help"
                )
            {
                PrintHelp(false);
                return 0;
            }

            if (!SubCommands.ContainsKey(args[0]))
            {
                Console.Error.WriteLine($"Error: Found argument '{args[0]}' which wasn't expected, or isn't valid in this context");
                PrintHelp(true);
                return 1;
            }

            ChipDB.LoadInternalChipFamilies();

            var devices = WchIsp.GetDevices();
            if (devices.Count() == 0)
            {
                Console.Error.WriteLine("Error: No WCH ISP USB device found");
                return 1;
            }
            var dev = devices.First();


            bool success = true;
            try
            {
                if (SubCommands.TryGetValue(args[0], out var f))
                {
                    success = f.Item2(args[1..], dev);
                }
            }catch(Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                success = false;
            }
            finally
            {
                dev.Dispose();
            }

            if (!success)
                return 1;

            return 0;
        }

        static bool Command_Info(string[] args, WchDevice device)
        {
            Console.WriteLine(device.DumpInfo());

            return true;
        }

        static bool Command_Flash(string[] args, WchDevice device)
        {
            bool no_erase = false;
            bool no_reset = false;
            bool no_verify = false;
            string path=string.Empty;

            if (args.Length == 0)
                return false;

            foreach (var arg in args)
            {
                if (arg.StartsWith('-'))
                {
                    if (arg.Contains('E') || arg == "--no-erase")
                        no_erase = true;
                    else if (arg.Contains('R') || arg == "--no-reset")
                        no_reset = true;
                    else if (arg.Contains('V') || arg == "--no-verify")
                        no_verify = true;
                    else
                    {
                        Console.Error.WriteLine($"Error: Unknown argument '{arg}'");
                        return false;
                    }  
                }
            }
            path = args.Last();

            byte[] raw = File.ReadAllBytes(path);

            if (raw.Length % 1024 != 0)
            {
                var raw_padding = raw.ToList();
                raw_padding.AddRange(new byte[1024 - (raw.Length % 1024)]);

                raw = raw_padding.ToArray();
            }

            Console.WriteLine($"no_erase: {no_erase} no_reset:{no_reset} no_verify:{no_verify} path:{path}");
            Console.WriteLine(device.DumpInfo());

            if (no_erase)
            {
                Console.WriteLine("Skipping erase");
            }
            else
            {
                var sectors = (raw.Length / Constants.SECTOR_SIZE) + 1;
                device.EraseCode((uint)sectors);
                Task.Delay(1000).Wait();
                Console.WriteLine("Erase done");
            }

            Console.WriteLine($"Firmware size: {raw.Length}");
            device.Flash(raw);
            Task.Delay(3000).Wait();

            if (no_verify)
            {
                Console.WriteLine("Skipping verify");
            }
            else
            {
                device.Verify(raw);
                Task.Delay(1000).Wait();
                Console.WriteLine("Verify OK");
            }

            if (no_reset)
            {
                Console.WriteLine("Skipping reset");
            }
            else
            {
                Console.WriteLine("Now reset device and skip any communication errors");
                device.Reset();
            }
            return true;
        }
        static bool Command_Erase(string[] args, WchDevice device)
        {
            device.EraseCode(device.Chip.flash_size/1024);

            return true;
        }
        static bool Command_Config(string[] args, WchDevice device)
        {

            return true;
        }
        static bool Command_UnProtect(string[] args, WchDevice device)
        {
            Console.WriteLine("Warning: Only applies to CH32F/CH32V devices for now");
            Console.WriteLine("Warning: Unprotect is deprected, use `config` to reset to default config");

            device.UnProtect(true);

            return true;
        }
        static bool Command_Reset(string[] args, WchDevice device)
        {
            device.Reset();
            return true;
        }
    }
}
