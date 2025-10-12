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

    /// <summary>
    /// Identifies the MaxTextLength dependency property, which specifies the maximum number of characters allowed in
    /// the text of a TextBoxTextBlock control.
    /// </summary>
    /// <remarks>This dependency property can be used in styles, data binding, and animations to control the
    /// maximum text length. The default value is 100.</remarks>
    public static readonly DependencyProperty MaxTextLengthProperty =
        DependencyProperty.Register("MaxLength", typeof(int), typeof(TextBoxTextBlock), new PropertyMetadata(100));

    public static readonly DependencyProperty LabelYPositionProperty =
        DependencyProperty.Register("LabelYPosition", typeof(double), typeof(TextBoxTextBlock), new PropertyMetadata(-18.0));

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
