using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using GuiClient.Commands;
using GuiClient.Contexts;
using GuiClient.Factories;
using Microsoft.EntityFrameworkCore;

namespace GuiClient.ViewModels.Windows;

public class AllEntitiesWindowViewModel<TEntity, TDto> : AuthenticatedViewModel
    where TEntity : class, IEntity
    where TDto : class, IEntity, new()
{
    private readonly DatabaseContextFactory _daaDatabaseContextFactory;
    private readonly DtoViewFactory _dtoFactory;
    private readonly IMapper _mapper;
    private IReadOnlyCollection<TDto> _entities;

    public AllEntitiesWindowViewModel(ISecurityContext securityContext,
        DatabaseContextFactory daaDatabaseContextFactory, IMapper mapper,
        DtoViewFactory dtoFactory)
        : base(securityContext)
    {
        _daaDatabaseContextFactory = daaDatabaseContextFactory;
        _mapper = mapper;
        _dtoFactory = dtoFactory;

        Refresh = new AsyncActionCommand(RefreshAsync);
        Update = new FuncCommand<TDto>(UpdateAsync);
        Delete = new AsyncFuncCommand<TDto>(DeleteAsync);
    }

    public string WindowTitle { get; } = $"All of {typeof(TEntity).Name}s";

    public IReadOnlyCollection<TDto> Entities
    {
        get => _entities;
        private set => SetField(ref _entities, value);
    }

    public ICommand Refresh { get; }

    public ICommand Update { get; }

    public ICommand Delete { get; }

    public async Task RefreshAsync()
    {
        await using var context = _daaDatabaseContextFactory.Create();
        Entities = await context.Set<TEntity>().ProjectTo<TDto>(_mapper.ConfigurationProvider).ToArrayAsync();
    }

    private void UpdateAsync(TDto item)
    {
        var window = _dtoFactory.Create(item);
        window.Show();
        window.Closed += async (_, _) => { await RefreshAsync(); };
    }

    private async Task DeleteAsync(TDto item)
    {
        await using var context = _daaDatabaseContextFactory.Create();
        await context.Set<TEntity>().Where(m => m.Id == item.Id).ExecuteDeleteAsync();

        await RefreshAsync();
    }
}