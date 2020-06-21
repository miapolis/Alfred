using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Alfred.Commands
{
	public class DefaultCommands : BaseCommandModule
	{
		#region IDS AND CONFIGURATIONS 
		
		public static ulong GeneralId { get; } = 688079932282241134; // FOR ALL OF THESE EDIT THE IDS TO MATCH YOUR GUILD AND CHANNEL
		public static ulong BotSpamId { get; } = 575282625871347722;

		public static ulong TestGeneralId { get; } = 723544396485754912;

		public static ulong SpamId { get; } = 577460633092620288;
		public static ulong HoglinSpamId { get; } = 680231622175752193;
		public static ulong PingPollBotId { get; } = 711573841222303774;
		public static ulong SuggestionsId { get; } = 592126471636779023;
		public static ulong EventSuggestionsId { get; } = 662047431113965588;
		public static ulong ProjectsId { get; } = 688538129644126272;
		public static ulong QuotesId { get; } = 707029272753340416;
		public static ulong CoordsId { get; } = 589830597875204107;
		public static ulong MinecraftNewsId { get; } = 591444895525502983;
		public static ulong MinecraftNewsDiscussionId { get; } = 705091216739401729;
		public static ulong MemesId { get; } = 672612901881905175;
		public static ulong SkyblockId { get; } = 703765487930179756;
		public static ulong ScreenshotsId { get; } = 618520829755654144;
		
		#endregion

		public static List<DiscordMember> registeredSenders = new List<DiscordMember>();

		protected bool hasBeenStarted = false;

		[Command("start")]
		public async Task Startup (CommandContext ctx)
		{
			if (hasBeenStarted) { return; }

			hasBeenStarted = true;

			var interactivity = ctx.Client.GetInteractivity();

			while (true)
			{
				var message = await interactivity.WaitForMessageAsync(x => x.Channel.Id == ReturnChannelId("g") && x.MentionedUsers.Count == 1);

				if (message.Result != null)
				{
					if (message.Result.MentionedUsers.First().Id == 713398514700517498)
					{
						await message.Result.Channel.SendMessageAsync("**Loading response...**").ConfigureAwait(false);

						foreach (var registeredMember in registeredSenders)
						{
							var dm = await registeredMember.CreateDmChannelAsync();

							if (message.Result.Attachments.Count == 0)
							{
								await dm.SendMessageAsync($"**{message.Result.Author.Username} said:** {message.Result.Content}");
							}
							else
							{
								if (message.Result.Attachments.Count > 1)
								{
									await dm.SendMessageAsync($"**{message.Result.Author.Username} said:** {message.Result.Content}\n{message.Result.Attachments.First().Url}\n\n`Message from Alfred:` There is more attached. Please view full message in #{message.Result.Channel.Name}");
								}
								else
								{
									await dm.SendMessageAsync($"**{message.Result.Author.Username} said:** {message.Result.Content}\n{message.Result.Attachments.First().Url}");
								}
							}
						}
					}
				}
			}
		}

		[Command("register")]
		public async Task Register (CommandContext ctx)
		{
			if (ctx.User.Id == 508420859476836364 || ctx.User.Id == 469228159082954772)
			{
				if (hasBeenStarted == false) { await ctx.Channel.SendMessageAsync("Alfred hasn't been started yet! Please use +start in the Alfred.net server!"); return; }

				if (ctx.Member == null) { await ctx.Channel.SendMessageAsync("Please Register in the Aflred.net server!"); return; }

				var dm = await ctx.Member.CreateDmChannelAsync();

				if (registeredSenders.Contains(ctx.Member))
				{
					await dm.SendMessageAsync("You are already registered!");

					return;
				}

				registeredSenders.Add(ctx.Member);

				await dm.SendMessageAsync("You have been registered. Use +unregister to unregister.");
			}
		}

		[Command("vr")]
		public async Task ViewRegistered (CommandContext ctx)
		{
			if (ctx.User.Id == 508420859476836364 || ctx.User.Id == 469228159082954772)
			{
				string s = string.Empty;

				foreach (var reg in registeredSenders)
				{
					s += $"\n{reg.Username}";
				}

				await ctx.Channel.SendMessageAsync($"**REGISTERED:** {s}");
			}
		}

		[Command("unregister")]
		public async Task UnRegister(CommandContext ctx)
		{
			if (ctx.User.Id == 508420859476836364 || ctx.User.Id == 469228159082954772)
			{
				if (hasBeenStarted == false) { await ctx.Channel.SendMessageAsync("Alfred hasn't been started yet! Please use +start in the Alfred.net server!"); return; }

				if (ctx.Member == null) { await ctx.Channel.SendMessageAsync("Please Register in the Aflred.net server!"); return; }

				var dm = await ctx.Member.CreateDmChannelAsync();

				if (!(registeredSenders.Contains(ctx.Member)))
				{
					await dm.SendMessageAsync("You aren't registered yet!");

					return;
				}

				registeredSenders.Remove(ctx.Member);

				await dm.SendMessageAsync("You have been unregistered. Use +register to register.");
			}
		}

		[Command("l")]
		public async Task MentionLoading (CommandContext ctx, string channelShortPrefix)
		{
			if (ctx.User.Id == 508420859476836364 || ctx.User.Id == 469228159082954772)
			{
				var cId = ReturnChannelId(channelShortPrefix);
				var guild = await GetRealGuildAsync(ctx).ConfigureAwait(false);

				var channel = guild.GetChannel(cId);

				await channel.SendMessageAsync("**Loading response...**").ConfigureAwait(false);
			}		
		}

		[Command("c")]
		public async Task LoadPredefinedResponse (CommandContext ctx, string channelShortPrefix, string messageType)
		{
			if (ctx.User.Id == 508420859476836364 || ctx.User.Id == 469228159082954772)
			{
				var cId = ReturnChannelId(channelShortPrefix);
				var guild = await GetRealGuildAsync(ctx).ConfigureAwait(false);

				var channel = guild.GetChannel(cId);

				var message = GeneratePredefinedMessage(messageType);

				if (!message.Equals(""))
				{
					await channel.SendMessageAsync(message).ConfigureAwait(false);
				}
				else
				{
					await ctx.Channel.SendMessageAsync("Try again").ConfigureAwait(false);
				}
			}
		}

		[Command("m")]
		public async Task LoadCustomResponse (CommandContext ctx, string channelShortPrefix, params string[] message)
		{
			if (ctx.User.Id == 508420859476836364 || ctx.User.Id == 469228159082954772)
			{
				if (message.Length == 0)
				{
					await ctx.Channel.SendMessageAsync("Please try again. If you are trying to send an image, copy the link and send it that way.");

					return;
				}

				var cId = ReturnChannelId(channelShortPrefix);
				var guild = await GetRealGuildAsync(ctx).ConfigureAwait(false);

				var channel = guild.GetChannel(cId);

				var response = string.Join(" ", message);

				await channel.SendMessageAsync(response);
			}
		}

		private static ulong ReturnChannelId(string abrv)
		{
			switch (abrv)
			{
				case "g":
					return GeneralId;

				case "bs":
					return BotSpamId;

				case "sp":
					return SpamId;

				case "hs":
					return HoglinSpamId;

				case "ps":
					return PingPollBotId;

				case "su":
					return SuggestionsId;

				case "esu":
					return EventSuggestionsId;

				case "pr":
					return ProjectsId;

				case "qu":
					return QuotesId;

				case "co":
					return CoordsId;

				case "mn":
					return MinecraftNewsId;

				case "mnd":
					return MinecraftNewsDiscussionId;

				case "me":
					return MemesId;

				case "sk":
					return SkyblockId;

				case "sc":
					return ScreenshotsId;

				default:
					return 0;
			}
		}

		private static async Task<DiscordGuild> GetOasisGuildAsync(CommandContext ctx)
		{
			return await ctx.Client.GetGuildAsync(688079169317634061).ConfigureAwait(false);
		}

		private static async Task<DiscordGuild> GetRealGuildAsync(CommandContext ctx)
		{
			return await ctx.Client.GetGuildAsync(688079169317634061).ConfigureAwait(false);
		}

		private static async Task<DiscordGuild> GetTestGuildAsync(CommandContext ctx)
		{
			return await ctx.Client.GetGuildAsync(723544396057935952).ConfigureAwait(false);
		}

		private string GeneratePredefinedMessage (string messageType)
		{
			switch (messageType)
			{
				case "ni":

					return "I'm sorry, I don't have enough information to determine that!";

				case "nu":

					var rn = new Random();

					var nu = rn.Next(3);

					switch (nu)
					{
						case 1:
							return "I'm sorry I do not understand.";
						case 2:
							return "Sorry, please try again.";
						case 3:
							return "I did not understand that. Please try again.";
					}

					break;

				case "hi":

					var rnd = new Random();

					var num = rnd.Next(6);

					switch (num)
					{
						case 1:
							return "Hello!";
						case 2:
							return "Greetings!";
						case 3:
							return "Hi!";
						case 4:
							return "How do ya?";
						case 5:
							return "Lovely day isn't it?";
						case 6:
							return "Greetings, traveller!";
					}

					break;

				case "sry":

					var rnda = new Random();

					var numa = rnda.Next(3);

					switch (numa)
					{
						case 1:
							return "My apologies.";
						case 2:
							return "I'm very sorry.";
						case 3:
							return "Sorry.";
					}

					break;

				case "y":

					return "Yes.";

				case "n":

					return "No.";

				case "haha":

					var rndb = new Random();

					var numb = rndb.Next(5);

					switch (numb)
					{
						case 1:
							return "Hilarious.";
						case 2:
							return "Very funny.";
						case 3:
							return "I do not have a sense of humor, but I can see how one might find that funny.";
						case 4:
							return "I find that rather funny.";
						case 5:
							return "Humor, am I right?";
					}

					break;

				case "info":

					return "This bot was created by **Darius168** and the Pipeline AI Framework developed by the Microsoft Corporation in 2008.";

				case "se": //Session Expired

					var rndc = new Random();

					var numc = rndc.Next(2);

					switch (numc)
					{
						case 1:
							return "Sorry, your session has expired.";
						case 2:
							return "Your session automatically ended.";
					}

					break;
			}

			return string.Empty;
		}
	}
}
