﻿using GuiClient.Contexts;
using GuiClient.ViewModels.Abstraction;

namespace GuiClient.ViewModels.UserControls;

public class BooksToTagsUserControlViewModel(ISecurityContext securityContext)
    : AuthenticatedViewModel(securityContext);