using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using System.Reflection;
using Microsoft.Extensions.Configuration;


namespace Afkgame_maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

			builder.Services.AddTransient<MainPage>();

					var a = Assembly.GetExecutingAssembly();
		using var stream = a.GetManifestResourceStream("Afkgame_maui.settings.json");

		var config = new ConfigurationBuilder()
		.AddJsonStream(stream)
		.Build();

		builder.Configuration.AddConfiguration(config);


#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
