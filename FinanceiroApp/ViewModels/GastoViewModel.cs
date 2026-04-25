using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceiroApp.Models;
using FinanceiroApp.Services;
using System.Collections.ObjectModel;

namespace FinanceiroApp.ViewModels;

public partial class UserConfig : ObservableObject
{
    [ObservableProperty]
    private bool isPremium;

    private static UserConfig? _instance;
    public static UserConfig Current => _instance ??= new UserConfig
    {
        IsPremium = Preferences.Get(nameof(IsPremium), false)
    };

    partial void OnIsPremiumChanged(bool value) =>
        Preferences.Set(nameof(IsPremium), value);
}

public class GastosPorMes : ObservableCollection<Gasto>
{
    public string Mes { get; }
    public decimal Total => this.Sum(g => g.Valor);
    public string TotalFormatado => Total.ToString("C2",
        new System.Globalization.CultureInfo("pt-BR"));

    public GastosPorMes(string mes, IEnumerable<Gasto> gastos) : base(gastos)
    {
        Mes = mes;
    }
}

public partial class GastoViewModel : ObservableObject
{
    private readonly DatabaseService _db;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SalvarCommand))]
    private string valorTexto = string.Empty;

    [ObservableProperty] private string descricao = string.Empty;
    [ObservableProperty] private string categoriaSelecionada = "🛒";
    [ObservableProperty] private bool isSaving;
    [ObservableProperty] private string? mensagemErro;
    [ObservableProperty] private ObservableCollection<GastosPorMes> gastosPorMes = [];
    [ObservableProperty] private string totalMesAtual = "R$ 0,00";
    [ObservableProperty] private bool isLoading;

    public UserConfig Config => UserConfig.Current;
    public bool ShowBanner => !Config.IsPremium;

    public List<string> Categorias { get; } =
    [
        "🛒", "🍔", "🚗", "🏠", "💊", "🎮",
        "👗", "📚", "✈️", "🎵", "💡", "🐾", "💰", "🏋️"
    ];

    public GastoViewModel(DatabaseService db)
    {
        _db = db;
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await _db.InitAsync();
        await CarregarGastosAsync();
    }

    [RelayCommand(CanExecute = nameof(PodeSalvar))]
    private async Task SalvarAsync()
    {
        if (!TryParseValor(out var valor))
        {
            MensagemErro = "Digite um valor numérico válido.";
            return;
        }

        IsSaving = true;
        MensagemErro = null;

        try
        {
            var gasto = new Gasto
            {
                Valor = valor,
                Descricao = Descricao.Trim(),
                Categoria = CategoriaSelecionada,
                Data = DateTime.Now
            };

            await _db.InserirGastoAsync(gasto);
            LimparFormulario();
            await CarregarGastosAsync();
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private void SelecionarCategoria(string categoria) =>
        CategoriaSelecionada = categoria;

    [RelayCommand]
    private async Task ExcluirGastoAsync(Gasto gasto)
    {
        await _db.ExcluirGastoAsync(gasto);
        await CarregarGastosAsync();
    }

    [RelayCommand]
    private async Task CarregarGastosAsync()
    {
        IsLoading = true;
        try
        {
            var todos = await _db.ObterTodosAsync();
            var grupos = todos
                .OrderByDescending(g => g.Data)
                .GroupBy(g => g.ChaveMes)
                .Select(g => new GastosPorMes(g.Key, g))
                .ToList();

            GastosPorMes = new ObservableCollection<GastosPorMes>(grupos);

            var mesAtual = DateTime.Now.ToString("MMMM yyyy",
                new System.Globalization.CultureInfo("pt-BR"));

            TotalMesAtual = todos
                .Where(g => g.ChaveMes.Equals(mesAtual, StringComparison.OrdinalIgnoreCase))
                .Sum(g => g.Valor)
                .ToString("C2", new System.Globalization.CultureInfo("pt-BR"));
        }
        finally { IsLoading = false; }
    }

    [RelayCommand]
    private async Task IrParaPremiumAsync() =>
        await Shell.Current.GoToAsync("//PremiumPage");

    private bool PodeSalvar() => !string.IsNullOrWhiteSpace(ValorTexto);

    private bool TryParseValor(out decimal valor)
    {
        var texto = ValorTexto.Replace(",", ".").Trim();
        return decimal.TryParse(texto,
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out valor) && valor > 0;
    }

    private void LimparFormulario()
    {
        ValorTexto = string.Empty;
        Descricao = string.Empty;
        CategoriaSelecionada = "🛒";
    }
}
