using System;
using DatabaseClient.Models;
using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.Factories;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.UserControls;

public class BooksUserControlViewModel(
    ISecurityContext securityContext,
    AllEntitiesWindowViewModelFactory factory)
    : EntityUserControlViewModel<Book, BookDto>(securityContext, factory)
{
    protected override object GetFilter(string filter)
    {
        return filter switch
        {
            "count" => AskerWindow.AskInt("Enter count"),
            "tags" => AskerWindow.AskString("Enter tags, separated by comma")
                ?.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
            _ => AskerWindow.AskString($"Enter {filter}")
        };
    }
}