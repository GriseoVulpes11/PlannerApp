using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                System.Windows.Controls.TextBox textbox = sender as System.Windows.Controls.TextBox;
                string sen = textbox.Text;
                textBlock1.Text = "You Entered: " + sen;
            }
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

