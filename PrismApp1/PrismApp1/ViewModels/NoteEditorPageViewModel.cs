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
        public NoteModel Note { get; set; }
        public NoteModel OriginalNote { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public DelegateCommand SaveNoteCommand { get; }

        public NoteEditorPageViewModel(INavigationService navigationService, INoteService noteService)
        {
            _navigationService = navigationService;
            _noteService = noteService;

            SaveNoteCommand = new DelegateCommand(SaveNote);
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
            }
        }


        private async void SaveNote()
        {
            if (string.IsNullOrWhiteSpace(Title) && string.IsNullOrWhiteSpace(Content))
            {
                await _navigationService.NavigateAsync("MainPage");
                return;
            }

            var finalTitle = string.IsNullOrWhiteSpace(Title)
                ? Content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim()
                : Title.Trim();

            if (string.IsNullOrWhiteSpace(finalTitle))
            {
                await _navigationService.NavigateAsync("MainPage");
                return;
            }

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
                    CreatedAt = DateTime.Now
                };

                await _noteService.AddAsync(newNote);
            }

            await _navigationService.NavigateAsync("MainPage");
        }

        public void OnNavigatedFrom(INavigationParameters parameters) { }
    }
}
