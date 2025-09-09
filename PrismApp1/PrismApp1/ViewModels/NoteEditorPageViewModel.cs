using Prism.Mvvm;
using Prism.Navigation;
using Prism.Commands;
using DataBase.Models;
using DataBase.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Dispatching;

namespace PrismApp1.ViewModels
{
    public class NoteEditorPageViewModel : BindableBase, INavigationAware, IDisposable
    {
        private readonly INoteService _noteService;
        private readonly INavigationService _navigationService;
        private int? _folderId;

        private IDispatcherTimer _autoSaveTimer;
        private CancellationTokenSource _cts;

        public NoteModel OriginalNote { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        private bool _isEditable = true;
        public bool IsEditable
        {
            get => _isEditable;
            set => SetProperty(ref _isEditable, value);
        }

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

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("note"))
            {
                OriginalNote = parameters.GetValue<NoteModel>("note");
                Title = OriginalNote.Title;
                Content = OriginalNote.Content;
                RaisePropertyChanged(nameof(Title));
                RaisePropertyChanged(nameof(Content));
                IsEditable = true;
            }

            if (parameters.ContainsKey("folderId"))
                _folderId = parameters.GetValue<int?>("folderId");

            // شروع AutoSave
            _cts = new CancellationTokenSource();
            _autoSaveTimer = Dispatcher.GetForCurrentThread().CreateTimer();
            _autoSaveTimer.Interval = TimeSpan.FromSeconds(5);
            _autoSaveTimer.Tick += async (s, e) => await AutoSaveNote(_cts.Token);
            _autoSaveTimer.Start();
        }

        private async Task AutoSaveNote(CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return;

            if (string.IsNullOrWhiteSpace(Title) && string.IsNullOrWhiteSpace(Content))
                return;

            var finalTitle = string.IsNullOrWhiteSpace(Title)
                ? Content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim()
                : Title.Trim();

            if (string.IsNullOrWhiteSpace(finalTitle))
                return;

            if (OriginalNote != null)
            {
                OriginalNote.Title = finalTitle;
                OriginalNote.Content = Content?.Trim();
                await _noteService.UpdateAsync(OriginalNote);
            }
            else
            {
                var newNote = new NoteModel
                {
                    Title = finalTitle,
                    Content = Content?.Trim(),
                    FolderId = _folderId,
                    CreatedAt = DateTime.Now
                };
                await _noteService.AddAsync(newNote);
                OriginalNote = newNote; // برای دفعات بعدی AutoSave
            }
        }

        private async void Back()
        {
            // قبل از خروج: ذخیره فوری
            if (_cts != null && !_cts.IsCancellationRequested)
                await AutoSaveNote(_cts.Token);

            await _navigationService.NavigateAsync("MainPage", new NavigationParameters
            {
                { "folderId", _folderId }
            });
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            // توقف AutoSave و کنسل کردن Taskها
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            _autoSaveTimer?.Stop();
            _autoSaveTimer = null;
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _autoSaveTimer?.Stop();
            _autoSaveTimer = null;
        }
    }
}
