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

    public static readonly DependencyProperty MaxTextLengthProperty =
        DependencyProperty.Register("MaxLength", typeof(int), typeof(TextBoxTextBlock), new PropertyMetadata(100));

    public static readonly DependencyProperty LabelYPositionProperty =
        DependencyProperty.Register("LabelYPosition", typeof(double), typeof(TextBoxTextBlock), new PropertyMetadata(-18.0));

    public static readonly DependencyProperty AcceptsReturnProperty =
        DependencyProperty.Register("AcceptsReturn", typeof(bool), typeof(TextBoxTextBlock), new PropertyMetadata(false));

    public static readonly DependencyProperty TextWrappingProperty =
        DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(TextBoxTextBlock), new PropertyMetadata(TextWrapping.NoWrap));

    public TextWrapping TextWrapping
    {
        get => (TextWrapping)GetValue(TextWrappingProperty);
        set => SetValue(TextWrappingProperty, value);
    }

    public bool AcceptsReturn
    {
        get => (bool)GetValue(AcceptsReturnProperty);
        set => SetValue(AcceptsReturnProperty, value);
    }

    public double LabelYPosition
    {
        get => (double)GetValue(LabelYPositionProperty);
        set => SetValue(LabelYPositionProperty, value);
    }

    public int MaxTextLength
    {
        get => (int)GetValue(MaxTextLengthProperty);
        set => SetValue(MaxTextLengthProperty, value);
    }

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
