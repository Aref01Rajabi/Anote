using DataBase.Data;
using DataBase.Models;
using Microsoft.EntityFrameworkCore;
using DataBase.Interfaces;


namespace DataBase.Services
{
    public class NoteService : INoteService
    {
        private readonly string _dbPath;

        public NoteService(string dbPath)
        {
            _dbPath = dbPath;
            using var context = new NotesDbContext(_dbPath);
            context.Database.EnsureCreated();
        }

        public async Task<List<NoteModel>> GetAllAsync()
        {
            using var context = new NotesDbContext(_dbPath);
            return await context.Notes.OrderByDescending(n => n.CreatedAt).ToListAsync();
        }

        public async Task<NoteModel?> GetByIdAsync(int id)
        {
            using var context = new NotesDbContext(_dbPath);
            return await context.Notes.FindAsync(id);
        }

        public async Task<List<NoteModel>> GetByFolderIdAsync(int? folderId)
        {
            using var context = new NotesDbContext(_dbPath);
            return await context.Notes
                .Where(n => n.FolderId == folderId)
                .ToListAsync();
        }

        public async Task AddAsync(NoteModel note)
        {
            using var context = new NotesDbContext(_dbPath);
            await context.Notes.AddAsync(note);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(NoteModel note)
        {
            using var context = new NotesDbContext(_dbPath);
            context.Notes.Update(note);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var context = new NotesDbContext(_dbPath);
            var entity = await context.Notes.FindAsync(id);
            if (entity != null)
            {
                context.Notes.Remove(entity);
                await context.SaveChangesAsync();
            }
        }
    }

}
