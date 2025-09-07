
namespace DataBase.Models
{
    public class FolderModel 
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? ParentFolderId { get; set; }

        //public int Id { get; set; }

        //public string Name { get; set; }

        //public int? ParentFolderId { get; set; }

        //public string NoteIdsSerialized { get; set; } = ""; 

        //public string ChildFolderIdsSerialized { get; set; } = "";

        //public List<int> NoteIds
        //{
        //    get => string.IsNullOrWhiteSpace(NoteIdsSerialized) ? new List<int>() : NoteIdsSerialized.Split(',').Select(int.Parse).ToList();
        //    set => NoteIdsSerialized = string.Join(",", value);
        //}

        //public List<int> ChildFolderIds
        //{
        //    get => string.IsNullOrWhiteSpace(ChildFolderIdsSerialized) ? new List<int>() : ChildFolderIdsSerialized.Split(',').Select(int.Parse).ToList();
        //    set => ChildFolderIdsSerialized = string.Join(",", value);
        //}
    }
}
