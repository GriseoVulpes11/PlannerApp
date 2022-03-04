using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlannerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            

            TextBox textBox = new TextBox();
            textBox.Width = 200;
            
        }

        private async Task WriteToFle(string sentString)
        {
             await File.WriteAllTextAsync("WriteText.txt", sentString);
        }

        private async void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox? textbox = sender as TextBox;
                string sen = textbox.Text;
                TitleBlock.Text = "You Entered: " + sen;
                await WriteToFle(sen);
            }
        }
        

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            TextBox dynamicTextBox = new TextBox
            {
                Text = "Enter An Assignment",
                Width = 200,
                Height = 40,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            dynamicTextBox.KeyDown += UIElement_OnKeyDown;
            dynamicTextBox.GotFocus += TextBox_GotFocus;
            
            StackPanel.Children.Remove(AddButton);
            StackPanel.Children.Add(dynamicTextBox);
            StackPanel.Children.Add(AddButton);
        }


        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }
    }
}
//https://www.codeguru.com/dotnet/using-sqlite-in-a-c-application/
public class dataBase
{
    private class Cla
    {
        public int Id { get; set; }
        public string Classname { get; set; }
    }

    private int ExecuteWrite(string query, Dictionary<string, object> args)
    {
        int RowsEffected;
        using (var Connection = new SQLiteConnection("Data Source=permStore.sqlite"))
        {
            Connection.Open();
            using (var cmd = new SQLiteCommand(query, Connection))
            {
                foreach (var pair in args)
                {
                    cmd.Parameters.AddWithValue(pair.Key, pair.Value);
                }

                RowsEffected = cmd.ExecuteNonQuery();
            }

            return RowsEffected;
        }
    }
}

