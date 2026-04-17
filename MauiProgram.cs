using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace ChessMAUI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.ConfigureMauiHandlers(handlers =>
			{
#if ANDROID
				// Remove sublinha/borda padrão dos Entry no Android
				handlers.AddHandler<Microsoft.Maui.Controls.Entry,
					Microsoft.Maui.Controls.Handlers.EntryHandler>();
#endif
			});

		builder.AddAudio();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
