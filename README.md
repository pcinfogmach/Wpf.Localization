# Wpf localization

Easy localization for WPF using JSON.

*Features:*
Change locale easily using the ChangeLocale command with the CommandParameter, or use NextLocaleCommand, or directly manipulate it through code.

It is a static class, allowing you to interact with it from anywhere in your code, including through a ViewModel.

*Note:*
The json files should be placed in "Locale" folder in your project.
```
<Window x:Class="localizationTestApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:localizationTestApp"
        xmlns:loc="clr-namespace:localization;assembly=localization"
        mc:Ignorable="d"
        Title="MainWindow" Height="450" Width="800">

    <StackPanel VerticalAlignment="Center" HorizontalAlignment="Center">

        <Button Width="200" Height="50"
                Content="{loc:LocaleExtension Text=Login}"
                Command="{x:Static loc:LocaleDictionary.NextLocaleCommand}"/>

        <Button Width="200" Height="50"
          Content="Load Hebrew"
          Command="{x:Static loc:LocaleDictionary.ChangeLocaleCommand}"
                CommandParameter="he"/>
        
        <TextBlock Margin="10" 
                   Text="{loc:LocaleExtension Text=Welcome}"/>

        <TextBox Width="200"
                 Text="{loc:LocaleExtension Text=Placeholder}"/>

        <ComboBox Width="200" IsEditable="True"
                  ItemsSource="{x:Static loc:LocaleDictionary.LocaleList}"
                  Text="{Binding Path=(loc:LocaleDictionary.Locale)}"/>
    </StackPanel>
</Window>
 
```
sample json
```
{
  "Login": "Conectar",
  "Welcome": "Bienvenido!",
  "Placeholder": "Escribe aquí..."
}
```




