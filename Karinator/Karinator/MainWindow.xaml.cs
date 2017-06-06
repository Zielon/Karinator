using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Karinator
{
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<Node> _nodes = new ObservableCollection<Node>();
        private readonly Transformation _transformation;

        public MainWindow()
        {
            _transformation = new Transformation();
            InitializeComponent();
        }

        private void button_SelectFiles(object sender, RoutedEventArgs e)
        {
            Helpers.GetNodes(_nodes);
            listView.ItemsSource = _nodes;
        }

        private void button_Encrypt(object sender, RoutedEventArgs e)
        {
            const Algorithm algorithm = Algorithm.Aes;

            if (_nodes.Count == 0) return;

            var keys = AlgorithmsManager.GenerateKeys(algorithm);
            vectorText.Text += keys.GetIV();
            passwordBox.Text += keys.GetKey();

            SetButtonsOn(false);

            Task.Factory.ContinueWhenAll(
                _transformation.Transform(_nodes.ToList(), CryptoStreamMode.Write, algorithm).ToArray(),
                t => SetButtonsOn(true),
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void button_Decrypt(object sender, RoutedEventArgs e)
        {
            const Algorithm algorithm = Algorithm.Aes;

            var key = passwordBox.Text;
            var vector = vectorText.Text;

            if (_nodes.Count == 0) return;
            if (!(key.Length > 0) || !(vector.Length > 0)) return;

            AlgorithmsManager.SetKeys(new Keys(key, vector), algorithm);

            SetButtonsOn(false);

            Task.Factory.ContinueWhenAll(
                _transformation.Transform(_nodes.ToList(), CryptoStreamMode.Read, algorithm).ToArray(),
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
    }
}
