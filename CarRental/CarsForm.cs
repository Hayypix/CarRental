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
    public partial class CarsForm : Form
    {
        public CarsForm()
        {
            InitializeComponent();
            LoadCars();
        }

        // загрузка списка автомобилей
        private void LoadCars()
        {
            try
            {
                DataTable dt = new DataTable();
                using (SQLiteConnection con = Database.GetConnection())
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter(
                        "SELECT Id, Brand, Model, Year, Plate, PricePerDay, Status FROM Cars", con);
                    da.Fill(dt);
                }
                dataGridView1.DataSource = dt;

                dataGridView1.Columns["Id"].Visible = false;
                dataGridView1.Columns["Brand"].HeaderText = "Марка";
                dataGridView1.Columns["Model"].HeaderText = "Модель";
                dataGridView1.Columns["Year"].HeaderText = "Год выпуска";
                dataGridView1.Columns["Plate"].HeaderText = "Гос. номер";
                dataGridView1.Columns["PricePerDay"].HeaderText = "Цена за сутки, руб.";
                dataGridView1.Columns["Status"].HeaderText = "Статус";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // добавить
        private void button1_Click(object sender, EventArgs e)
        {
            CarEditForm f = new CarEditForm();
            if (f.ShowDialog() == DialogResult.OK)
                LoadCars();
        }

        // изменить
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите автомобиль в списке", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
            CarEditForm f = new CarEditForm(id);
            if (f.ShowDialog() == DialogResult.OK)
                LoadCars();
        }

        // удалить
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите автомобиль в списке", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);

            if (MessageBox.Show("Удалить этот автомобиль?", "Удаление",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    using (SQLiteConnection con = Database.GetConnection())
                    {
                        con.Open();

                        // если по автомобилю есть договоры - удалять нельзя
                        SQLiteCommand cmdCheck = new SQLiteCommand(
                            "SELECT COUNT(*) FROM Rentals WHERE CarId = @id", con);
                        cmdCheck.Parameters.AddWithValue("@id", id);
                        int n = Convert.ToInt32(cmdCheck.ExecuteScalar());
                        if (n > 0)
                        {
                            MessageBox.Show("Нельзя удалить автомобиль, по нему есть договоры проката",
                                "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        SQLiteCommand cmd = new SQLiteCommand("DELETE FROM Cars WHERE Id = @id", con);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadCars();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message, "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // поиск по марке и модели
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            DataTable dt = dataGridView1.DataSource as DataTable;
            if (dt == null) return;

            string s = textBox1.Text.Replace("'", "''");
            dt.DefaultView.RowFilter = string.Format("Brand LIKE '%{0}%' OR Model LIKE '%{0}%'", s);
        }
    }
}
