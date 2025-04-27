using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Localization
{
    public static class LocalizationExtension
    {
        private static string _locale = "en";
        private static readonly string _localeFolder = "Locale";
        private static ConcurrentDictionary<string, string> translations;
        private static readonly List<WeakReference<DependencyObject>> RegisteredElements = new List<WeakReference<DependencyObject>>();

        public static string Locale
        {
            get => _locale;
            set
            {
                if (value != _locale) 
                {
                    _locale = value; 
                    UpdateAllRegisteredElements();
                }               
            }
        }

        public static readonly DependencyProperty KeyProperty =
           DependencyProperty.RegisterAttached(
               "Key",
               typeof(string),
               typeof(LocalizationExtension),
               new PropertyMetadata(null, OnKeyChanged));

        public static void SetKey(DependencyObject element, string value) =>
            element.SetValue(KeyProperty, value);

        public static string GetKey(DependencyObject element) =>
            (string)element.GetValue(KeyProperty);

        private static void OnKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            SetText(d, (string)e.NewValue);

            CleanupRegisteredElements();
            RegisteredElements.Add(new WeakReference<DependencyObject>(d));
            
        }

        public static RelayCommand<string> ChangeLocaleCommand = new RelayCommand<string>((value) => { Locale = value; } );
        public static RelayCommand NextLocaleCommand = new RelayCommand(() => { NextLocale(); });
        public static void NextLocale()
        {
            string localeFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _localeFolder);
            if (Directory.Exists(localeFolder))
            {
                var locales = Directory.GetFiles(localeFolder, "*.json")
                                        .Select(Path.GetFileNameWithoutExtension)
                                        .ToList();

                if (locales.Count == 0)
                    return;

                int currentIndex = locales.IndexOf(Locale);

                if (currentIndex == -1 || currentIndex == locales.Count - 1)
                    Locale = locales[0];
                else
                    Locale = locales[currentIndex + 1];
            }
        }



        private static void UpdateAllRegisteredElements()
        {
            LoadTranslations();
            CleanupRegisteredElements();

            foreach (var weakReference in RegisteredElements)
            {
                if (weakReference.TryGetTarget(out var target))
                {
                    var key = GetKey(target);
                    SetText(target, key);
                }
            }
        }

        private static void CleanupRegisteredElements() =>
            RegisteredElements.RemoveAll(wr => !wr.TryGetTarget(out _));

        public static void LoadTranslations()
        {
            string filePath = GetLocaleFilePath();

            try
            {
                string json = File.ReadAllText(filePath);
                translations = JsonSerializer.Deserialize<ConcurrentDictionary<string, string>>(json) ?? new ConcurrentDictionary<string, string>();
            }
            catch
            {
                translations = new ConcurrentDictionary<string, string>();
            }

        }

        static string GetLocaleFilePath()
        {
            string localeFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _localeFolder);
            string filePath = Path.Combine(localeFolder, _locale + ".json");

            if (!File.Exists(filePath))
            {
                if (Directory.Exists(localeFolder))
                {
                    string[] files = Directory.GetFiles(localeFolder, "*.json");
                    if (files.Length > 0)
                        filePath = filePath = files[0];
                    else
                        filePath = null;
                }
                else
                    filePath = null;
            }

            return filePath;
        }

        public static void SetText(DependencyObject d, string key)
        {
            if (d == null || string.IsNullOrEmpty(key))
                return;

            if (translations == null)
                LoadTranslations();

            if (translations.TryGetValue(key, out var translation))
            {
                switch (d)
                {
                    case ContentControl contentControl:
                        contentControl.Content = translation;
                        break;
                    default:
                        var textProperty = d.GetType().GetProperty("Text");
                        textProperty?.SetValue(d, translation);
                        break;
                }
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke((T)parameter) ?? true;

        public void Execute(object parameter) => _execute((T)parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
