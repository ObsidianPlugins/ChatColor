using Newtonsoft.Json;
using Obsidian.API;
using Obsidian.API.Events;
using Obsidian.API.Plugins;
using Obsidian.API.Plugins.Services;
using Obsidian.CommandFramework;
using Obsidian.CommandFramework.Attributes;
using Obsidian.CommandFramework.Entities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ChatColor
{
    [Plugin(Name = "ChatColor", Version = "0.1", ProjectUrl = "https://github.com/ObsidianPlugins/ChatColor",
        Authors = "Roxxel")]
    public class Plugin : PluginBase
    {
        [Inject] public ILogger Logger { get; set; }
        [Inject] public IFileReader FileReader { get; set; }
        [Inject] public IFileWriter FileWriter { get; set; }

        public async Task OnLoad(IServer server)
        {
            Globals.FileReader = FileReader;
            Globals.FileWriter = FileWriter;
            Globals.WorkingDirectory = Info.Name;
            Globals.PluginInfo = this.Info;


            if (FileWriter.IsUsable)
            {
                string path = Path.Combine("ChatColor", "config.json");
                if (!FileWriter.FileExists(path))
                {
                    await FileWriter.WriteAllTextAsync(path, JsonConvert.SerializeObject(new Config(), Formatting.Indented));
                    Logger.Log("&aNew config file was generated, please fill it up and reload plugin. &d/cc reload");
                }
                try
                {
                    Globals.Config = JsonConvert.DeserializeObject<Config>(FileReader.ReadAllText(path));

                }
                catch (Exception e)
                {
                    Logger.LogError($"&cThere was an error while reloading plugin:\n{e.Message}\n{e.StackTrace}");
                    Globals.Config = new Config();
                }
            }
            else
            {
                Logger.LogError("&File writer isn't usable");
            }
            server.RegisterCommandClass<MainCommands>();
            Logger.Log($"Loaded {Info.Name}!");
            await Task.CompletedTask;
        }


        public async Task OnIncomingChatMessage(IncomingChatMessageEventArgs args)
        {
            if (Globals.Config != null && Globals.Config.Enabled)
            {
                args.Cancel = true;
                foreach (var player in args.Server.Players)
                {
                    await player.SendMessageAsync(Globals.Config.ChatFormat
                    .Replace("{PLAYER}", args.Player.Username)
                    .Replace("{MESSAGE}", args.Message));
                }
            }
            await Task.CompletedTask;
        }

        
    }
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
                    string path = Path.Combine("ChatColor", "config.json");
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
