using GuiClient.Views.UserControls;

namespace GuiClient.ViewModels.UserControls;

public class BooksUserControlViewModel(BooksUserControl control) : AuthenticatedViewModel<BooksUserControl>(control);