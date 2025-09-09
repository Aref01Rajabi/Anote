using Prism.Mvvm;
using Prism.Navigation;
using Prism.Commands;
using DataBase.Models;
using DataBase.Interfaces;
namespace PrismApp1.ViewModels
{
    public class NoteEditorPageViewModel : BindableBase, INavigationAware
    {
        private readonly INoteService _noteService;
        private readonly INavigationService _navigationService;
        private int? _folderId;

        private bool _isEditable = true;
        public bool IsEditable
        {
            get => _isEditable;
            set => SetProperty(ref _isEditable, value);
        }
        public NoteModel Note { get; set; }
        public NoteModel OriginalNote { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DelegateCommand BackCommand { get; }
        public DelegateCommand ReadModeCommand { get; }
        public NoteEditorPageViewModel(INavigationService navigationService, INoteService noteService)
        {
            _navigationService = navigationService;
            _noteService = noteService;
            BackCommand = new DelegateCommand(Back);
            ReadModeCommand = new DelegateCommand(ReadMode);
            IsEditable = false;
        }

        private void ReadMode()
        {
            IsEditable = !IsEditable;
        }

        private void Back()
        {
            _navigationService.NavigateAsync("MainPage", new NavigationParameters { { "folderId", _folderId } });
        }
        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("note"))
            {
                OriginalNote = parameters.GetValue<NoteModel>("note");
                Title = OriginalNote.Title; Content = OriginalNote.Content;
                RaisePropertyChanged(nameof(Title)); RaisePropertyChanged(nameof(Content));
            }
            if (parameters.ContainsKey("folderId"))
                _folderId = parameters.GetValue<int?>("folderId");
        }
        private async void SaveNote()
        {
            if (string.IsNullOrWhiteSpace(Title) && string.IsNullOrWhiteSpace(Content))
            { return; }


            var finalTitle = string.IsNullOrWhiteSpace(Title) ?
                Content.Split(new[] { '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim() : Title.Trim();


            if (string.IsNullOrWhiteSpace(finalTitle))
            { return; }


            if (OriginalNote != null)
            {
                OriginalNote.Title = finalTitle;
                OriginalNote.Content = Content?.Trim();
                await _noteService.UpdateAsync(OriginalNote);
            }
            else { var newNote = new NoteModel
            {
                Title = finalTitle, Content = Content?.Trim(),
                FolderId = _folderId, CreatedAt = DateTime.Now };
                await _noteService.AddAsync(newNote);
            }
        }
        public void OnNavigatedFrom(INavigationParameters parameters) { SaveNote(); }
    }
}
