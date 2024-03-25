using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.UserControls;

public class TagsUserControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<Tag, Tag>(securityContext)
{
    protected override Func<IRepository<Tag>, Task<ICollection<Tag>>> GetFilter(string filter)
    {
        return filter switch
        {
            "name" when AskerWindow.TryAskString("Enter tag name", out var name) => async r =>
            {
                var repo = r as ITagsRepository ?? throw InvalidRepo(r.GetType(), typeof(ITagsRepository));
                return [await repo.GetTagByNameAsync(name)];
            },
            "name" => null,
            _ => throw InvalidFilter(filter)
        };
    }
}