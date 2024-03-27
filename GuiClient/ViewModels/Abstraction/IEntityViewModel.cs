using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using DatabaseClient.Repositories.Abstraction;

namespace GuiClient.ViewModels.Abstraction;

public interface IEntityViewModel<TEntity, TDto>
{
    ICommand ShowEntities { get; }

    Task ShowBy(Func<IRepository<TEntity>, Task<ICollection<TEntity>>> filter, Func<Task<TDto>> dtoFactory);
}