using CommunityToolkit.Maui;
using FinanceiroApp.Services;
using FinanceiroApp.ViewModels;
using FinanceiroApp.Views;
using Microsoft.Extensions.Logging;

namespace FinanceiroApp;

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

        // ── Serviços ──────────────────────────────────────────────
        builder.Services.AddSingleton<DatabaseService>();

        // ── ViewModels ────────────────────────────────────────────
        builder.Services.AddTransient<GastoViewModel>();

        // ── Pages ─────────────────────────────────────────────────
        builder.Services.AddTransient<LancamentoPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
