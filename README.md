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
change locale easily with command and commandproperty or with nextlocalecommand or directly through code

need to add a "Locale" Folder to your project
