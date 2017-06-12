using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Karinator.API;
using Karinator.Enums;
using static Karinator.Helpers.Helpers;

namespace Karinator
{
    public partial class MainWindow : Window
    {
        private readonly List<Algorithm> _algorithms;
        private readonly ObservableCollection<Node> _nodes = new ObservableCollection<Node>();
        private readonly Transformation _transformation;
        private Algorithm _currentAlgorithm = Algorithm.Aes;

        public MainWindow()
        {
            _transformation = new Transformation();
            _algorithms = Enum.GetValues(typeof(Algorithm)).Cast<Algorithm>().ToList();
            InitializeComponent();
            algorithmType.ItemsSource = _algorithms;
            algorithmType.SelectedIndex = 0;
        }

        private void button_SelectFiles(object sender, RoutedEventArgs e)
        {
            GetNodes(_nodes);
            listView.ItemsSource = _nodes;
        }

        private void button_Encrypt(object sender, RoutedEventArgs e)
        {
            if (_nodes.Count == 0) return;

            var keys = AlgorithmsManager.GenerateKeys(_currentAlgorithm);
            vectorText.Text += keys.GetIV();
            passwordBox.Text += keys.GetKey();

            SetButtonsOn(false);

            Task.Factory.ContinueWhenAll(
                _transformation.Transform(_nodes.ToList(), CryptoStreamMode.Write, _currentAlgorithm).ToArray(),
                t => SetButtonsOn(true),
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void button_Decrypt(object sender, RoutedEventArgs e)
        {
            var key = passwordBox.Text;
            var vector = vectorText.Text;

            if (_nodes.Count == 0) return;
            if (!(key.Length > 0) || !(vector.Length > 0)) return;

            AlgorithmsManager.SetKeys(new Keys(key, vector), _currentAlgorithm);

            SetButtonsOn(false);

            Task.Factory.ContinueWhenAll(
                _transformation.Transform(_nodes.ToList(), CryptoStreamMode.Read, _currentAlgorithm).ToArray(),
                t => SetButtonsOn(true),
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void SetButtonsOn(bool flag)
        {
            buttonDecrypt.IsEnabled = flag;
            buttonEncrypt.IsEnabled = flag;
        }

        private void algorithmType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentAlgorithm = _algorithms[algorithmType.SelectedIndex];
        }
    }
}
