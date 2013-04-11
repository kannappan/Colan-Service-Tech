using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace DailyRecap
{
    [RunInstaller(true)]
    public partial class ProgramInstaller : System.Configuration.Install.Installer
    {
        private System.ServiceProcess.ServiceInstaller serviceInstaller;
        private System.ServiceProcess.ServiceProcessInstaller
                serviceProcessInstaller;

        public ProgramInstaller()
        {
            InitializeComponent();
        }
    }
}
