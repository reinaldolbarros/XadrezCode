using ChessMAUI.Views;

namespace ChessMAUI;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		Page root = AppState.Current.Auth.IsAuthenticated
			? new AppShell()
			: new LoginPage();

		var window = new Window(root)
		{
			MinimumWidth  = 480,
			MinimumHeight = 600
		};
		window.HandlerChanged += OnWindowHandlerChanged;
		return window;
	}

	private static void OnWindowHandlerChanged(object? sender, EventArgs e)
	{
#if WINDOWS
		if (sender is not Window mauiWindow) return;
		var native = mauiWindow.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
		if (native is null) return;

		// Garante que maximize/minimize/resize estejam habilitados no presenter nativo
		var handle = WinRT.Interop.WindowNative.GetWindowHandle(native);
		var winId  = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
		var appWin = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(winId);
		if (appWin.Presenter is Microsoft.UI.Windowing.OverlappedPresenter p)
		{
			p.IsResizable   = true;
			p.IsMaximizable = true;
			p.IsMinimizable = true;
		}
#endif
	}
}