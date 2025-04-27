using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows.Data;

namespace localization
{
    public static class LocaleDictionary
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

        private static string _locale = "en";
        static ConcurrentDictionary<string, string> _translations;

        static string LocaleFolder =>
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Locale");
        public static List<string> LocaleList => (Directory.Exists(LocaleFolder)) ?
            Directory.GetFiles(LocaleFolder, "*.json").Select(Path.GetFileNameWithoutExtension).ToList() : null;
        public static string Locale
        {
            get => _locale;
            set
            {
                if (!string.IsNullOrEmpty(value) && value != _locale)
                    ChangeDictionary(value);                
            }
        }

        public static ConcurrentDictionary<string, Binding> Bindings = new ConcurrentDictionary<string, Binding>();
        public static ConcurrentDictionary<string, string> Translations
        {
            get
            {
                if (_translations == null)
                    LoadDictionary(Locale);
                return _translations;
            }
            private set
            {
                if (value != _translations)
                    _translations = value;
            }
        }

        public static RelayCommand<string> ChangeLocaleCommand = new RelayCommand<string>((value) => { Locale = value; });
        public static RelayCommand NextLocaleCommand = new RelayCommand(() => { NextLocale(); });

        public static void ChangeDictionary(string locale)
        {
            _locale = locale;
            LoadDictionary(locale);
            UpdateLocalization();
            OnStaticPropertyChanged(nameof(Locale));
        }

        static void LoadDictionary(string locale)
        {
            string filePath = GetLocaleFilePath();
            if (!File.Exists(filePath))
                return;

            string json = string.Empty;
            try { json = File.ReadAllText(filePath); } catch { }
            Translations = JsonSerializer.Deserialize<ConcurrentDictionary<string, string>>(json) ?? new ConcurrentDictionary<string, string>();
        }

        public static void NextLocale()
        {
            if (Directory.Exists(LocaleFolder))
            {
                var locales = LocaleList;

                if (locales.Count == 0)
                    return;

                int currentIndex = locales.IndexOf(Locale);

                if (currentIndex == -1 || currentIndex == locales.Count - 1)
                    Locale = locales[0];
                else
                    Locale = locales[currentIndex + 1];
            }
        }

        static string GetLocaleFilePath()
        {
            string localeFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), LocaleFolder);
            string filePath = Path.Combine(localeFolder, Locale + ".json");

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

        public static void UpdateLocalization()
        {
            foreach (var binding in Bindings.Values)
            {
                if (binding.Source is LocaleExtension localeExtension)
                    localeExtension.OnTranslationChanged();
            }
        }

        public static string Translate(string key)
        {
            if (Translations.TryGetValue(key, out string value))
                return value;
            return key;
        }

        private static void OnStaticPropertyChanged(string propertyName)
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }
    }
}
