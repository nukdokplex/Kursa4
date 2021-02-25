using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Kursa4.Entitities;

namespace Kursa4
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>

    
    public partial class App : Application
    {
        public static Entitities.dbConnection DB;
        public static User? CurrentUser;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            DB = new dbConnection();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            DB.Dispose();
        }
    }
}
