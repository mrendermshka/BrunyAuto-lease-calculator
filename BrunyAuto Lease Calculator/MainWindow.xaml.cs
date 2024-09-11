using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Text;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using System.IO;
using System.Drawing;
using System.Windows.Media.Imaging;



namespace BrunyAuto_Lease_Calculator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            byte[] logoBytes = Properties.Resources.logo;
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream ms = new MemoryStream(logoBytes))
            {
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();  // Freeze to make it thread-safe and ready for use in UI.
            }
            this.Icon = bitmapImage;
            // Додаємо валюти в ComboBox
            Currency.ItemsSource = new List<string> { "USD", "EUR", "UAH" };
            Currency.SelectedIndex = 0; // Встановлюємо валюту за замовчуванням
        }

        private void Calculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Вхідні дані
                decimal beginPrice = decimal.Parse(BeginPrice.Text); // Початкова ціна
                decimal interestRate = decimal.Parse(InterestRate.Text) / 100; // Процентна ставка (річна)
                int durationMonths = int.Parse(DurationMonth.Text); // Термін лізингу у місяцях
                decimal casco = decimal.Parse(Casco.Text); // Вартість КАСКО
                decimal vat = decimal.Parse(VAT.Text) / 100; // Податок (VAT)
                decimal currencyToHrivnya = decimal.Parse(CurrencyToHrivnya.Text); // Курс валюти до гривні

                // Початковий внесок
                decimal firstPay;
                if (FirstPayPercentRB.IsChecked == true)
                {
                    firstPay = decimal.Parse(FirtsPay.Text) / 100 * beginPrice; // Відсоток від початкової ціни
                }
                else
                {
                    firstPay = decimal.Parse(FirtsPay.Text); // Фіксована сума
                }

                // Сума кредиту після початкового внеску
                decimal loanAmount = beginPrice - firstPay;

                // Місячна процентна ставка
                decimal monthlyInterestRate = interestRate / 12;

                // Ініціалізація результатів
                List<LeaseResult> results = new List<LeaseResult>();

                for (int month = 1; month <= durationMonths; month++)
                {
                    // Відсоток на залишок за поточний місяць
                    decimal interestForThisMonth = Math.Round(loanAmount * monthlyInterestRate, 2);

                    // Фіксований платіж (основна сума боргу)
                    decimal fixedPayment = Math.Round((beginPrice - firstPay) / durationMonths, 2);

                    // Загальний платіж (основна сума + відсотки)
                    decimal totalPayment = Math.Round(fixedPayment + interestForThisMonth, 2);

                    // Додаткові витрати: КАСКО + податок
                    decimal totalWithCasco = Math.Round(totalPayment + casco / durationMonths, 2);
                    decimal totalWithVAT = Math.Round(totalWithCasco * (1 + vat), 2);
                    // Зменшення залишкової суми після щомісячного платежу
                    loanAmount = Math.Round(loanAmount - fixedPayment, 2);
                    if (month == durationMonths)
                    {
                        totalWithVAT += loanAmount;
                        loanAmount = 0;
                    }

                    // Платежі в гривнях
                    decimal totalPaymentInHrivnya = totalWithVAT * currencyToHrivnya;
                    decimal interestInHrivnya = interestForThisMonth * currencyToHrivnya;


                    // Додавання даних до списку результатів
                    results.Add(new LeaseResult
                    {
                        Month = month,
                        Description = $"Місяць {month}",
                        Payment = totalWithVAT, // Загальний платіж з врахуванням податків
                        PaymentInHrivnya = totalPaymentInHrivnya, // Загальний платіж в гривнях
                        Interest = interestForThisMonth,
                        InterestInHrivnya = interestInHrivnya, // Відсотки в гривнях
                        Casco = Math.Round(casco / durationMonths, 2),
                        VAT = Math.Round(totalWithCasco * vat, 2),
                        RemainingBalance = loanAmount,
                        CurrencyToHrivnya = currencyToHrivnya,
                        Currency = Currency.Text,
                    });
                }

                // Оновлення DataGrid
                DataGrid.ItemsSource = results;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при розрахунку: " + ex.Message);
            }
        }
        private string GenerateHtmlFromDataGrid()
        {
            StringBuilder html = new StringBuilder();

            html.Append("<html><head><style>table { width: 100%; border-collapse: collapse; } th, td { border: 1px solid black; padding: 8px; text-align: left; } th { background-color: #f2f2f2; }</style></head><body>");

            // Add logo
            html.Append("<div style='text-align:center; margin-bottom:20px;'>");
            html.Append("<img src='https://bruny-auto.com/wp-content/uploads/2023/11/logo_png.png' alt='Company Logo' style='max-width:200px;'>");
            html.Append("</div>");

            html.Append("<h1>BrunyAuto Калькуляція Лізінгових платежів</h1>");
            html.Append("<h2>Початкові дані:</h2>");

            // Add initial data
            html.Append("<ul>");
            html.Append($"<li>Початкова ціна: {BeginPrice.Text}</li>");
            html.Append($"<li>Процентна ставка: {InterestRate.Text}</li>");
            html.Append($"<li>Строк лізингу (місяців): {DurationMonth.Text}</li>");
            html.Append($"<li>Ціна каско: {Casco.Text}</li>");
            html.Append($"<li>Податок: {VAT.Text}</li>");
            html.Append($"<li>Початковий внесок: {FirtsPay.Text} ({(FirstPayPercentRB.IsChecked == true ? "Відсоток" : "Сумма")})</li>");
            html.Append($"<li>Валюта: {Currency.Text}</li>");
            html.Append($"<li>Курс гривні до валюти: {CurrencyToHrivnya.Text}</li>");
            html.Append("</ul>");

            // Add table
            html.Append("<table><tr>");
            string[] headers = new string[] { "Місяць", "Опис", "Платіж (валюта)", "Платіж (грн)", "Відсотки (валюта)", "Відсотки (грн)", "КАСКО", "ПДВ", "Залишок боргу", "Сума до сплати в Валюті", "Сума до сплати в Гривні" };
            foreach (string header in headers)
            {
                html.Append($"<th>{header}</th>");
            }
            html.Append("</tr>");

            foreach (var item in DataGrid.Items)
            {
                var result = item as LeaseResult;
                if (result != null)
                {
                    html.Append("<tr>");
                    html.Append($"<td>{result.Month}</td>");
                    html.Append($"<td>{result.Description}</td>");
                    html.Append($"<td>{result.Payment}</td>");
                    html.Append($"<td>{result.PaymentInHrivnya}</td>");
                    html.Append($"<td>{result.Interest}</td>");
                    html.Append($"<td>{result.InterestInHrivnya}</td>");
                    html.Append($"<td>{result.Casco}</td>");
                    html.Append($"<td>{result.VAT}</td>");
                    html.Append($"<td>{result.RemainingBalance}</td>");
                    html.Append($"<td>{result.TotalAmountToPay}</td>");
                    html.Append($"<td>{result.TotalAmountToPayInHrivnya}</td>");
                    html.Append("</tr>");
                }
            }
            html.Append("</table>");
            html.Append("</body></html>");

            return html.ToString();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Generate HTML
                string html = GenerateHtmlFromDataGrid();
                
                // Save the HTML to a file
                string filename = "Lease_Calculation_Results.html";
                File.WriteAllText(filename, html);

                // Open the HTML file
                Process.Start(new ProcessStartInfo(filename) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка при створенні HTML: " + ex.Message);
            }

        }
    }

    public class LeaseResult
    {
        public int Month { get; set; }
        public string Description { get; set; }
        public decimal Payment { get; set; }
        public decimal PaymentInHrivnya { get; set; }
        public decimal Interest { get; set; }
        public decimal InterestInHrivnya { get; set; }
        public decimal Casco { get; set; }
        public decimal VAT { get; set; }
        public decimal RemainingBalance { get; set; }
        public decimal CurrencyToHrivnya { get; set; }

        public string Currency { get; set; }

        public decimal TotalAmountToPay
        {
            get
            {
                return Math.Round(Payment + Interest + VAT, 2);
            }
        }

        public decimal TotalAmountToPayInHrivnya
        {
            get
            {
                return (Math.Round(this.TotalAmountToPay * this.CurrencyToHrivnya, 2));
            }
        }
    }
}