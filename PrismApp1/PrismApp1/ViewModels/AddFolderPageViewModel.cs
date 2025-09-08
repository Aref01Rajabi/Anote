using DataBase.Models;
using CommunityToolkit.Maui.Views;


namespace PrismApp1.ViewModels
{


    public class AddFolderPageViewModel : BindableBase
{
        private string _newFolderName;
        public string NewFolderName
        {
            get => _newFolderName;
            set => SetProperty(ref _newFolderName, value);
        }
        public DelegateCommand AddFolderCommand { get; }
        public Action<string> FolderAddedCallback { get; set; }


        public AddFolderPageViewModel()
        {
            AddFolderCommand = new DelegateCommand(async () => await AddFolderAsync(), CanAddFolder).ObservesProperty(() => NewFolderName);
        }

        private bool CanAddFolder()
    {
        return !string.IsNullOrWhiteSpace(NewFolderName);
    }

        private async Task AddFolderAsync()
        {
                FolderAddedCallback?.Invoke(NewFolderName);
                NewFolderName = string.Empty;

                await PopupInstance.CloseAsync();
        }

        public CommunityToolkit.Maui.Views.Popup PopupInstance { get; set; }
    }

}
