using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Newtonsoft.Json;

namespace PlannerApp;

public partial class SettingsWindow
{
    public SettingsWindow()
    {
        InitializeComponent();
        LoadThemeJson();
        Title = "Settings";
        foreach (string themeKey in ThemeDictonary.Keys)
        {
            ThemeTemplateBox.Items.Add(themeKey);
        }
        
        
    }


    public class SettingsDesterilize
    {
        public string ThemeBackgroundRgb { get; set; }
        public string themeTextRgb { get; set; }
        public string themeHighlightRgb { get; set; }
        public string headerText { get; set; }
    }

    public static void fillSettingsData(string themeBackgroundRgb, string themeTextRgb, string themeHighlightRgb,
        string headerText)
    {
        if (headerText == "")
        {
            headerText = "welcome to Planner";
        }
        SettingsDesterilize data = new()
        {
            //header
            headerText = headerText,
            //theme colors
            ThemeBackgroundRgb = themeBackgroundRgb,
            themeTextRgb = themeTextRgb,
            themeHighlightRgb = themeHighlightRgb
        };

        string jsonData = JsonConvert.SerializeObject(data);
        string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PlannerSettings.json");
        using StreamWriter outputFile = new(fileName);
        outputFile.WriteLine(jsonData);
    }

    public static void FillThemeJson()
    {
        string themeListJson = JsonConvert.SerializeObject(ThemeDictonary);
        string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ThemesStorage.json");
        using StreamWriter outputFile = new(fileName);
        outputFile.WriteLine(themeListJson);
    }

    private void LoadThemeJson()
    {
        string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ThemesStorage.json");
        StreamReader reader = new(fileName);
        string themesJson = reader.ReadToEnd();
        reader.Close();
        var jDeserializeObject = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string,string>>>(themesJson);
        ThemeDictonary.Clear();
        if (jDeserializeObject != null)
            foreach (var Theme in jDeserializeObject)
            {
                ThemeDictonary.Add(Theme.Key, Theme.Value);
            }
    }
    
    public static Dictionary<string, Dictionary<string, string>> ThemeDictonary =
        new()
        {
            {
                "Dark Tan",
                new Dictionary<string, string>
                {
                    { "themeBackgroundRgb", "50,50,50" },
                    { "themeTextRgb", "200,171,131" },
                    { "themeHighlightRgb", "40,40,40" }
                }
            }
        };

    private void ApplyButtonBaseOnClick(object sender, RoutedEventArgs e)
    {
        string themeComboBoxSelection = ThemeTemplateBox.Text;
        if (themeComboBoxSelection == "")
        {
            themeComboBoxSelection = "Dark Tan";
        }

        string headerChangeText = HeaderChangeTextBox.Text;
        Dictionary<string, string> themeDictionary = ThemeDictonary[themeComboBoxSelection];
        fillSettingsData(themeDictionary["themeBackgroundRgb"], themeDictionary["themeTextRgb"],
            themeDictionary["themeHighlightRgb"], headerChangeText);
        Close();
    }

