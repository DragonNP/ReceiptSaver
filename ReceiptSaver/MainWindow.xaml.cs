﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using IronBarCode;
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
        private readonly int NormalFontSize = 14;
        private readonly FontWeight NormalFontWeight = FontWeights.Normal;

        public MainWindow()
        {
            InitializeComponent();

            apiToken = "15239.20dUQQYmlHxbOPLzb";
            proverkacheka = new Proverkacheka(apiToken);

            goodsGrid.Visibility = Visibility.Hidden;
            //selectFile.IsEnabled = false;
        }

        private async void searchButton_Click(object sender, RoutedEventArgs e)
        {
            searchButton.Content = "Идёт поиск...";
            searchButton.IsEnabled = false;
            goodsGrid.Visibility = Visibility.Hidden;
            selectFile.IsEnabled = false;

            string qrRaw = qrRawBox.Text;
            Receipt receipt = await GetReciept(qrRaw);

            ShowTitle(receipt);
            ShowGoods(receipt.Goods);
            ShowFooter(receipt);

            searchButton.Content = "Найти чек";
            searchButton.IsEnabled = true;
            selectFile.IsEnabled = true;
            goodsGrid.Visibility = Visibility.Visible;
        }

        private void selectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Фотографии (*.png, *jpg)|*.png;*.jpg";
            dialog.Multiselect = false;
            dialog.Title = "Выберите файл с QR кодом";
            dialog.RestoreDirectory = true;

            if ((bool)dialog.ShowDialog())
            {
                string filename = dialog.FileName;
                qrRawBox.Text = filename;

                BarcodeResults result = BarcodeReader.Read(filename);

                qrRawBox.Text = result.Values()[0];
                searchButton_Click(sender, e);
            }
        }

        private async Task<Receipt> GetReciept(string qrRaw)
        {
            return await proverkacheka.GetAsyncByRaw(qrRaw);
        }

        private void ShowTitle(Receipt receipt)
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
            dateLabel.Content = receipt.Date.ToString("MM.dd.yyyy HH:mm");

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

                // Добавление товара в сетку
                grid.Children.Add(CreateProductLabel(position.ToString(), HorizontalAlignment.Center, 0));
                grid.Children.Add(CreateProductLabel(product.Name, HorizontalAlignment.Left, 1));
                grid.Children.Add(CreateProductLabel(FormatMoney(product.Price), HorizontalAlignment.Right, 2));
                grid.Children.Add(CreateProductLabel(product.Quantity.ToString(), HorizontalAlignment.Center, 3));
                grid.Children.Add(CreateProductLabel(FormatMoney(product.Sum), HorizontalAlignment.Right, 4));

                goodsPanel.Children.Add(grid);
                position++;
            }
        }

        private void ShowFooter(Receipt receipt)
        {
            AddContentToFooter("ИТОГО:", FormatMoney(receipt.TotalSum), 18, FontWeights.Bold);
            AddContentToFooter("Наличными", FormatMoney(receipt.CashTotalSum));
            AddContentToFooter("Карта", FormatMoney(receipt.EcashTotalSum));

            // НДС 20%
            if (receipt.Nds20 != 0)
                AddContentToFooter("НДС 20%", FormatMoney(receipt.Nds20));

            // НДС 10%
            if (receipt.Nds10 != 0)
                AddContentToFooter("НДС 10%", FormatMoney(receipt.Nds10));

            // НДС не облагается
            if (receipt.NdsNo != 0)
                AddContentToFooter("НДС не облагается", FormatMoney(receipt.NdsNo));
        }

        private string FormatAddress(string address)
        {
            while (address[address.Length - 1] == ',')
                address = address.Remove(address.Length - 1);

            address = address.Replace(",,", ",");
            address = address.Replace(", ", ",");
            address = address.Replace(" ,", ",");
            address = address.Replace(",", ", ");


            return address;
        }

        private string FormatMoney(int num)
        {
            string rub = (num / 100).ToString();
            string copeics = (num % 100).ToString().Length > 1 ? $"{num % 100}" : $"0{num % 100}";
            return $"{rub}.{copeics}";
        }

        private void AddContentToFooter(string name, string content, int fontFize, FontWeight fontWeight)
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(CreateColumnDefinition(0, GridUnitType.Auto));
            grid.ColumnDefinitions.Add(CreateColumnDefinition(1, GridUnitType.Star));

            Label nameLabel = new Label()
            {
                Content = name,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                FontSize = fontFize,
                FontWeight = fontWeight,
            };
            Grid.SetColumn(nameLabel, 0);
            grid.Children.Add(nameLabel);

            Label contentLabel = new Label()
            {
                Content = content,
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                FontSize = fontFize,
                FontWeight = fontWeight,
            };
            Grid.SetColumn(contentLabel, 1);
            grid.Children.Add(contentLabel);

            footerPanel.Children.Add(grid);
        }

        private void AddContentToFooter(string name, string content) => AddContentToFooter(name, content, NormalFontSize, NormalFontWeight);

        private Label CreateProductLabel(string content, HorizontalAlignment horizontalAlignment, int columnValue)
        {
            Label label = new Label()
            {
                Content = content,
                HorizontalContentAlignment = horizontalAlignment,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontFamily = new FontFamily("Arial"),
                FontSize = NormalFontSize,
                FontWeight = NormalFontWeight,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0, 0, 0, 0.3),
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
