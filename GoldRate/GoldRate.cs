using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack; // Required for parsing HTML

namespace GoldRatesExtractor
{
    public partial class GoldRate : Form
    {
        private readonly string connectionString = "Data Source=LAPTOP-VF4TA7MH;Initial Catalog=GOLD2;User ID=sa;Password=1234;";

        public GoldRate()
        {
            InitializeComponent();
        }

        private async void btnSelectFile_Click(object sender, EventArgs e)
        {
            SetStatus("Please select a file...");

            using (OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select Data File",
                Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                FilterIndex = 1
            })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    await ProcessFileAsync(filePath);
                }
                else
                {
                    SetStatus("File selection cancelled.");
                }
            }
        }

        private async Task ProcessFileAsync(string filePath)
        {
            SetStatus("Processing file content...");

            try
            {
                string htmlContent = File.ReadAllText(filePath); // Read the entire file
                var goldRates = ExtractGoldRatesFromHtml(htmlContent); // Extract gold rates

                if (goldRates.Count == 0)
                {
                    SetStatus("No valid data found in the file.");
                    return;
                }

                await UpdateDatabaseAsync(goldRates);
            }
            catch (Exception ex)
            {
                SetStatus($"Processing error: {ex.Message}");
            }
        }

        private Dictionary<string, (decimal Buy, decimal Sell)> ExtractGoldRatesFromHtml(string htmlContent)
        {
            var goldRates = new Dictionary<string, (decimal Buy, decimal Sell)>();
            string[] expectedTypes = { "USD/oz", "MYR/kg", "MYR/tael", "MYR/g", "USD/MYR" };

            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(htmlContent); // Load the HTML

            var rows = htmlDoc.DocumentNode.SelectNodes("//table/tbody/tr");

            if (rows == null)
            {
                SetStatus("No table data found in the HTML.");
                return goldRates;
            }

            foreach (var row in rows)
            {
                var columns = row.SelectNodes("td");

                if (columns == null || columns.Count < 3)
                    continue;

                string currencyType = columns[0].InnerText.Trim();
                string buyStr = columns[1].InnerText.Trim().Replace(",", ""); // Remove commas for parsing
                string sellStr = columns[2].InnerText.Trim().Replace(",", "");

                if (!decimal.TryParse(buyStr, out decimal buyRate) || !decimal.TryParse(sellStr, out decimal sellRate))
                {
                    SetStatus($"Invalid data format: {currencyType} - {buyStr} / {sellStr}");
                    continue;
                }

                if (Array.Exists(expectedTypes, type => type.Equals(currencyType, StringComparison.OrdinalIgnoreCase)))
                {
                    goldRates[currencyType] = (buyRate, sellRate);
                }
            }

            foreach (var type in expectedTypes)
            {
                if (!goldRates.ContainsKey(type))
                {
                    SetStatus($"Missing data for {type}. Please check the file.");
                }
            }

            return goldRates;
        }

        private async Task UpdateDatabaseAsync(Dictionary<string, (decimal Buy, decimal Sell)> goldRates)
        {
            SetStatus("Updating database...");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    foreach (var type in goldRates.Keys)
                    {
                        (decimal buyRate, decimal sellRate) = goldRates[type];

                        using (SqlCommand command = new SqlCommand(
                            "UPDATE Rate_Gold SET Buy = @Buy, Sell = @Sell, UpdatedAt = @UpdatedAt WHERE CurrencyType = @CurrencyType",
                            connection))
                        {
                            command.Parameters.AddWithValue("@CurrencyType", type);
                            command.Parameters.AddWithValue("@Buy", buyRate);
                            command.Parameters.AddWithValue("@Sell", sellRate);
                            command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                            int rowsAffected = await command.ExecuteNonQueryAsync();

                            if (rowsAffected == 0)
                            {
                                SetStatus($"Warning: No record found for {type}.");
                            }
                        }
                    }
                }

                SetStatus("Database updated successfully!");
            }
            catch (Exception ex)
            {
                SetStatus($"Database update error: {ex.Message}");
            }
        }

        private void SetStatus(string message)
        {
            if (statusTextBox.InvokeRequired)
            {
                statusTextBox.Invoke(new Action(() => statusTextBox.Text = message));
            }
            else
            {
                statusTextBox.Text = message;
            }
        }
    }
}
