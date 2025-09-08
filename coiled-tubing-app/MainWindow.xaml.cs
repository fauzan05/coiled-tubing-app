using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace coiled_tubing_app
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Page page = new DashboardPage();
            contentFrame9.Navigate(page.GetType(), null, new EntranceNavigationTransitionInfo());
        }

        private void OpenLoginWindow(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Activate();
        }

        private void NavView_Navigate(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem selectedItem)
            {
                string navItemTag = selectedItem.Tag.ToString();
                Page page = null;

                switch (navItemTag)
                {
                    case "DashboardPage":
                        page = new DashboardPage();
                        break;

                    case "SensorPage":
                        page = new SensorPage();
                        break;
                }

                if (page != null)
                {
                    // contentFrame9 adalah <Frame x:Name="contentFrame9" /> di XAML
                    contentFrame9.Navigate(page.GetType(), null, new EntranceNavigationTransitionInfo());
                }
            }
        }
    }
}
