using CacxClient.MVVM;
using System;
using System.Windows.Controls;

namespace CacxClient.Windows;
/// <summary>
/// Interaction logic for VerificationWindow.xaml
/// </summary>
public partial class VerificationWindow : UserControl
{
    public VerificationWindow(VerificationViewModel verificationViewModel, string token)
    {
        InitializeComponent();
    }
}
