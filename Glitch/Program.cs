using System.ServiceProcess;

namespace Glitch
{
    class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new GlitchService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
