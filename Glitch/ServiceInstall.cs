using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;

namespace Glitch
{
    [RunInstaller(true)]
    public partial class ServiceInstall : Installer
    {
        ServiceInstaller serviceInstaller;
        ServiceProcessInstaller processInstaller;
        public ServiceInstall()
        {
            InitializeComponent();
            serviceInstaller = new ServiceInstaller();
            processInstaller = new ServiceProcessInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "DchpServer";
            serviceInstaller.DisplayName = "DCHP-сервер";
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
