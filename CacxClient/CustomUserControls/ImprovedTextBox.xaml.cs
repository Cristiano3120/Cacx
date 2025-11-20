using System.Windows;
using System.Windows.Controls;

namespace CacxClient.CustomUserControls;
/// <summary>
/// Interaction logic for ImprovedTextBox.xaml
/// </summary>
public partial class ImprovedTextBox : UserControl
{
    public ImprovedTextBox()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty LabelProperty =
           DependencyProperty.Register("Label", typeof(string), typeof(ImprovedTextBox), new PropertyMetadata(""));

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(ImprovedTextBox),
            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty MaxTextLengthProperty =
        DependencyProperty.Register("MaxLength", typeof(int), typeof(ImprovedTextBox), new PropertyMetadata(100));

    public static readonly DependencyProperty LabelYPositionProperty =
        DependencyProperty.Register("LabelYPosition", typeof(double), typeof(ImprovedTextBox), new PropertyMetadata(-18.0));

    public static readonly DependencyProperty AcceptsReturnProperty =
        DependencyProperty.Register("AcceptsReturn", typeof(bool), typeof(ImprovedTextBox), new PropertyMetadata(false));

    public static readonly DependencyProperty TextWrappingProperty =
        DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(ImprovedTextBox), new PropertyMetadata(TextWrapping.NoWrap));

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
