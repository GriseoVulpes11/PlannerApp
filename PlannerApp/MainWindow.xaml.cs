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
using static PlannerApp.SettingsWindow;


namespace PlannerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        // ReSharper disable once PublicConstructorInAbstractClass
        public MainWindow()
        {
            InitializeComponent();
            //Checks if PlannerSettings.Json exists
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PlannerSettings.json");
            if (File.Exists(fileName) == false)
            {
                fillSettingsData("50,50,50","200,171,131","40,40,40","Welcome to Planner");
            }
            //Checks if ThemesStorage.Json exists 
            string themesStorageFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ThemesStorage.json");
            if (File.Exists(themesStorageFileName) == false)
            {
                FillThemeJson();
            }
            
            
            
            Dictionary<int, Dictionary<string, string>> jsonDictionary = LoadUserDataJson();
            foreach(Dictionary<string, string> item in jsonDictionary.Values)
            {
                bool checker = Convert.ToBoolean(item["check"]);
                CreateAssignmentBox(item["text"], item["due"], checker);
            }
            ApplyThemesAndHeader();
        }

        private void ApplyThemesAndHeader()
        {
            Dictionary<string, string?> settingsData = LoadSettingsDataJson();
            ColorBrushed theme = ThemeFormatting(settingsData!);
                
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

        private ColorBrushed ThemeFormatting(Dictionary<string, string> unformattedDictionary)
        {
            string[] themeBackgroundRgb = unformattedDictionary["themeBackgroundRgb"].Split(",");
            string[] themeTextRgb = unformattedDictionary["themeTextRgb"].Split(",");
            string[] themeHighlightRgb = unformattedDictionary["themeHighlightRgb"].Split(",");
            
            ColorBrushed theme = new ColorBrushed();
            try
            {

                theme.BackgroundColorBrush = new SolidColorBrush(Color.FromRgb((byte)Int16.Parse(themeBackgroundRgb[0]),
                    (byte)Int16.Parse(themeBackgroundRgb[1]), (byte)Int16.Parse(themeBackgroundRgb[2])));
                theme.TextColorBrush = new SolidColorBrush(Color.FromRgb((byte)Int16.Parse(themeTextRgb[0]),
                    (byte)Int16.Parse(themeTextRgb[1]), (byte)Int16.Parse(themeTextRgb[2])));
                theme.HighlightedBackgroundColorBrush = new SolidColorBrush(Color.FromRgb(
                    (byte)Int16.Parse(themeHighlightRgb[0]), (byte)Int16.Parse(themeHighlightRgb[1]),
                    (byte)Int16.Parse(themeHighlightRgb[2])));

                return theme;
            }
            catch (FormatException)
            {
                string fileNameLoad = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ThemesStorage.json");
                StreamReader reader = new(fileNameLoad);
                string themesJson = reader.ReadToEnd();
                reader.Close();
                var jDeserializeObject = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string,string>>>(themesJson);
                Dictionary<string, Dictionary<string, string>> themeDictionary = new();
                if (jDeserializeObject != null)
                    foreach (var Theme in jDeserializeObject)
                    {
                        if (Theme.Value != unformattedDictionary)
                        {
                            themeDictionary.Add(Theme.Key, Theme.Value);
                        }
                    }

                ColorBrushed  themeColorBrushed = ThemeFormatting(themeDictionary["Dark Tan"]);
                
                SettingsDesterilize data = new()
                {
                    //header
                    HeaderText = TitleBlock.Text,
                    //theme colors
                    ThemeBackgroundRgb = "50,50,50",
                    ThemeTextRgb = "200,171,131",
                    ThemeHighlightRgb = "40,40,40"
                };

                string jsonData = JsonConvert.SerializeObject(data);
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "PlannerSettings.json");
                using StreamWriter outputFile = new(fileName);
                outputFile.WriteLine(jsonData);
                return themeColorBrushed;
            }
            
        }

        private void SaveData()
        {
            Dictionary<int, Dictionary<string, string?>> fullJsonDictionary = new();
            int iterNum = 0;
            foreach (object child in AsinPannel.Children)
            {
                if (child is StackPanel)
                {
                    StackPanel stackParent = (child as StackPanel);
                    {
                        Dictionary<string, string> loaderDictionary = new();

                        TextBox? grandTextBox = stackParent.Children.OfType<TextBox>().FirstOrDefault();
                        loaderDictionary.Add("text", grandTextBox!.Text);

                        DatePicker? grandDatePicker = stackParent.Children.OfType<DatePicker>().FirstOrDefault();
                        loaderDictionary.Add("due",
                            grandDatePicker!.DisplayDate.ToString(CultureInfo.InvariantCulture));

                        CheckBox? grandCheckBox = stackParent.Children.OfType<CheckBox>().FirstOrDefault();
                        loaderDictionary.Add("check", grandCheckBox!.IsChecked.ToString()!);

                        fullJsonDictionary.Add(iterNum, loaderDictionary!);
                        iterNum = +1;
                    }
                }
            }

            FillUserJson(fullJsonDictionary);

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

        private static void FillUserJson(Dictionary<int, Dictionary<string, string?>> newData = null)
        {
            List<IJsonSterilizedData> data = new()
            {
                new JsonDataNum
                {
                    Amm = newData.Count
                }
            };
            data.AddRange(newData.Values.Select(item => new JsonDataBody { Text = item["text"], Due = item["due"], Check = Convert.ToBoolean(item["check"]) }));


            string userJsonData = JsonConvert.SerializeObject(data);
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PermStore.json");
            using StreamWriter outputFile = new(fileName);
            outputFile.WriteLine(userJsonData);
        }
        public class JsonDeserialize
        {
            public int Amm { get; set; }
            public string? Text { get; set; }
            public string? Due { get; set; }
            public bool? Check { get; set; }
        }

        private static Dictionary<int, Dictionary<string, string>> LoadUserDataJson()
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PermStore.json");
            if (File.Exists(fileName))
            {
                StreamReader r = new(fileName);
                string json = r.ReadToEnd();
                r.Close();
                Dictionary<int, Dictionary<string, string>> jsonData = new();
                JArray obj = JsonConvert.DeserializeObject(json) as JArray ?? throw new InvalidOperationException();
                List<JsonDeserialize> jsonDeserializes =
                    JsonConvert.DeserializeObject<List<JsonDeserialize>>(obj.ToString())!;
                if (jsonDeserializes != null)
                {
                    int rangeIterable = jsonDeserializes[0].Amm;

                    foreach (int value in Enumerable.Range(1, rangeIterable))
                    {
                        Dictionary<string, string?> temporaryDictionary = new();
                        JsonDeserialize jsonIter = jsonDeserializes[value];
                        temporaryDictionary.Add("text", jsonIter.Text);
                        temporaryDictionary.Add("due", jsonIter.Due);
                        temporaryDictionary.Add("check", jsonIter.Check.ToString());
                        jsonData.Add(value, temporaryDictionary!);
                    }
                }

                return jsonData;
            }
            else
            {
                Dictionary<int, Dictionary<string, string>> newUserDictonary =
                    new()
                    {
                        {
                            1,
                            new Dictionary<string, string>
                            {
                                { "text", "Enter An Assignment" },
                                { "due", "04/16/2022 00:00:00" },
                                { "check", "false" }
                            }
                        }
                    };

                FillUserJson(newUserDictonary);
                return newUserDictonary;
            }
        }

        public class SettingsDesterilize
        {
            public string? ThemeBackgroundRgb { get; init; }
            public string? ThemeTextRgb { get; init; }
            public string? ThemeHighlightRgb { get; init; }
            
            public string? HeaderText { get; init; }
        }
        private static Dictionary<string, string?> LoadSettingsDataJson()
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PlannerSettings.json");
            StreamReader r = new(fileName);
            string json = r.ReadToEnd();
            r.Close();
            Dictionary<string, string?> settingsLoad = new();
            JObject? settingsObject = JsonConvert.DeserializeObject(json) as JObject;
            
            SettingsDesterilize settingsDeserializes = JsonConvert.DeserializeObject<SettingsDesterilize>(settingsObject?.ToString()!)!;
            string? themeBackgroundRgb = settingsDeserializes.ThemeBackgroundRgb;
            string? themeTextRgb = settingsDeserializes.ThemeTextRgb;
            string? themeHighlightRgb = settingsDeserializes.ThemeHighlightRgb;
            
            string? headerText = settingsDeserializes.HeaderText;
            settingsLoad.Add("themeBackgroundRgb", themeBackgroundRgb);
            settingsLoad.Add("themeTextRgb",themeTextRgb);
            settingsLoad.Add("themeHighlightRgb",themeHighlightRgb);
            
            
            settingsLoad.Add("headerText", headerText);
            return settingsLoad;

        }

        
        
        private void CreateAssignmentBox(string? boxFillText = null,string? datePicker = null, bool check = false )
        {
            Dictionary<string, string?> settingsData = LoadSettingsDataJson();
            ColorBrushed theme = ThemeFormatting(settingsData!);
            
            StackPanel assignmentStackPanel = AsinPannel;
            StackPanel dynamicStackPanel = new()
            {
                Width = 400,
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0,0,0,0)
                
            };

            
            
            TextBox dynamicTextBox = new()
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

            DatePicker dynamicDatePicker = new()
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

            CheckBox dynamicCheckBox = new()
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
            SettingsWindow subWindow = new();
            subWindow.Show();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveData();
        }
    }
}