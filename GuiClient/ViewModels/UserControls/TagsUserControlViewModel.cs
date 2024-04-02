using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Extensions;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;
using GuiClient.Views.Windows;

namespace GuiClient.ViewModels.UserControls;

public class TagsUserControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<Tag, Tag>(securityContext)
{
    protected override (Func<IRepository<Tag>, IMapper, Task<ICollection<Tag>>>, Func<Task<Tag>>) GetFilter(
        string filterName)
    {
        return filterName switch
        {
            null => (null, () => Task.FromResult(new Tag { Id = -1 })),
            "name" when AskerWindow.TryAskString("Enter tag name", out var name) => (async (r, _) =>
            {
                var repo = r.Cast<Tag, ITagsRepository>();
                var tag = await repo.GetTagByNameAsync(name);
                return tag == null ? [] : [tag];
            }, null),
            "name" => (null, null),
            _ => throw InvalidFilter(filterName)
        };
    }
}