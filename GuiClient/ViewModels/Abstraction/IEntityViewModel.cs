using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;

namespace GuiClient.ViewModels.Abstraction;

public interface IEntityViewModel<TEntity> where TEntity : IEntity
{
    ICommand ShowEntities { get; }

    Task ShowBy(Func<IRepository<TEntity>, Task<ICollection<TEntity>>> filter);
}