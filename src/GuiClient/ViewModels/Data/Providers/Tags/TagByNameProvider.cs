﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace GuiClient.ViewModels.Data.Providers.Tags;

public class TagByNameProvider : IDataViewModelProvider<Tag>
{
    private readonly ITagsRepository _tagsRepository;
    private readonly string _name;

    private TagByNameProvider(ITagsRepository tagsRepository, string name)
    {
        _tagsRepository = tagsRepository;
        _name = name;
    }

    public async Task<ICollection<Tag>> GetAllAsync()
    {
        return [await _tagsRepository.GetTagByNameAsync(_name)];
    }

    public Task<Tag> CreateNewAsync()
    {
        throw new NotSupportedException();
    }

    public bool CanCreate => false;

    public string Name => $"Tags with name '{_name}'";

    public static TagByNameProvider Create()
    {
        return AskerWindow.TryAskString("Enter tag name", out var name)
            ? new TagByNameProvider(App.ServiceProvider.GetRequiredService<ITagsRepository>(), name)
            : null;
    }
}