using PrismApp1.ViewModels;

namespace PrismApp1.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MainPageViewModel vm)
        {
            await vm.LoadNotesPublic();
        }
    }
}


