using DataBase.Data;
using DataBase.Interfaces;
using DataBase.Models;
using Microsoft.EntityFrameworkCore;

namespace DataBase.Services
{
    public class FolderService : IFolderService
    {
        private readonly string _dbPath;

        public FolderService(string dbPath)
        {
            _dbPath = dbPath;
            using var context = new NotesDbContext(_dbPath);
            context.Database.EnsureCreated();
        }

        public async Task<List<FolderModel>> GetAllAsync()
        {
            using var context = new NotesDbContext(_dbPath);
            return await context.Folders.ToListAsync();
        }

        public async Task<FolderModel?> GetByIdAsync(int id)
        {
            using var context = new NotesDbContext(_dbPath);
            return await context.Folders.FindAsync(id);
        }

        public async Task<List<FolderModel>> GetByParentIdAsync(int? parentId)
        {
            using var context = new NotesDbContext(_dbPath);
            return await context.Folders
                .Where(f => f.ParentFolderId == parentId)
                .ToListAsync();
        }

        public async Task AddAsync(FolderModel folder)
        {
            using var context = new NotesDbContext(_dbPath);
            await context.Folders.AddAsync(folder);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FolderModel folder)
        {
            using var context = new NotesDbContext(_dbPath);
            context.Folders.Update(folder);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var context = new NotesDbContext(_dbPath);
            var entity = await context.Folders.FindAsync(id);
            if (entity != null)
            {
                context.Folders.Remove(entity);
                await context.SaveChangesAsync();
            }
        }
    }
}
