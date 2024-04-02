using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.ViewModels.UserControls;

public class ReviewsUserControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<Review, ReviewDto>(securityContext)
{
    protected override (Func<IRepository<Review>, IMapper, Task<ICollection<ReviewDto>>>, Func<Task<ReviewDto>>)
        GetFilter(
            string filterName)
    {
        return (null, null);
    }
}