using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace coiled_tubing_app
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;

        // Static property untuk akses MainWindow dari service
        public static Window MainWindow { get; private set; } = null!;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            // Add global exception handling
            this.UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // Log the exception (in production, use proper logging)
            System.Diagnostics.Debug.WriteLine($"Unhandled exception: {e.Exception}");

            // Mark as handled to prevent crash
            e.Handled = true;

            // Optionally show user-friendly message
            if (_window != null)
            {
                // Could show a content dialog here
            }
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            try
            {
                _window = new MainWindow();
                MainWindow = _window;  // Set static property
                _window.Activate();
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                System.Diagnostics.Debug.WriteLine($"COM Exception during launch: {ex}");

                // Try to create a minimal window
                try
                {
                    _window = new Window();
                    MainWindow = _window;
                    _window.Activate();
                }
                catch
                {
                    // Last resort - exit gracefully
                    Current.Exit();
                }
            }
        }
    }
}
