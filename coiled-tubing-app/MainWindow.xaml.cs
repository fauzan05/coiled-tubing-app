using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System.Linq;

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

            // Navigate to default page
            // Page page = new DashboardPage();
            Page page = new SensorPage();
            contentFrame9.Navigate(page.GetType(), null, new EntranceNavigationTransitionInfo());

            // Set default selected item di NavigationView
            //SetSelectedNavigationItem("DashboardPage");
            SetSelectedNavigationItem("SensorPage");
        }

        private void SetSelectedNavigationItem(string tag)
        {
            // Cari item di MenuItems
            var menuItem = nvSample9.MenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(item => item.Tag?.ToString() == tag);

            if (menuItem != null)
            {
                nvSample9.SelectedItem = menuItem;
                return;
            }

            // Jika tidak ditemukan di MenuItems, coba di FooterMenuItems
            var footerItem = nvSample9.FooterMenuItems
                .OfType<NavigationViewItem>()
                .FirstOrDefault(item => item.Tag?.ToString() == tag);

            if (footerItem != null)
            {
                nvSample9.SelectedItem = footerItem;
            }
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
