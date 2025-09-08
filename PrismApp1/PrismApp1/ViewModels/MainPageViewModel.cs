using CommunityToolkit.Maui.Extensions;
using DataBase.Interfaces;
using DataBase.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PrismApp1.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PrismApp1.ViewModels
{
    public class MainPageViewModel : BindableBase, INavigatedAware
    {

        private readonly INoteService _noteService;
        private readonly INavigationService _navigationService;
        private readonly IFolderService _folderService;

        private int? _currentFolderId;

        public ObservableCollection<NoteItemModel> Notes { get; } = new();
        public ObservableCollection<FolderModel> Folders { get; } = new();
        public DelegateCommand AddNoteCommand { get; }
        public DelegateCommand DeleteSelectedCommand { get; }
        public DelegateCommand<NoteItemModel> LongPressCommand { get; }
        public DelegateCommand UndoSelectionModeCommand { get; }
        public DelegateCommand<NoteItemModel?> ClickNoteCommand { get; }
        public DelegateCommand<FolderModel?> ClickFolderCommand { get; }
        public DelegateCommand AddFolderCommand { get; }

        private bool _selectionMode;
        public bool SelectionMode
        {
            get => _selectionMode;
            set => SetProperty(ref _selectionMode, value);
        }


        public MainPageViewModel(INavigationService navigationService, INoteService noteService, IFolderService folderService)
        {
            _navigationService = navigationService;
            _noteService = noteService;
            _folderService = folderService;
            AddNoteCommand = new DelegateCommand(GoToNoteEditor);
            DeleteSelectedCommand = new DelegateCommand(DeleteSelectedNotes);
            ClickNoteCommand = new DelegateCommand<NoteItemModel?>(ClickNote);
            ClickFolderCommand = new DelegateCommand<FolderModel?>(ClickFolder);
            LongPressCommand = new DelegateCommand<NoteItemModel>(LongPressItem);
            AddFolderCommand = new DelegateCommand(ShowAddFolderAsync);
            UndoSelectionModeCommand = new DelegateCommand(UndoSelectionMode);
            SelectionMode = false;
            _currentFolderId = null;
        }

        private async void ClickFolder(FolderModel? model)
        {
            _currentFolderId = model.Id;
            await LoadFolders();
            await LoadNotes();
        }

        private async void ShowAddFolderAsync()
        {
            var tcs = new TaskCompletionSource<string?>();

            var popup = new AddFolderPage(tcs);
            await Application.Current.MainPage.ShowPopupAsync(popup);

            var folderName = await tcs.Task;
            if(_currentFolderId == null)
            {
                _folderService.AddAsync(new FolderModel { Name = folderName, ParentFolderId = null });
            }
            else
            {
                _folderService.AddAsync(new FolderModel { Name = folderName, ParentFolderId = _currentFolderId });
            }
            await LoadFolders();
        }

        private void UndoSelectionMode()
        {
            SelectionMode = false;
            foreach(var note in Notes)
            {
                note.IsSelected = false;
            }
        }

        private async void GoToNoteEditor()
        {
            await _navigationService.NavigateAsync("NoteEditorPage");
        }

        private async void ClickNote(NoteItemModel? note)
        {
            if (!SelectionMode)
            {
                if (note == null) return;
                var noteParameter = new NoteModel { Id = note.Id, Title = note.Title, Content = note.Content, CreatedAt = note.CreatedAt };
                var parameters = new NavigationParameters
                {
                    { "note", noteParameter }
                };

                await _navigationService.NavigateAsync("NoteEditorPage", parameters);

            }
            if (SelectionMode)
            {
                Notes.FirstOrDefault(n => n.Id == note.Id).IsSelected = !note.IsSelected; 
            }
        }

        private async void DeleteSelectedNotes()
        {
            foreach(var note in Notes)
            {
                if (note.IsSelected)
                {
                    _noteService.DeleteAsync(note.Id);
                }
            }
            SelectionMode = false;
            await LoadNotes();
        }

        public void LongPressItem(NoteItemModel item)
        {
            if (!SelectionMode)
            {
                SelectionMode = true;
            }
        }

        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            await LoadNotes();
            await LoadFolders();
        }

        public void OnNavigatedFrom(INavigationParameters parameters) { }

        private async Task LoadNotes()
        {
            Notes.Clear();
            var items = await _noteService.GetByFolderIdAsync(_currentFolderId);

            foreach (var note in items)
            {
                Notes.Add(new NoteItemModel
                {
                    Id = note.Id,
                    Title = note.Title,
                    Content = note.Content,
                    CreatedAt = note.CreatedAt,
                    IsSelected = false
                });
            }
        }

        private async Task LoadFolders()
        {
            Folders.Clear();
            if(_currentFolderId == null)
            {
                var items = await _folderService.GetAllAsync();
                foreach (var folder in items)
                {
                    Folders.Add(folder);
                }
            }
            else
            {
                var items = await _folderService.GetByParentIdAsync(_currentFolderId);
                foreach (var folder in items)
                {
                    Folders.Add(folder);
                }
            }
        }

    }
}
