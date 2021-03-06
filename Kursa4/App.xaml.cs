using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;
using Kursa4.Entitities;
using Kursa4.Utils;

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

            //Unpacking fonts, used by iText
            //Now reading list of font files stored in Resources/Fonts/FontList.txt

            var resourceStream = Application.GetResourceStream(new Uri("pack://application:,,,/Resources/Fonts/FontList.txt"));
            List<string> fonts = new List<string>();

            using (var reader = new StreamReader(resourceStream.Stream))
            {
                while (!reader.EndOfStream)
                {
                    fonts.Add(reader.ReadLine());
                }    
            }
            
            if (!Directory.Exists("Fonts"))
            {
                Directory.CreateDirectory("Fonts");
            }

            
            
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            DB.Dispose();
        }
    }
}
