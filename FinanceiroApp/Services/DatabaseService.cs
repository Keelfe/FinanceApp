using SQLite;
using FinanceiroApp.Models;

namespace FinanceiroApp.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _conn;
    private readonly string _dbPath;

    public DatabaseService()
    {
        _dbPath = Path.Combine(FileSystem.AppDataDirectory, "financeiro.db3");
    }

    public async Task InitAsync()
    {
        if (_conn is not null) return;
        _conn = new SQLiteAsyncConnection(_dbPath);
        await _conn.CreateTableAsync<Gasto>();
    }

    public Task<int> InserirGastoAsync(Gasto gasto) => _conn!.InsertAsync(gasto);
    public Task<int> ExcluirGastoAsync(Gasto gasto) => _conn!.DeleteAsync(gasto);
    public Task<List<Gasto>> ObterTodosAsync() => _conn!.Table<Gasto>().ToListAsync();
    public async Task<int> ContarGastosAsync() => await _conn!.Table<Gasto>().CountAsync();
}
