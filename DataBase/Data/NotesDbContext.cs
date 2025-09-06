using Microsoft.EntityFrameworkCore;
using DataBase.Models;

namespace DataBase.Data
{
    public class NotesDbContext : DbContext
    {
        private readonly string _dbPath;

        public NotesDbContext(string dbPath)
        {
            _dbPath = dbPath;
        }

        public DbSet<NoteModel> Notes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_dbPath}");
        }
    }
}