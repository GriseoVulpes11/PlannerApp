using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Data.Sqlite;
using Npgsql;
using Json.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
            Dictionary<int, Dictionary<string, string>> jsonDictonary = load_json();
            foreach(var item in jsonDictonary.Values)
            {
                bool checker = Convert.ToBoolean(item["check"]);
                Create_asignment_box(item["text"], item["due"], checker);
            }
            
        }
        
        private async void UiElementOnKeyDownSaveOnKeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.Enter)
            {
                Dictionary<int, Dictionary<string, string>> fullJsonDictonary = new Dictionary<int, Dictionary<string, string>>();
                int iterNum = 0;
                foreach (object child in AsinPannel.Children)
                {
                    if (child is StackPanel)
                    {
                        Dictionary<string, string> loaderDictionary = new Dictionary<string, string>();
                        StackPanel stackParent = (child as StackPanel);
                        
                        TextBox grandTextBox = stackParent.Children.OfType<TextBox>().FirstOrDefault();
                        loaderDictionary.Add("text",grandTextBox.Text);
                        
                        DatePicker grandDatePicker = stackParent.Children.OfType<DatePicker>().FirstOrDefault();
                        loaderDictionary.Add("due",grandDatePicker.DisplayDate.ToString());
                        
                        CheckBox grandCheckBox = stackParent.Children.OfType<CheckBox>().FirstOrDefault();
                        loaderDictionary.Add("check",grandCheckBox.IsChecked.ToString());
                        
                        
                        fullJsonDictonary.Add(iterNum,loaderDictionary);
                        iterNum = iterNum + 1;
                    }
                }
                fill_json(fullJsonDictonary);
                
                                
            }
        }
        public interface JsonSterilizedData{}

        public class JsonDataNum : JsonSterilizedData
        {
            public int Amm { get; set; }
        }
        public class JsonDataBody : JsonSterilizedData
        {
            public string? Text { get; set; }
            public string? Due { get; set; }
            public bool Check { get; set; }
        }

        private void fill_json(Dictionary<int, Dictionary<string, string>> newData = null)
        {
            if (newData != null)
            {
                List<JsonSterilizedData> _data = new List<JsonSterilizedData>();


                _data.Add(new JsonDataNum
                {
                    Amm = newData.Count
                });
                

                foreach (var item in newData.Values)
                {
                    _data.Add(new JsonDataBody
                    {
                        Text = item["text"],
                        Due = item["due"],
                        Check = Convert.ToBoolean(item["check"])
                    });
                }

                string objjsonData = JsonConvert.SerializeObject(_data);
                string docPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PermStore.json");
                //@"C:\Users\BCoop\RiderProjects\PlannerApp\PlannerApp\Assets\PermStore.json"
                using (StreamWriter outputFile = new StreamWriter(fileName))
                {
                    outputFile.WriteLine(objjsonData);
                }

            }
        }
        public class JsonDeserialize
        {
            public int Amm { get; set; }
            public string? Text { get; set; }
            public string? Due { get; set; }
            public bool? Check { get; set; }
        }

        private Dictionary<int, Dictionary<string, string>> load_json()
        {
            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PermStore.json");
            StreamReader r = new StreamReader(fileName);
            //@"C:\Users\BCoop\RiderProjects\PlannerApp\PlannerApp\Assets\PermStore.json"
            string json = r.ReadToEnd();
            Dictionary<int, Dictionary<string,string>> jsonData = new Dictionary<int, Dictionary<string,string>>();
            JArray obj = JsonConvert.DeserializeObject(json) as JArray;
            List<JsonDeserialize> jsonDeserializes = JsonConvert.DeserializeObject<List<JsonDeserialize>>(obj.ToString());
            int rangeIterable = jsonDeserializes[0].Amm;

            foreach (var value in Enumerable.Range(1, rangeIterable))
            {
                Dictionary<string, string?> temporaryDictionary = new Dictionary<string, string?>();
                var JsonIter = jsonDeserializes[value];
                temporaryDictionary.Add("text", JsonIter.Text);
                temporaryDictionary.Add("due", JsonIter.Due);
                temporaryDictionary.Add("check",JsonIter.Check.ToString());
                jsonData.Add(value, temporaryDictionary!);
            }
            
            return jsonData;
        }
        
        
        private void Create_asignment_box(string? boxFillText = null,string? datePicker = null, bool check = false )
        {
            StackPanel asignmentStackPannel = AsinPannel;
            StackPanel dynamicStackPannel = new StackPanel()
            {
                Width = 400,
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0,0,0,0)
                
            };

            
            
            TextBox dynamicTextBox = new TextBox
            {
                Width = 200,
                Height = 25,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0,0,0,0)
            };
            dynamicTextBox.KeyDown += UiElementOnKeyDownSaveOnKeyDown;
            
            
            if (boxFillText != null)
            {
                dynamicTextBox.Text = boxFillText;
            }
            else
            {
                dynamicTextBox.GotFocus += TextBox_GotFocus;
                dynamicTextBox.Text = "Enter An Assignment";
            }
            
            dynamicStackPannel.Children.Add(dynamicTextBox);

            DatePicker dynamicDatePicker = new DatePicker()
            {
                Height = 25,
                Width = 115,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0,0,0,0),
                Name = "DatePicker"
            };

            if (datePicker != null)
            {
                dynamicDatePicker.SelectedDate = Convert.ToDateTime(datePicker);
            }
            
            dynamicStackPannel.Children.Add(dynamicDatePicker);

            CheckBox dynamicCheckBox = new CheckBox()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                LayoutTransform = new ScaleTransform(2.1,2.1)
            };

            if (check)
            {
                dynamicCheckBox.IsChecked = true;
            }
            
            dynamicStackPannel.Children.Add(dynamicCheckBox);

            Button dynamicButton = new Button()
            {
                Height = 25,
                Width = 40,
                Content = "Delete"
            };
            dynamicButton.Click += DeleteButton_OnClick;
            dynamicStackPannel.Children.Add(dynamicButton);
            Button addButton = this.AddButton;
            AsinPannel.Children.Remove(addButton);
            asignmentStackPannel.Children.Add(dynamicStackPannel);
            AsinPannel.Children.Add(addButton);


        }
        
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Create_asignment_box();
        }


        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(sender as Button);
            AsinPannel.Children.Remove((UIElement)parentObject);

        }

    }
}




