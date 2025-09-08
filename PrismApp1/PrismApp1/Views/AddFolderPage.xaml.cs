using CommunityToolkit.Maui.Views;
using DataBase.Interfaces;
using PrismApp1.ViewModels;

namespace PrismApp1.Views;

public partial class AddFolderPage : Popup
{
    private TaskCompletionSource<string?> _tcs;

    public AddFolderPage(TaskCompletionSource<string?> tcs)
    {
        InitializeComponent();
        _tcs = tcs;

        var vm = new AddFolderPageViewModel();
        vm.PopupInstance = this;
        vm.FolderAddedCallback = name =>
        {
            _tcs.TrySetResult(name); // ????? ?? ???????????
        };

        BindingContext = vm;
    }

}