    private void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        TextBox tb = (TextBox)sender;
        tb.Text = string.Empty;
        tb.GotFocus -= TextBox_GotFocus;
    }

    private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
    {
        Regex regex = new Regex("[^0-9]+$");
        e.Handled = regex.IsMatch(e.Text);
    }
    private void CustomTheme_OnClick(object sender, RoutedEventArgs e)
    {
        StackPanel mainStackPanel = new StackPanel
        {
            Name = "CustomThemeStackPanel"
        };
        RegisterName("CustomThemeStackPanel", mainStackPanel);
        TextBlock themeIntrodution = new TextBlock
        {
            Text = "Input colors you want in RGB format",
            Margin = new Thickness(10, 10, 0, 0)
        };
        mainStackPanel.Children.Add(themeIntrodution);


        //BackGround Color
        TextBlock userChosenBackgroundColorIntroduction = new TextBlock
        {
            Text = "Choose a background color",
            Margin = new Thickness(10, 10, 0, 0)
        };
        mainStackPanel.Children.Add(userChosenBackgroundColorIntroduction);

        DockPanel userChosenBackgroundColorDockPanel = new DockPanel();
        TextBox userChosenBackgroundColorR = new TextBox
        {
            Name = "UserChosenBackgroundColorR",
            Text = "Red",
            Margin = new Thickness(10, 10, 0, 0),
            Width = 40,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        userChosenBackgroundColorR.GotFocus += TextBox_GotFocus;
        userChosenBackgroundColorR.PreviewTextInput += NumberValidationTextBox;
        userChosenBackgroundColorDockPanel.Children.Add(userChosenBackgroundColorR);

        TextBox userChosenBackgroundColorG = new TextBox
        {
            Name = "UserChosenBackgroundColorG",
            Text = "Green",
            Margin = new Thickness(10, 10, 0, 0),
            Width = 40,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        userChosenBackgroundColorG.GotFocus += TextBox_GotFocus;
        userChosenBackgroundColorG.PreviewTextInput += NumberValidationTextBox;
        userChosenBackgroundColorDockPanel.Children.Add(userChosenBackgroundColorG);

        TextBox userChosenBackgroundColorB = new TextBox
        {
            Name = "UserChosenBackgroundColorB",
            Text = "Blue",
            Margin = new Thickness(10, 10, 0, 0),
            Width = 40,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        userChosenBackgroundColorB.GotFocus += TextBox_GotFocus;
        userChosenBackgroundColorB.PreviewTextInput += NumberValidationTextBox;
        userChosenBackgroundColorDockPanel.Children.Add(userChosenBackgroundColorB);
        mainStackPanel.Children.Add(userChosenBackgroundColorDockPanel);

        //Text Color
        TextBlock userChosenTextColorIntroduction = new TextBlock
        {
            Text = "Choose a text color",
            Margin = new Thickness(10, 10, 0, 0)
        };

        mainStackPanel.Children.Add(userChosenTextColorIntroduction);
        DockPanel userChosenTextColor = new DockPanel();
        TextBox userChosenTextColorR = new TextBox
        {
            Name = "UserChosenTextColorR",
            Text = "Red",
            Margin = new Thickness(10, 10, 0, 0),
            Width = 40,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        userChosenTextColorR.GotFocus += TextBox_GotFocus;
        userChosenTextColorR.PreviewTextInput += NumberValidationTextBox;
        userChosenTextColor.Children.Add(userChosenTextColorR);

        TextBox userChosenTextColorG = new TextBox
        {
            Name = "UserChosenTextColorG",
            Text = "Green",
            Margin = new Thickness(10, 10, 0, 0),
            Width = 40,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        userChosenTextColorG.GotFocus += TextBox_GotFocus;
        userChosenTextColorG.PreviewTextInput += NumberValidationTextBox;
        userChosenTextColor.Children.Add(userChosenTextColorG);

        TextBox userChosenTextColorB = new TextBox
        {
            Name = "UserChosenTextColorB",
            Text = "Blue",
            Margin = new Thickness(10, 10, 0, 0),
            Width = 40,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        userChosenTextColorB.GotFocus += TextBox_GotFocus;
        userChosenTextColorB.PreviewTextInput += NumberValidationTextBox;
        userChosenTextColor.Children.Add(userChosenTextColorB);

        mainStackPanel.Children.Add(userChosenTextColor);

        //Highlight Color
        TextBlock userChosenHighlightColorIntroduction = new TextBlock
        {
            Text = "Chose a Highlight Color",
            Margin = new Thickness(10,10,0,0)
        };
        mainStackPanel.Children.Add(userChosenHighlightColorIntroduction);

        DockPanel userChosenHighlightColor = new DockPanel();
        TextBox userChosenHighlightColorR = new TextBox
        {
            Name = "userChosenHighlightColorR",
            Text = "Red",
            Margin = new Thickness(10, 10, 0, 0),
            Width = 40,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        userChosenHighlightColorR.GotFocus += TextBox_GotFocus;
        userChosenHighlightColorR.PreviewTextInput += NumberValidationTextBox;
        userChosenHighlightColor.Children.Add(userChosenHighlightColorR);

        TextBox userChosenHighlightColorG = new TextBox
        {
            Name = "userChosenHighlightColorG",
            Text = "Green",
            Margin = new Thickness(10, 10, 0, 0),
            Width = 40,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        userChosenHighlightColorG.GotFocus += TextBox_GotFocus;
        userChosenHighlightColorG.PreviewTextInput += NumberValidationTextBox;
        userChosenHighlightColor.Children.Add(userChosenHighlightColorG);

        TextBox userChosenHighlightColorB = new TextBox
        {
            Name = "userChosenHighlightColorB",
            Text = "Blue",
            Margin = new Thickness(10, 10, 0, 0),
            Width = 40,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        userChosenHighlightColorB.GotFocus += TextBox_GotFocus;
        userChosenHighlightColorB.PreviewTextInput += NumberValidationTextBox;
        userChosenHighlightColor.Children.Add(userChosenHighlightColorB);

        mainStackPanel.Children.Add(userChosenHighlightColor);
        
        TextBox userChosenThemeName = new TextBox
        {
            Name = "userChosenThemeName",
            Text = "Theme Name",
            Margin = new Thickness(10, 10, 0, 0),
            Width = 200,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        userChosenThemeName.GotFocus += TextBox_GotFocus;
        RegisterName("userChosenThemeName", userChosenThemeName);
        mainStackPanel.Children.Add(userChosenThemeName);
        Button applyThemeButton = new Button
        {
            Content = "Add new theme",
            Margin = new Thickness(10, 10, 0, 0),
            Width = 100,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        applyThemeButton.Click += ApplyThemeButtonOnClick;
        mainStackPanel.Children.Add(applyThemeButton);

        MainSettingsStackPanel.Children.Add(mainStackPanel);
    }

    private bool CheckCommonEntryErrors(int entry)
    {
        TextBlock emptyValueError = new()
        {
            Text = "You left a RGB value empty or is negative",
            Foreground = new SolidColorBrush(Colors.Crimson)
        };
        TextBlock valueTooLargeError = new()
        {
            Text = "A value Entered was too large",
            Foreground = new SolidColorBrush(Colors.Crimson)
        };
        if (entry > 255)
        {
            MainSettingsStackPanel.Children.Add(valueTooLargeError);
            return false;
        }

        if (entry < 0)
        {
            MainSettingsStackPanel.Children.Add(emptyValueError);
            return false;
        }
        return true;
    }
    
    
    private void ApplyThemeButtonOnClick(object sender, RoutedEventArgs e)
    {
        bool continueProcess = true;
        StackPanel customThemeStackPanel = (StackPanel)FindName("CustomThemeStackPanel")!;
        Dictionary<string, string> customThemeTemporaryDictionary = new();
        List<string> customThemeBackgroundColorList = new();
        List<string> customThemeTextColorList = new();
        List<string> customThemeHighlightColorList = new();
        string customThemeNameDefault = "theme" + ThemeDictonary.Count + 1;
        string customThemeName = "";
        List<string> variableNameList = new()
        {
            "UserChosenBackgroundColorR",
            "UserChosenBackgroundColorG",
            "UserChosenBackgroundColorB",
            "UserChosenTextColorR",
            "UserChosenTextColorG",
            "UserChosenTextColorB",
            "userChosenHighlightColorR",
            "userChosenHighlightColorG",
            "userChosenHighlightColorB"
        };

        TextBlock nameAlreadyExistsError = new()
        {
            Text = "This theme name already exists",
            Foreground = new SolidColorBrush(Colors.Crimson)
        };
        TextBlock emptyValueError = new()
        {
            Text = "You left a RGB value empty or is negative",
            Foreground = new SolidColorBrush(Colors.Crimson)
        };
        foreach (var child in customThemeStackPanel.Children)
        {
            if(child is TextBox)
            {
                TextBox childTextBox = (child as TextBox)!; 
                if (childTextBox.Name == "userChosenThemeName")
                {
                    MessageBox.Show("It runs");
                    customThemeName = childTextBox.Text;
                    if (string.IsNullOrEmpty(customThemeName))
                    {
                        customThemeName = customThemeNameDefault;
                    }
                    if (ThemeDictonary.ContainsKey(customThemeName))
                    {
                        
                        MainSettingsStackPanel.Children.Add(nameAlreadyExistsError);
                        continueProcess = false;
                    }
                }
            }
            if (child is DockPanel)
            {
                DockPanel dockPanelChild = (child as DockPanel)!;
                foreach (var grandchild in dockPanelChild.Children)
                {
                    TextBox childTextBox = (grandchild as TextBox)!;
                    if (variableNameList.Contains(childTextBox.Name))
                    {
                        try
                        {
                            int childTextBoxContentTry = Int32.Parse(childTextBox.Text);
                        }
                        catch (FormatException)
                        {
                            MainSettingsStackPanel.Children.Add(emptyValueError);
                            return;
                        }
                        string childTextBoxName = childTextBox.Name;
                        if (variableNameList.IndexOf(childTextBoxName) < 3)
                        {
                            if (CheckCommonEntryErrors(int.Parse(childTextBox.Text)))
                            {
                                customThemeBackgroundColorList.Add(childTextBox.Text);
                            }
                            else
                            {
                                return;
                            }
                        }
                        else if (variableNameList.IndexOf(childTextBoxName) < 6)
                        {
                            if (CheckCommonEntryErrors(int.Parse(childTextBox.Text)))
                            {
                                customThemeTextColorList.Add(childTextBox.Text);
                            }
                            else
                            {
                                return;
                            }
                        }
                        else if (variableNameList.IndexOf(childTextBoxName) < 9)
                        {
                            if (CheckCommonEntryErrors(Int32.Parse(childTextBox.Text)))
                            {
                                customThemeHighlightColorList.Add(childTextBox.Text);
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }

        if (!continueProcess) return;
        string customThemeBackgroundColorString = String.Join(",", customThemeBackgroundColorList.ToArray());
        customThemeTemporaryDictionary.Add("themeBackgroundRgb", customThemeBackgroundColorString);

        string customThemeTextColorString = String.Join(",", customThemeTextColorList.ToArray());
        customThemeTemporaryDictionary.Add("themeTextRgb", customThemeTextColorString);

        string customThemeHighlightColorString = String.Join(",", customThemeHighlightColorList.ToArray());
        customThemeTemporaryDictionary.Add("themeHighlightRgb", customThemeHighlightColorString);

        ThemeDictonary.Add(customThemeName, customThemeTemporaryDictionary);
        MessageBox.Show("Restart App after applying settings for changes to apply");
        FillThemeJson();
    }

    private void DeleteTheme_OnClick(object sender, RoutedEventArgs e)
    {
        string themeToRemove = ThemeTemplateBox.Text;
        if (themeToRemove == "Dark Tan")
        {
            MessageBox.Show("You cannot remove the default theme");
            return;
        }

        if (string.IsNullOrEmpty(ThemeTemplateBox.Text))
        {
            MessageBox.Show("You must select the item you want to remove");
            return;
        }
        ThemeDictonary.Remove(ThemeTemplateBox.Text);
        MessageBox.Show("The Theme " + themeToRemove + "Has been removed");
        FillThemeJson();
    }
}


