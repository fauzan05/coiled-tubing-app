using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Graphics;

namespace coiled_tubing_app;

public sealed partial class LoginWindow : Window
{
    public LoginWindow()
    {
        this.InitializeComponent();
        this.CenterWindow();
        this.Content = new LoginPage();
    }

    private void CenterWindow()
    {
        var area = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest)?.WorkArea;
        if (area == null) return;
        AppWindow.Move(new PointInt32((area.Value.Width - AppWindow.Size.Width) / 2, (area.Value.Height - AppWindow.Size.Height) / 2));
    }

}

