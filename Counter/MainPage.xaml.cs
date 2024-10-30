using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;

namespace Counter
{
    public partial class MainPage : ContentPage
    {
        private const string CounterStorageKey = "counterStorage";

        public ObservableCollection<Counter> Counters { get; set; } = new();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            LoadCounters();
        }

        private async void OnAddCounterClicked(object sender, EventArgs e)
        {
            string counterName = $"Counter {Counters.Count + 1}";

            string result = await DisplayPromptAsync("Set Initial Value", $"Enter initial value for {counterName}",
                                                     initialValue: "0", keyboard: Keyboard.Numeric);

            if (int.TryParse(result, out int initialValue))
            {
                Counters.Add(new Counter { Name = counterName, Value = initialValue });
                SaveCounters();
            }
            else
            {
                await DisplayAlert("Invalid Input", "Please enter a valid integer.", "OK");
            }
        }

        private void OnIncreaseClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Counter counter)
            {
                counter.Value++;
                SaveCounters();
            }
        }

        private void OnDecreaseClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Counter counter)
            {
                counter.Value--;
                SaveCounters();
            }
        }

        private void SaveCounters()
        {
            var json = JsonSerializer.Serialize(Counters);
            Preferences.Set(CounterStorageKey, json);
        }

        private void LoadCounters()
        {
            if (Preferences.ContainsKey(CounterStorageKey))
            {
                var json = Preferences.Get(CounterStorageKey, string.Empty);
                if (!string.IsNullOrEmpty(json))
                {
                    var counters = JsonSerializer.Deserialize<ObservableCollection<Counter>>(json);
                    if (counters != null)
                    {
                        Counters.Clear();
                        foreach (var counter in counters)
                        {
                            Counters.Add(counter);
                        }
                    }
                }
            }
        }
    }

    public class Counter : BindableObject
    {
        public string Name { get; set; }

        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }
    }
}
