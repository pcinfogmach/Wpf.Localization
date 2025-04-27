using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace localization
{
    [MarkupExtensionReturnType(typeof(string))]
    public class LocaleExtension : MarkupExtension, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Text { get; set; }
        public string Translation { get => LocaleDictionary.Translate(Text);}

        public LocaleExtension(string text)
        { 
            if(!string.IsNullOrEmpty(text))
                Text = text;
        }

        public LocaleExtension() { }

        public void OnTranslationChanged() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Translation)));

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(Text))
                throw new InvalidOperationException("Text Cannot Be Blank");

            if (!LocaleDictionary.Bindings.TryGetValue(Text, out var binding))
            {
                binding = new Binding
                {
                    Source = this,
                    Path = new System.Windows.PropertyPath(nameof(Translation)),
                    Mode = BindingMode.OneWay
                };

                LocaleDictionary.Bindings.TryAdd(Text, binding);
            }

            return binding.ProvideValue(serviceProvider);
        }
    }
}
