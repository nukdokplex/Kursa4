using iText.IO.Font;
using iText.Kernel.Font;
using Kursa4.Entitities;
using Kursa4.Utils;
using System;
using System.Windows;

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

        public static PdfFont GetDefaultPdfFont()
        {
            byte[] fontContents = Static.ReadStreamFully(Static.GetResourceStream(new Uri("/Resources/Fonts/Roboto-Regular.ttf", UriKind.Relative)));

            var fontProgram = FontProgramFactory.CreateFont(fontContents);
            return PdfFontFactory.CreateFont(fontProgram, PdfEncodings.IDENTITY_H);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            DB.Dispose();
        }
    }
}