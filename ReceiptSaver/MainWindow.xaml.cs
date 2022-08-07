using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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
            searchButton.Content = "Идёт поиск...";
            searchButton.IsEnabled = false;

            string qrRaw = qrRawBox.Text;
            ShowReceipt(qrRaw);
        }

        private async void ShowReceipt(string qrRaw)
        {
            Proverkacheka proverkacheka = new Proverkacheka(apiToken);
            Receipt receipt = await proverkacheka.GetAsync(qrRaw);

            userBox.Text = !string.IsNullOrEmpty(receipt.User) ? receipt.User : "Не указано";
            addressBox.Text = !string.IsNullOrEmpty(receipt.Address) ? receipt.Address : "Не указано";
            nds20Box.Text = receipt.Nds20 > 0 ? $"{receipt.Nds20 / 100} руб." : "Нет";
            nds10Box.Text = receipt.Nds10 > 0 ? $"{receipt.Nds10 / 100} руб." : "Нет";
            ndsNoBox.Text = receipt.NdsNo > 0 ? $"{receipt.NdsNo / 100} руб." : "Нет";
            regionBox.Text = receipt.Region > 0 ? receipt.Region.ToString() : "Не указано";
            dateBox.Text = receipt.Date.ToString();
            placeBox.Text = !string.IsNullOrEmpty(receipt.RetailPlace) ? receipt.RetailPlace : "Не указано";
            sumBox.Text = $"{receipt.TotalSum / 100} руб. (нал:{receipt.CashTotalSum / 100}руб., безнал:{receipt.EcashTotalSum / 100}руб.)";

            switch (receipt.TaxationType)
            {
                case 1:
                    taxationBox.Text = "ОСН";
                    break;
                case 2:
                    taxationBox.Text = "УСН";
                    break;
                case 4:
                    taxationBox.Text = "УСН доход - расход";
                    break;
                case 8:
                    taxationBox.Text = "ЕНВД";
                    break;
                case 16:
                    taxationBox.Text = "ЕСХН";
                    break;
                case 32:
                    taxationBox.Text = "ПСН";
                    break;
                default:
                    taxationBox.Text = "Не указано";
                    break;
            }

            switch (receipt.OperationType)
            {
                case 1:
                    operationBox.Text = "Приход";
                    break;
                case 2:
                    operationBox.Text = "Возврат прихода";
                    break;
                case 3:
                    operationBox.Text = "Расход";
                    break;
                case 4:
                    operationBox.Text = "Возврат расхода";
                    break;
                default:
                    operationBox.Text = "Не указано";
                    break;
            }

            ShowGoods(receipt.Goods);

            searchButton.Content = "Найти чек";
            searchButton.IsEnabled = true;
        }

        private void ShowGoods(List<Product> goods)
        {
            foreach (Product product in goods)
            {
                TextBox box = new TextBox();

                box.Text = product.Name;

                goodsPlace.Children.Add(box);
            }
        }
    }
}
