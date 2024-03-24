using AutoMapper;
using DatabaseClient.Contexts;
using DatabaseClient.Models;
using GuiClient.Contexts;
using GuiClient.ViewModels.Windows;

namespace GuiClient.Factories;

public class AllEntitiesWindowViewModelFactory(
    ISecurityContext securityContext,
    IMapper mapper,
    DatabaseContextFactory databaseContextFactory,
    DtoViewFactory dtoViewFactory)
{
    public AllEntitiesWindowViewModel<TEntity, TDto> Create<TEntity, TDto>()
        where TEntity : class, IEntity
        where TDto : class, IEntity, new()
    {
        return new AllEntitiesWindowViewModel<TEntity, TDto>(securityContext, databaseContextFactory, mapper,
            dtoViewFactory);
    }
}