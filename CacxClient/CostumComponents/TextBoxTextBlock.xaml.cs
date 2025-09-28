using System.Windows;
using System.Windows.Controls;

namespace CacxClient.CostumComponents;

/// <summary>
/// Custom UserControl that combines a <see cref="TextBlock"/> and a <see cref="TextBox"/>
/// <para>
/// Uses animations to make it look better
/// </para>
/// </summary>
public partial class TextBoxTextBlock : UserControl
{
    public TextBoxTextBlock()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty LabelProperty =
           DependencyProperty.Register("Label", typeof(string), typeof(TextBoxTextBlock), new PropertyMetadata(""));

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(TextBoxTextBlock),
            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}
