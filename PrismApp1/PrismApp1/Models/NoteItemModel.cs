using DataBase.Models;

public class NoteItemModel : BindableBase
{
    //public NoteModel Note { get; }


    //public NoteItemModel(NoteModel note)
    //{
    //    Note = note;
    //    return Note;
    //}

    public int Id { get; set; }
    public string Title { get; set; }
    public string Content   { get; set; }
    public DateTime CreatedAt { get; set; }
    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

}
