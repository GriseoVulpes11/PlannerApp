using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PlannerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            
            Dictionary<int, Dictionary<string, string>> jsonDictonary = LoadUserDataJson();
            foreach(var item in jsonDictonary.Values)
            {
                bool checker = Convert.ToBoolean(item["check"]);
                CreateAssignmentBox(item["text"], item["due"], checker);
            }
            ApplyThemesAndHeader();
        }

        private void ApplyThemesAndHeader()
        {
            Dictionary<string, string?> settingsData = LoadSettingsDataJson();
            ColorBrushed theme = themeFormating(settingsData);
                
            MainView.Background =  theme.BackgroundColorBrush;
            
            settingsButton.Background = theme.TextColorBrush;
            
            TitleBlock.Foreground = theme.TextColorBrush;
            
            AddButton.Background = theme.BackgroundColorBrush;
            AddButton.Foreground = theme.TextColorBrush;

            SaveButton.Background = theme.BackgroundColorBrush;
            SaveButton.Foreground = theme.TextColorBrush;
            
            
            TitleBlock.Text = settingsData["headerText"];
            Title = settingsData["headerText"];
        }

        public class ColorBrushed
        {
            public SolidColorBrush? TextColorBrush { get; set;  }
            public SolidColorBrush? BackgroundColorBrush { get; set; }
            public SolidColorBrush? HighlightedBackgroundColorBrush { get; set; }
        }

        private ColorBrushed themeFormating(Dictionary<string, string?> unformatedDictonary)
        {
            string[] themeBackgroundRgb = unformatedDictonary["themeBackgroundRgb"]!.Split(",");
            string[] themeTextRgb = unformatedDictonary["themeTextRgb"]!.Split(",");
            string[] themeHighlightRgb = unformatedDictonary["themehighlighRgb"]!.Split(",");
            
            ColorBrushed theme = new ColorBrushed();
            theme.BackgroundColorBrush = new SolidColorBrush(Color.FromRgb((byte)Int16.Parse(themeBackgroundRgb[0]), (byte)Int16.Parse(themeBackgroundRgb[1]), (byte)Int16.Parse(themeBackgroundRgb[2])));
            theme.TextColorBrush = new SolidColorBrush(Color.FromRgb((byte)Int16.Parse(themeTextRgb[0]), (byte)Int16.Parse(themeTextRgb[1]), (byte)Int16.Parse(themeTextRgb[2])));
            theme.HighlightedBackgroundColorBrush = new SolidColorBrush(Color.FromRgb((byte)Int16.Parse(themeHighlightRgb[0]),(byte)Int16.Parse(themeHighlightRgb[1]),(byte)Int16.Parse(themeHighlightRgb[2])));

            return theme;
        }

        private void SaveData()
        {
            Dictionary<int, Dictionary<string, string?>> fullJsonDictonary = new Dictionary<int, Dictionary<string, string?>>();
            int iterNum = 0;
            foreach (object child in AsinPannel.Children)
            {
                if (child is StackPanel)
                {
                    Dictionary<string, string?> loaderDictionary = new Dictionary<string, string?>();
                    StackPanel? stackParent = child as StackPanel;
                    
                    TextBox? grandTextBox = stackParent?.Children.OfType<TextBox>().FirstOrDefault();
                    loaderDictionary.Add("text",grandTextBox?.Text);
                        
                    DatePicker? grandDatePicker = stackParent?.Children.OfType<DatePicker>().FirstOrDefault();
                    loaderDictionary.Add("due",grandDatePicker?.DisplayDate.ToString(CultureInfo.InvariantCulture));
                        
                    CheckBox? grandCheckBox = stackParent?.Children.OfType<CheckBox>().FirstOrDefault();
                    loaderDictionary.Add("check",grandCheckBox?.IsChecked.ToString());
                    
                    fullJsonDictonary.Add(iterNum,loaderDictionary);
                    iterNum =  + 1;
                }
            }
            FillUserJson(fullJsonDictonary);

        }
        
        
        private void UiElementOnKeyDownSaveOnKeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.Key == Key.Enter)
            {
                SaveData();
            }
        }
        public interface IJsonSterilizedData{}

        public class JsonDataNum : IJsonSterilizedData
        {
            public int Amm { get; set; }
        }
        public class JsonDataBody : IJsonSterilizedData
        {
            public string? Text { get; set; }
            public string? Due { get; set; }
            public bool Check { get; set; }
        }

        private void FillUserJson(Dictionary<int, Dictionary<string, string?>> newData = null!)
        {
            List<IJsonSterilizedData> data = new List<IJsonSterilizedData>();
                
                data.Add(new JsonDataNum
                {
                    Amm = newData.Count
                });
                

                foreach (var item in newData.Values)
                {
                    data.Add(new JsonDataBody
                    {
                        Text = item["text"],
                        Due = item["due"],
                        Check = Convert.ToBoolean(item["check"])
                    });
                }

                string userJsonData = JsonConvert.SerializeObject(data);
                var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PermStore.json");
                using (StreamWriter outputFile = new StreamWriter(fileName))
                {
                    outputFile.WriteLine(userJsonData);
                }

            
        }
        public class JsonDeserialize
        {
            public int Amm { get; set; }
            public string? Text { get; set; }
            public string? Due { get; set; }
            public bool? Check { get; set; }
        }

        private Dictionary<int, Dictionary<string, string>> LoadUserDataJson()
        {
            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PermStore.json");
            StreamReader r = new StreamReader(fileName);
            string json = r.ReadToEnd();
            r.Close();
            Dictionary<int, Dictionary<string,string>> jsonData = new Dictionary<int, Dictionary<string,string>>();
            JArray obj = JsonConvert.DeserializeObject(json) as JArray;
            List<JsonDeserialize> jsonDeserializes = JsonConvert.DeserializeObject<List<JsonDeserialize>>(obj.ToString());
            if (jsonDeserializes != null)
            {
                int rangeIterable = jsonDeserializes[0].Amm;

                foreach (var value in Enumerable.Range(1, rangeIterable))
                {
                    Dictionary<string, string?> temporaryDictionary = new Dictionary<string, string?>();
                    var jsonIter = jsonDeserializes[value];
                    temporaryDictionary.Add("text", jsonIter.Text);
                    temporaryDictionary.Add("due", jsonIter.Due);
                    temporaryDictionary.Add("check",jsonIter.Check.ToString());
                    jsonData.Add(value, temporaryDictionary!);
                }
            }

            return jsonData;
        }

        public class SettingsDesterilize
        {
            public string? ThemeBackgroundRgb { get; set; }
            public string? ThemeTextRgb { get; set; }
            public string? ThemehighlighRgb { get; set; }
            
            public string? HeaderText { get; set; }
        }
        private Dictionary<string, string?> LoadSettingsDataJson()
        {
            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PlannerSettings.json");
            StreamReader r = new StreamReader(fileName);
            string json = r.ReadToEnd();
            r.Close();
            Dictionary<string, string?> settingsLoad = new Dictionary<string, string?>();
            JObject? settingsObject = JsonConvert.DeserializeObject(json) as JObject;
            
            SettingsDesterilize settingsDeserializes = JsonConvert.DeserializeObject<SettingsDesterilize>(settingsObject?.ToString()!)!;
            string? themeBackgroundRgb = settingsDeserializes?.ThemeBackgroundRgb;
            string? themeTextRgb = settingsDeserializes?.ThemeTextRgb;
            string? themehighlighRgb = settingsDeserializes?.ThemehighlighRgb;
            
            string? headerText = settingsDeserializes!.HeaderText;
            settingsLoad.Add("themeBackgroundRgb", themeBackgroundRgb);
            settingsLoad.Add("themeTextRgb",themeTextRgb);
            settingsLoad.Add("themehighlighRgb",themehighlighRgb);
            
            
            settingsLoad.Add("headerText", headerText);
            return settingsLoad;

        }

        
        
        private void CreateAssignmentBox(string? boxFillText = null,string? datePicker = null, bool check = false )
        {
            Dictionary<string, string?> settingsData = LoadSettingsDataJson();
            ColorBrushed theme = themeFormating(settingsData);
            
            StackPanel assignmentStackPanel = AsinPannel;
            StackPanel dynamicStackPanel = new StackPanel()
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
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0,0,0,0),
                BorderThickness = new Thickness(0),
                Foreground = theme.TextColorBrush,
                Background = theme.HighlightedBackgroundColorBrush
                
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
            
            dynamicStackPanel.Children.Add(dynamicTextBox);

            DatePicker dynamicDatePicker = new DatePicker()
            {
                Height = 25,
                Width = 115,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0,0,0,0),
                Name = "DatePicker",
                Background = theme.TextColorBrush
                
            };
            dynamicDatePicker.KeyDown += UiElementOnKeyDownSaveOnKeyDown;
            if (datePicker != null)
            {
                dynamicDatePicker.SelectedDate = Convert.ToDateTime(datePicker);
            }
            
            dynamicStackPanel.Children.Add(dynamicDatePicker);

            CheckBox dynamicCheckBox = new CheckBox()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                LayoutTransform = new ScaleTransform(2.1,2.1),
                Background = theme.BackgroundColorBrush
            };
                dynamicCheckBox.KeyDown += UiElementOnKeyDownSaveOnKeyDown;
            if (check)
            {
                dynamicCheckBox.IsChecked = true;
            }
            
            dynamicStackPanel.Children.Add(dynamicCheckBox);

            Button dynamicButton = new Button()
            {
                Height = 25,
                Width = 40,
                Content = "Delete",
                BorderThickness = new Thickness(0),
                Background = theme.HighlightedBackgroundColorBrush,
                Foreground = theme.TextColorBrush
                
            };
            dynamicButton.Click += DeleteButton_OnClick;
            dynamicStackPanel.Children.Add(dynamicButton);
            AsinPannel.Children.Remove(MaluMableDockpanel);
            assignmentStackPanel.Children.Add(dynamicStackPanel);
            AsinPannel.Children.Add(MaluMableDockpanel);
        }
        
        private void AddButtonBaseOnClick(object sender, RoutedEventArgs e)
        {
            CreateAssignmentBox();
        }
    

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent((sender as Button)!)!;
            AsinPannel.Children.Remove((UIElement)parentObject);

        }

        private void ButtonBaseSettingsOnClick(object sender, RoutedEventArgs e)
        {
            SettingsWindow subWindow = new SettingsWindow();
            subWindow.Show();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveData();
        }
    }
}




