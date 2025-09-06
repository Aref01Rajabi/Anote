using PrismApp1.Views;
using DataBase.Services;
using System.IO;
using DataBase.Interfaces;


namespace PrismApp1;

internal static class PrismStartup
{
    public static void Configure(PrismAppBuilder builder)
    {
        builder.RegisterTypes(RegisterTypes)
               .OnAppStart("NavigationPage/MainPage");
    }

    private static void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<MainPage>()
                         .RegisterInstance(SemanticScreenReader.Default);
        containerRegistry.RegisterForNavigation<NoteEditorPage>();

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "notData.db3");
        containerRegistry.RegisterInstance(dbPath);
        containerRegistry.Register<INoteService, NoteService>();
    }
}
