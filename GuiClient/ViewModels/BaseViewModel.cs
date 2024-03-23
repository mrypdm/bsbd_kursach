using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace GuiClient.ViewModels;

public abstract class BaseViewModel<TType> : NotifyPropertyChanged where TType : FrameworkElement
{
    protected BaseViewModel(TType control)
    {
        Control = control;
        Control.DataContext = this;
    }

    protected TType Control { get; }
}