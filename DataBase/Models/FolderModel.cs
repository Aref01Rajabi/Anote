
namespace DataBase.Models
{
    public class FolderModel 
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ParentFolderId { get; set; }
    }
}
