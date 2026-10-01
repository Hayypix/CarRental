using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarRental
{
    public partial class RentalForm : Form
    {
        private DataTable carsTable;
        private DataTable clientsTable;
        private double price = 0;   // цена за сутки выбранного автомобиля

        public RentalForm()
        {
            InitializeComponent();
            LoadCars();
            LoadClients();

            dateTimePicker1.Value = DateTime.Today;
            dateTimePicker2.Value = DateTime.Today.AddDays(3);
            CalcPrice();
        }

        // список свободных автомобилей
        private void LoadCars()
        {
            carsTable = new DataTable();
            using (SQLiteConnection con = Database.GetConnection())
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter(
                    "SELECT Id, Brand, Model, Plate, PricePerDay " +
                    "FROM Cars WHERE Status = 'Свободен' ORDER BY Brand", con);
                da.Fill(carsTable);
            }

            carsTable.Columns.Add("Title", typeof(string), "Brand + ' ' + Model + ' [' + Plate + ']'");
            comboBox1.DataSource = carsTable;
            comboBox1.DisplayMember = "Title";
            comboBox1.ValueMember = "Id";
        }

        // список клиентов
        private void LoadClients()
        {
            clientsTable = new DataTable();
            using (SQLiteConnection con = Database.GetConnection())
            {
                SQLiteDataAdapter da = new SQLiteDataAdapter(
                    "SELECT Id, FullName FROM Clients ORDER BY FullName", con);
                da.Fill(clientsTable);
            }

            comboBox2.DataSource = clientsTable;
            comboBox2.DisplayMember = "FullName";
            comboBox2.ValueMember = "Id";
        }

        // расчёт предварительной стоимости
        private void CalcPrice()
        {
            int days = (dateTimePicker2.Value.Date - dateTimePicker1.Value.Date).Days;
            if (days < 1) days = 1;

            double sum = days * price;
            label6.Text = sum.ToString("0.##") + " руб. (" + days + " дн.)";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataRowView row = comboBox1.SelectedItem as DataRowView;
            if (row != null)
                price = Convert.ToDouble(row["PricePerDay"]);
            CalcPrice();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            CalcPrice();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            CalcPrice();
        }

        // оформить прокат
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Выберите автомобиль (нет свободных)", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Выберите клиента", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dateTimePicker2.Value.Date < dateTimePicker1.Value.Date)
            {
                MessageBox.Show("Дата возврата не может быть раньше даты выдачи", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int carId = Convert.ToInt32(((DataRowView)comboBox1.SelectedItem)["Id"]);
            int clientId = Convert.ToInt32(((DataRowView)comboBox2.SelectedItem)["Id"]);

            int days = (dateTimePicker2.Value.Date - dateTimePicker1.Value.Date).Days;
            if (days < 1) days = 1;
            double total = days * price;

            try
            {
                using (SQLiteConnection con = Database.GetConnection())
                {
                    con.Open();

                    SQLiteCommand cmd = new SQLiteCommand(
                        "INSERT INTO Rentals (CarId, ClientId, StartDate, EndDate, TotalCost, Status) " +
                        "VALUES (@car, @cl, @s, @e, @t, 'Открыт')", con);
                    cmd.Parameters.AddWithValue("@car", carId);
                    cmd.Parameters.AddWithValue("@cl", clientId);
                    cmd.Parameters.AddWithValue("@s", dateTimePicker1.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@e", dateTimePicker2.Value.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@t", total);
                    cmd.ExecuteNonQuery();

                    // автомобиль передан клиенту
                    cmd = new SQLiteCommand("UPDATE Cars SET Status = 'В аренде' WHERE Id = @id", con);
                    cmd.Parameters.AddWithValue("@id", carId);
                    cmd.ExecuteNonQuery();
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при оформлении проката: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // отмена
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
