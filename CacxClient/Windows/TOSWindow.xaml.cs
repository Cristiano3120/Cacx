using CacxClient.Extensions;
using System.Windows.Controls;

namespace CacxClient.Windows;

/// <summary>
/// Interaction logic for TOSWindow.xaml
/// </summary>
public partial class TOSWindow : UserControl
{
    public TOSWindow()
    {
        InitializeComponent();
        GoBackBtn.EnableHoverAnimation();
    }
}
