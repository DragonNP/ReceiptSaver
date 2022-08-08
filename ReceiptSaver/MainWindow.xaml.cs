using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using ProverkachekaSDK;

namespace ReceiptSaver
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string apiToken;
        private readonly Proverkacheka proverkacheka;

        public MainWindow()
        {
            InitializeComponent();

            apiToken = "15239.20dUQQYmlHxbOPLzb";
            proverkacheka = new Proverkacheka(apiToken);

            goodsGrid.Visibility = Visibility.Hidden;
            selectFile.IsEnabled = false;
        }

        private async void searchButton_Click(object sender, RoutedEventArgs e)
        {
            searchButton.Content = "Идёт поиск...";
            searchButton.IsEnabled = false;
            goodsGrid.Visibility = Visibility.Hidden;

            string qrRaw = qrRawBox.Text;
            ShowReceipt(await GetReciept(qrRaw));
        }

        private void selectFile_Click(object sender, RoutedEventArgs e)
        {
        }

        private async Task<Receipt> GetReciept(string qrRaw)
        {
            return await proverkacheka.GetAsyncByRaw(qrRaw);
        }

        private void ShowReceipt(Receipt receipt)
        {
            if (receipt.Message != "")
            {
                MessageBox.Show(receipt.Message);

                searchButton.Content = "Найти чек";
                searchButton.IsEnabled = true;

                goodsGrid.Visibility = Visibility.Visible;

                return;
            }

            userLabel.Content = !string.IsNullOrEmpty(receipt.User) ? receipt.User : "Не указано";
            addresLabel.Content = !string.IsNullOrEmpty(receipt.Address) ? FormatAddress(receipt.Address) : "Не указано";
            dateLabel.Content = receipt.Date.ToString();

            switch (receipt.OperationType)
            {
                case 1:
                    operationLabel.Content = "Приход";
                    break;
                case 2:
                    operationLabel.Content = "Возврат прихода";
                    break;
                case 3:
                    operationLabel.Content = "Расход";
                    break;
                case 4:
                    operationLabel.Content = "Возврат расхода";
                    break;
                default:
                    operationLabel.Content = "Не указано";
                    break;
            }

            ShowGoods(receipt.Goods);

            searchButton.Content = "Найти чек";
            searchButton.IsEnabled = true;

            goodsGrid.Visibility = Visibility.Visible;
        }

        private void ShowGoods(List<Product> goods)
        {
            int position = 1;

            foreach (Product product in goods)
            {
                // Создание сетки для товара
                Grid grid = new Grid();
                grid.ColumnDefinitions.Add(CreateColumnDefinition(5, GridUnitType.Star));
                grid.ColumnDefinitions.Add(CreateColumnDefinition(50, GridUnitType.Star));
                grid.ColumnDefinitions.Add(CreateColumnDefinition(10, GridUnitType.Star));
                grid.ColumnDefinitions.Add(CreateColumnDefinition(10, GridUnitType.Star));
                grid.ColumnDefinitions.Add(CreateColumnDefinition(10, GridUnitType.Star));

                grid.Children.Add(CreateProductLabel(position.ToString(), HorizontalAlignment.Center, 0));
                grid.Children.Add(CreateProductLabel(product.Name, HorizontalAlignment.Left, 1));
                grid.Children.Add(CreateProductLabel(FormatPriceNum(product.Price), HorizontalAlignment.Right, 2));
                grid.Children.Add(CreateProductLabel(product.Quantity.ToString(), HorizontalAlignment.Left, 3));
                grid.Children.Add(CreateProductLabel(FormatPriceNum(product.Sum), HorizontalAlignment.Right, 4));

                goodsPanel.Children.Add(grid);
                position++;
            }
        }

        private string FormatAddress(string address)
        {
            while (address[address.Length - 1] == ',')
                address = address.Remove(address.Length - 1);

            address = address.Replace(",,", ", ");

            return address;
        }

        private string FormatPriceNum(int num)
        {
            string rub = (num / 100).ToString();
            string copeics = (num % 100).ToString().Length > 1 ? $"{num % 100}" : $"0{num % 100}";
            return $"{rub}.{copeics}руб.";
        }

        private Label CreateProductLabel(string content, HorizontalAlignment horizontalAlignment, int columnValue)
        {
            Label label = new Label()
            {
                Content = content,
                HorizontalContentAlignment = horizontalAlignment,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                FontSize = 14,
                FontWeight = FontWeights.Bold
            };
            Grid.SetColumn(label, columnValue);
            return label;
        }

        private ColumnDefinition CreateColumnDefinition(double value, GridUnitType type)
        {
            return new ColumnDefinition
            {
                Width = new GridLength(value, type)
            };
        }
    }
}
