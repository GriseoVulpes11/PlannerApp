using System.Windows;

namespace PlannerApp;

public partial class ThemeMaker : Window
{
    public ThemeMaker()
    {
        InitializeComponent();
    }

    
    private void AddToThemes(object sender, RoutedEventArgs e)
    {
        CustomUserTheme userTheme = new CustomUserTheme();
    }
}