# Wpf localization
Easy localization For wpf using json.

sample usage

add xmlns refrence and then

```
<Button Width="200" Height="50"
        locale:LocalizationExtension.Key="LoginButton"
        Command="{x:Static locale:LocalizationExtension.NextLocaleCommand}"  
```

features:
change locale easily with changeLocale command and commandparameter, or with nextlocalecommand or directly through code
is static class so you can interact with it from anywhere in your code including through a viewmodel.

note:
Requires a "Locale" Folder in your project


