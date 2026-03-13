using System.Windows;
using PlugINCivil3D.Presentation.ViewModels;

namespace PlugINCivil3D.Presentation.Views;

public partial class CulvertWindow : Window
{
    public CulvertWindow(CulvertViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
