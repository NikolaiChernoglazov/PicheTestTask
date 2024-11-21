using BankingApp.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankingApp.DataAccess;

public class BankingDbContext : DbContext
{
    public DbSet<DbAccount> Accounts { get; set; }

    private string DbPath { get; }
    
    public BankingDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "banking.db");
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
} 