using DataBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase.Interfaces
{
    public interface IFolderService
    {
        Task<List<FolderModel>> GetAllAsync();
        Task<FolderModel?> GetByIdAsync(int id);
        Task AddAsync(FolderModel folder);
        Task UpdateAsync(FolderModel folder);
        Task DeleteAsync(int id);
    }
}
