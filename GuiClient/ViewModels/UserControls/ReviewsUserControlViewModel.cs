using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseClient.Models;
using DatabaseClient.Repositories.Abstraction;
using GuiClient.Contexts;
using GuiClient.Dto;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.ViewModels.UserControls;

public class ReviewsUserControlViewModel(ISecurityContext securityContext)
    : EntityUserControlViewModel<Review, ReviewDto>(securityContext)
{
    protected override Func<IRepository<Review>, Task<ICollection<Review>>> GetFilter(string filter)
    {
        throw new NotImplementedException();
    }
}