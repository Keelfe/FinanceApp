using SQLite;

namespace FinanceiroApp.Models;

[Table("Gastos")]
public class Gasto
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public decimal Valor { get; set; }

    [MaxLength(200)]
    public string Descricao { get; set; } = string.Empty;

    [NotNull, MaxLength(50)]
    public string Categoria { get; set; } = "🛒";

    [NotNull]
    public DateTime Data { get; set; } = DateTime.Now;

    [Ignore]
    public string ValorFormatado => Valor.ToString("C2",
        new System.Globalization.CultureInfo("pt-BR"));

    [Ignore]
    public string DataFormatada => Data.ToString("dd/MM/yyyy");

    [Ignore]
    public string ChaveMes => Data.ToString("MMMM yyyy",
        new System.Globalization.CultureInfo("pt-BR"));
}
