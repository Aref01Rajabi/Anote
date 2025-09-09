using CommunityToolkit.Maui.Extensions;
using DataBase.Interfaces;
using DataBase.Models;
using PrismApp1.Models;
using PrismApp1.Views;
using System.Collections.ObjectModel;

namespace PrismApp1.ViewModels
{
    public class MainPageViewModel : BindableBase, INavigatedAware
    {

        private readonly INoteService _noteService;
        private readonly INavigationService _navigationService;
        private readonly IFolderService _folderService;


        public ObservableCollection<NoteItemeModel> Notes { get; } = new();
        public ObservableCollection<FolderItemeModel> Folders { get; } = new();
        public DelegateCommand AddNoteCommand { get; }
        public DelegateCommand DeleteSelectedCommand { get; }
        public DelegateCommand LongPressCommand { get; }
        public DelegateCommand UndoSelectionModeCommand { get; }
        public DelegateCommand GoBackFolderCommand { get; }
        public DelegateCommand<NoteItemeModel?> ClickNoteCommand { get; }
        public DelegateCommand<FolderItemeModel> ClickFolderCommand { get; }
        public DelegateCommand AddFolderCommand { get; }

        private bool _selectionMode;
        public bool SelectionMode
        {
            get => _selectionMode;
            set => SetProperty(ref _selectionMode, value);
        }
        private int? _currentFolderId;
        public int? CurrentFolderId
        {
            get => _currentFolderId;
            set
            {
                SetProperty(ref _currentFolderId, value);
                RaisePropertyChanged(nameof(IsInSubFolder));
            }
        }

        public bool IsInSubFolder => _currentFolderId != 0;


        public MainPageViewModel(INavigationService navigationService, INoteService noteService, IFolderService folderService)
        {
            _navigationService = navigationService;
            _noteService = noteService;
            _folderService = folderService;
            AddNoteCommand = new DelegateCommand(GoToNoteEditor);
            DeleteSelectedCommand = new DelegateCommand(DeleteSelected);
            ClickNoteCommand = new DelegateCommand<NoteItemeModel?>(ClickNote);
            ClickFolderCommand = new DelegateCommand<FolderItemeModel>(ClickFolder);
            LongPressCommand = new DelegateCommand(LongPressItem);
            AddFolderCommand = new DelegateCommand(ShowAddFolderAsync);
            GoBackFolderCommand = new DelegateCommand(GoBackFolder);
            UndoSelectionModeCommand = new DelegateCommand(UndoSelectionMode);
            SelectionMode = false;
            CurrentFolderId = 0;
        }

        private async void GoBackFolder()
        {
            var currentFolder = await _folderService.GetByIdAsync(CurrentFolderId.Value);

            CurrentFolderId = currentFolder?.ParentFolderId;

            SelectionMode = false;
            await LoadFolders();
            await LoadNotes();
        }

        private async void ClickFolder(FolderItemeModel model)
        {
            if(!SelectionMode)
            {
                
                CurrentFolderId = model.Id;
                await LoadFolders();
                await LoadNotes();
            }
            else
            {
                Folders.FirstOrDefault(n => n.Id == model.Id).IsSelected = !model.IsSelected;
            }
        }

        private async void ShowAddFolderAsync()
        {
            var tcs = new TaskCompletionSource<string?>();

            var popup = new AddFolderPage(tcs);
            await Application.Current.MainPage.ShowPopupAsync(popup);

            var folderName = await tcs.Task;
            if(CurrentFolderId == 0)
            {
                _folderService.AddAsync(new FolderModel { Name = folderName, ParentFolderId = 0 });
            }
            else
            {
                _folderService.AddAsync(new FolderModel { Name = folderName, ParentFolderId = CurrentFolderId });
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
            var parameters = new NavigationParameters
            {
                { "folderId", CurrentFolderId }
            };

            await _navigationService.NavigateAsync("NoteEditorPage", parameters);
        }

        private async void ClickNote(NoteItemeModel? note)
        {
            if (!SelectionMode)
            {
                var noteParameter = new NoteModel { Id = note.Id, Title = note.Title, Content = note.Content, CreatedAt = note.CreatedAt };
                var parameters = new NavigationParameters
                {
                    { "note", noteParameter }
                };

                await _navigationService.NavigateAsync("NoteEditorPage", parameters);

            }
            else
            {
                Notes.FirstOrDefault(n => n.Id == note.Id).IsSelected = !note.IsSelected; 
            }
        }

        private async void DeleteSelected()
        {
            foreach(var note in Notes)
            {
                if (note.IsSelected)
                {
                    _noteService.DeleteAsync(note.Id);
                }
            }

            foreach (var folder in Folders)
            {
                if (folder.IsSelected)
                {
                    _folderService.DeleteAsync(folder.Id);
                }
            }

            SelectionMode = false;
            await LoadFolders();
            await LoadNotes();
        }

        public void LongPressItem()
        {
            if (!SelectionMode)
            {
                SelectionMode = true;
            }
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("folderId"))
            {
                CurrentFolderId = parameters.GetValue<int?>("folderId");
            }
             LoadNotes();
             LoadFolders();
        }

        public void OnNavigatedFrom(INavigationParameters parameters) { }

        private async Task LoadNotes()
        {
            Notes.Clear();
            var items = await _noteService.GetByFolderIdAsync(CurrentFolderId);

            foreach (var note in items)
            {
                Notes.Add(new NoteItemeModel
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
            var items = await _folderService.GetByParentIdAsync(CurrentFolderId);

            foreach (var folder in items)
            {
                Folders.Add( new FolderItemeModel
                {
                    Id = folder.Id,
                    Name = folder.Name ?? string.Empty,
                    ParentFolderId = folder.ParentFolderId,
                    IsSelected = false
                });
            }
        }


    }
}
