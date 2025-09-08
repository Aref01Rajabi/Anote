using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBase.Models;

namespace DataBase.Interfaces
{
    public interface INoteService
    {
        Task<List<NoteModel>> GetAllAsync();
        Task<NoteModel?> GetByIdAsync(int id);
        Task<List<NoteModel>> GetByFolderIdAsync(int? folderId);
        Task AddAsync(NoteModel note);
        Task UpdateAsync(NoteModel note);
        Task DeleteAsync(int id);
    }

}
