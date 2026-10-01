using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarRental
{
    public partial class RentalsForm : Form
    {
        public RentalsForm()
        {
            InitializeComponent();
            LoadRentals();
        }

        // загрузка списка договоров
        private void LoadRentals()
        {
            try
            {
                DataTable dt = new DataTable();
                string sql =
                    "SELECT r.Id, r.CarId, r.ClientId, " +
                    "c.Brand || ' ' || c.Model AS CarName, cl.FullName AS ClientName, " +
                    "strftime('%d.%m.%Y', r.StartDate) AS StartDate, " +
                    "strftime('%d.%m.%Y', r.EndDate) AS EndDate, " +
                    "strftime('%d.%m.%Y', r.ReturnDate) AS ReturnDate, " +
                    "r.TotalCost, r.Status, c.PricePerDay, r.StartDate AS StartDateRaw " +
                    "FROM Rentals r, Cars c, Clients cl " +
                    "WHERE r.CarId = c.Id AND r.ClientId = cl.Id " +
                    "ORDER BY r.Id DESC";

                using (SQLiteConnection con = Database.GetConnection())
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter(sql, con);
                    da.Fill(dt);
                }
                dataGridView1.DataSource = dt;

                dataGridView1.Columns["Id"].Visible = false;
                dataGridView1.Columns["CarId"].Visible = false;
                dataGridView1.Columns["ClientId"].Visible = false;
                dataGridView1.Columns["PricePerDay"].Visible = false;
                dataGridView1.Columns["StartDateRaw"].Visible = false;

                dataGridView1.Columns["CarName"].HeaderText = "Автомобиль";
                dataGridView1.Columns["ClientName"].HeaderText = "Клиент";
                dataGridView1.Columns["StartDate"].HeaderText = "Дата выдачи";
                dataGridView1.Columns["EndDate"].HeaderText = "План. возврат";
                dataGridView1.Columns["ReturnDate"].HeaderText = "Факт. возврат";
                dataGridView1.Columns["TotalCost"].HeaderText = "Сумма, руб.";
                dataGridView1.Columns["Status"].HeaderText = "Статус";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // оформить новый прокат
        private void button1_Click(object sender, EventArgs e)
        {
            RentalForm f = new RentalForm();
            if (f.ShowDialog() == DialogResult.OK)
                LoadRentals();
        }

        // возврат автомобиля
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите договор в списке", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow row = dataGridView1.SelectedRows[0];

            if (row.Cells["Status"].Value.ToString() == "Закрыт")
            {
                MessageBox.Show("Этот договор уже закрыт", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string carName = row.Cells["CarName"].Value.ToString();

            if (MessageBox.Show("Оформить возврат автомобиля " + carName + "?", "Возврат",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int id = Convert.ToInt32(row.Cells["Id"].Value);
                    int carId = Convert.ToInt32(row.Cells["CarId"].Value);
                    double price = Convert.ToDouble(row.Cells["PricePerDay"].Value);

                    // сколько дней прошло с выдачи (минимум 1 день)
                    DateTime start = DateTime.ParseExact(row.Cells["StartDateRaw"].Value.ToString(),
                        "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    int days = (DateTime.Today - start).Days;
                    if (days < 1) days = 1;

                    double total = days * price;

                    using (SQLiteConnection con = Database.GetConnection())
                    {
                        con.Open();

                        SQLiteCommand cmd = new SQLiteCommand(
                            "UPDATE Rentals SET ReturnDate = @d, TotalCost = @t, Status = 'Закрыт' " +
                            "WHERE Id = @id", con);
                        cmd.Parameters.AddWithValue("@d", DateTime.Today.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@t", total);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();

                        // автомобиль снова свободен
                        cmd = new SQLiteCommand("UPDATE Cars SET Status = 'Свободен' WHERE Id = @car", con);
                        cmd.Parameters.AddWithValue("@car", carId);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Автомобиль возвращён.\nСрок проката: " + days +
                        " дн.\nИтого к оплате: " + total.ToString("0.##") + " руб.",
                        "Возврат", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadRentals();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при возврате: " + ex.Message, "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
