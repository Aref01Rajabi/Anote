using DataBase.Interfaces;
using DataBase.Models;
using System.Collections.ObjectModel;

namespace PrismApp1.ViewModels
{
    public class MainPageViewModel : BindableBase, INavigatedAware
    {

        private readonly INoteService _noteService;
        private readonly INavigationService _navigationService;


        public ObservableCollection<NoteModel> Notes { get; } = new();
        public DelegateCommand AddNoteCommand { get; }
        public DelegateCommand DeleteSelectedCommand { get; }
        public DelegateCommand<Button> ChoosNoteCommand { get; }
        public DelegateCommand<NoteModel?> EditNoteCommand { get; }


        public MainPageViewModel(INavigationService navigationService, INoteService noteService)
        {
            _navigationService = navigationService;
            _noteService = noteService;
            AddNoteCommand = new DelegateCommand(GoToNoteEditor);
            DeleteSelectedCommand = new DelegateCommand(DeleteSelectedNotes);
            EditNoteCommand = new DelegateCommand<NoteModel?>(GoToEditNote);
            ChoosNoteCommand = new DelegateCommand<Button>(ChoosNote);
        }

        private void ChoosNote(Button button)
        {
            
                if (button == null) return;

                var currentColor = button.BackgroundColor;

                var targetColor = Color.FromArgb("#2BFFF1");

                button.BackgroundColor = currentColor == Colors.Transparent
                    ? targetColor
                    : Colors.Transparent;

        }

        private async void GoToNoteEditor()
        {
            await _navigationService.NavigateAsync("NoteEditorPage");
        }

        private async void GoToEditNote(NoteModel? note)
        {
            if (note == null) return;

            var parameters = new NavigationParameters
            {
                { "note", note }
            };

            await _navigationService.NavigateAsync("NoteEditorPage", parameters);
        }

        private async void DeleteSelectedNotes()
        {

        }


        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            await LoadNotes();
        }

        public void OnNavigatedFrom(INavigationParameters parameters) { }

        private async Task LoadNotes()
        {
            Notes.Clear();
            var items = await _noteService.GetAllAsync();
            foreach (var note in items)
                Notes.Add(note);
        }
        public async Task LoadNotesPublic()
        {
            await LoadNotes();
        }


    }
}
