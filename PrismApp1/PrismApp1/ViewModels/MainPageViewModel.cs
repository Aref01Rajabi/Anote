using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using DataBase.Interfaces;
using DataBase.Models;
using Microsoft.Maui.Controls.Shapes;
using PrismApp1.PopUps;
using PrismApp1.Services;
using System.Collections.ObjectModel;

namespace PrismApp1.ViewModels
{
    public class MainPageViewModel : BindableBase, INavigatedAware
    {

        private readonly INoteService _noteService;
        private readonly INavigationService _navigationService;
        private readonly IFolderService _folderService;
        private readonly DialogService _dialogService;

        //public BoolToColorConverter BoolToColor;
        public ObservableCollection<NoteItemModel> Notes { get; } = new();
        public ObservableCollection<FolderModel> Folders { get; } = new();
        public DelegateCommand AddNoteCommand { get; }
        public DelegateCommand DeleteSelectedCommand { get; }
        public DelegateCommand<NoteItemModel> LongPressCommand { get; }
        public DelegateCommand UndoSelectionModeCommand { get; }
        public DelegateCommand<NoteItemModel?> ClickNoteCommand { get; }
        public DelegateCommand<FolderModel?> AddFolderCommand { get; }

        private bool _selectionMode;
        public bool SelectionMode
        {
            get => _selectionMode;
            set => SetProperty(ref _selectionMode, value);
        }


        public MainPageViewModel(INavigationService navigationService, INoteService noteService, IFolderService folderService, DialogService dialogService)
        {
            _navigationService = navigationService;
            _noteService = noteService;
            _folderService = folderService;
            _dialogService = dialogService;
            AddNoteCommand = new DelegateCommand(GoToNoteEditor);
            DeleteSelectedCommand = new DelegateCommand(DeleteSelectedNotes);
            ClickNoteCommand = new DelegateCommand<NoteItemModel?>(ClickNote);
            LongPressCommand = new DelegateCommand<NoteItemModel>(LongPressItem);
            AddFolderCommand = new DelegateCommand<FolderModel?>(AddFolder);
            UndoSelectionModeCommand = new DelegateCommand(UndoSelectionMode);
            //BoolToColor = new BoolToColorConverter();
            SelectionMode = false;
        }

        private void AddFolder(FolderModel? model)
        {
            this.ShowPopupAsync(new Label
            {
                Text = "This is a very important message!"
            }, new PopupOptions
            {
                CanBeDismissedByTappingOutsideOfPopup = false,
                Shape = new RoundRectangle
                {
                    CornerRadius = new CornerRadius(20, 20, 20, 20),
                    StrokeThickness = 2,
                    Stroke = Colors.LightGray
                }
            });
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
            var items = await _noteService.GetAllAsync();

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
            var items = await _folderService.GetAllAsync();

            foreach (var folder in items)
            {
                Folders.Add(new FolderModel
                {
                    Id = folder.Id,
                    Name = folder.Name,
                    //NoteIds = folder.NoteIds,
                    //NoteIdsSerialized = folder.NoteIdsSerialized,
                    //ChildFolderIds = folder.ChildFolderIds,
                    //ChildFolderIdsSerialized = folder.ChildFolderIdsSerialized,
                    ParentFolderId = folder.ParentFolderId
                });
            }
        }

    }
}
