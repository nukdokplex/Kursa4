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
using iText.IO.Font;
using iText.Kernel.Font;
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
        public static PdfFont DefaultPdfFont;

        protected override void OnStartup(StartupEventArgs e)
        {
            InitializeiTextFonts();
            base.OnStartup(e);
            DB = new dbConnection();

        }

        protected void InitializeiTextFonts()
        {
            byte[] fontContents = Static.ReadStreamFully(Static.GetResourceStream(new Uri("/Resources/Fonts/Roboto-Regular.ttf", UriKind.Relative)));

            var fontProgram = FontProgramFactory.CreateFont(fontContents);
            DefaultPdfFont = PdfFontFactory.CreateFont(fontProgram, PdfEncodings.IDENTITY_H);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            DB.Dispose();
        }
    }
}
