using Newtonsoft.Json;
using Obsidian.CommandFramework;
using Obsidian.CommandFramework.Attributes;
using Obsidian.CommandFramework.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatColor
{
    public class MainCommands : BaseCommandClass
    {
        [CommandGroup("cc")]
        public class ChatColorCommands
        {
            [Command("reload")]
            [RequirePermission(PermissionCheckType.All, true, "chatcolor.reload")]
            public async Task ReloadConfig(ObsidianContext ctx)
            {

                if (Globals.FileWriter.IsUsable)
                {
                    string path = Path.Combine(Globals.WorkingDirectory, "config.json");
                    if (!Globals.FileWriter.FileExists(path))
                    {
                        await Globals.FileWriter.WriteAllTextAsync(path, JsonConvert.SerializeObject(new Config(), Formatting.Indented));
                        await ctx.Player.SendMessageAsync("&aNew config file was generated, please fill it up and reload plugin. &d/cc reload");
                    }
                    try
                    {
                        Globals.Config = JsonConvert.DeserializeObject<Config>(Globals.FileReader.ReadAllText(path));
                        if (Globals.FileWriter.FileExists(path))
                            await ctx.Player.SendMessageAsync("&aConfig reloaded successfully");
                    }
                    catch (Exception e)
                    {
                        await ctx.Player.SendMessageAsync($"&cThere was an error while reloading plugin:\n{e.Message}\n{e.StackTrace}");
                        Globals.Config = new Config();
                    }
                }
                else
                {
                    await ctx.Player.SendMessageAsync("&cFile writer isn't usable");
                }
            }

            [GroupCommand]
            [RequirePermission(PermissionCheckType.All, true, "chatcolor.main")]
            public async Task ChatColor(ObsidianContext ctx)
            {
                var chatFormat = Globals.Config.ChatFormat
                    .Replace("{PLAYER}", ctx.Player.Username)
                    .Replace("{MESSAGE}", "Woop dee doo");

                await ctx.Player.SendMessageAsync($"&dChatColor &av{Globals.PluginInfo.Version}\n" +
                    $"&bCurrent chat format:&r {chatFormat}");
            }
        }
    }
}
