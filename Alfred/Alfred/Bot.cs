using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Alfred.Commands;
using DSharpPlus.Interactivity;

namespace Alfred
{
	public class Bot
	{
		public DiscordClient Client { get; private set; }
		public CommandsNextExtension Commands { get; private set; }

		public async Task RunAsync ()
		{
			var json = string.Empty;

			using (var fs = File.OpenRead("config.json"))
			using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
				json = await sr.ReadToEndAsync().ConfigureAwait(false);

			var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

			var config = new DiscordConfiguration
			{
				Token = configJson.Token,
				TokenType = TokenType.Bot,
				AutoReconnect = true,
				LogLevel = LogLevel.Debug,
				UseInternalLogHandler = true
			};

			Client = new DiscordClient(config);

			Client.Ready += OnClientReady;

			Client.UseInteractivity(new InteractivityConfiguration
			{
				Timeout = TimeSpan.FromMinutes(2)
			});

			var commandsConfig = new CommandsNextConfiguration
			{
				StringPrefixes = new string[] { configJson.Prefix },
				EnableMentionPrefix = true,
				EnableDefaultHelp = false,
				EnableDms = true
			};

			Commands = Client.UseCommandsNext(commandsConfig);

			Commands.RegisterCommands<DefaultCommands>();

			await Client.ConnectAsync();

			await Task.Delay(-1);
		}

		private Task OnClientReady (ReadyEventArgs e)
		{
			return Task.CompletedTask;
		}
	}
}
