using System.Windows;
using ProverkachekaSDK;

namespace ReceiptSaver
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string apiToken = "15239.20dUQQYmlHxbOPLzb";

        public MainWindow()
        {
            InitializeComponent();


        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            sumBox.Text = "Идёт поиск...";

            string qrRaw = qrRawBox.Text;
            ShowReceipt(qrRaw);
        }

        private async void ShowReceipt(string qrRaw)
        {
            Proverkacheka proverkacheka = new Proverkacheka(apiToken);
            Receipt receipt = await proverkacheka.GetAsync(qrRaw);

            sumBox.Text = receipt.TotalSum.ToString();
        }
    }
}
