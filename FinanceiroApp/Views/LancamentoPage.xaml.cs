using FinanceiroApp.ViewModels;

namespace FinanceiroApp.Views;

public partial class LancamentoPage : ContentPage
{
    public LancamentoPage(GastoViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ValorEntry.Focus();
    }
}
