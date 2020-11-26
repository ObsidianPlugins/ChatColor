using Obsidian.API.Plugins;
using Obsidian.API.Plugins.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatColor
{
    public class Globals
    {
        public static Config Config { get; set; }
        public static IFileReader FileReader { get; set; }
        public static IFileWriter FileWriter { get; set; }
        public static string WorkingDirectory { get; set; }
        public static IPluginInfo PluginInfo { get; set; }
    }
}
