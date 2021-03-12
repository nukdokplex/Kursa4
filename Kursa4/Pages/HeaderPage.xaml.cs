using System.Windows;
using System.Windows.Controls;

namespace Kursa4.Pages
{
    /// <summary>
    /// Логика взаимодействия для HeaderPage.xaml
    /// </summary>
    public partial class HeaderPage : Page
    {
        protected Frame ControllingFrame;

        public string HeaderTitle
        {
            get
            {
                return TitleTextBlock.Text;
            }

            set
            {
                TitleTextBlock.Text = value;
            }
        }

        public HeaderPage(Frame controllingFrame)
        {
            InitializeComponent();

            ControllingFrame = controllingFrame;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BackButton.Click += BackButton_Click;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (ControllingFrame.CanGoBack)
            {
                ControllingFrame.GoBack();
            }
        }
    }
}