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
    public partial class CarEditForm : Form
    {
        private int carId = 0;   // если 0 - добавление нового автомобиля

        public CarEditForm()
        {
            InitializeComponent();
        }

        // конструктор для редактирования
        public CarEditForm(int id) : this()
        {
            carId = id;
            Text = "Редактирование автомобиля";

            try
            {
                using (SQLiteConnection con = Database.GetConnection())
                {
                    con.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM Cars WHERE Id = @id", con);
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            textBox1.Text = dr["Brand"].ToString();
                            textBox2.Text = dr["Model"].ToString();
                            textBox3.Text = dr["Year"].ToString();
                            textBox4.Text = dr["Plate"].ToString();
                            textBox5.Text = Convert.ToDouble(dr["PricePerDay"]).ToString("0.##");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // сохранить
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "" || textBox2.Text.Trim() == "")
            {
                MessageBox.Show("Заполните марку и модель", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            double price;
            if (!double.TryParse(textBox5.Text.Replace('.', ','), out price) || price <= 0)
            {
                MessageBox.Show("Цена за сутки указана неверно", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int year = 0;
            if (textBox3.Text.Trim() != "")
            {
                if (!int.TryParse(textBox3.Text, out year) || year < 1950 || year > DateTime.Now.Year)
                {
                    MessageBox.Show("Год выпуска указан неверно", "Внимание",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            try
            {
                using (SQLiteConnection con = Database.GetConnection())
                {
                    con.Open();

                    string sql;
                    if (carId == 0)
                        sql = "INSERT INTO Cars (Brand, Model, Year, Plate, PricePerDay) " +
                              "VALUES (@b, @m, @y, @p, @c)";
                    else
                        sql = "UPDATE Cars SET Brand = @b, Model = @m, Year = @y, " +
                              "Plate = @p, PricePerDay = @c WHERE Id = @id";

                    SQLiteCommand cmd = new SQLiteCommand(sql, con);
                    cmd.Parameters.AddWithValue("@b", textBox1.Text.Trim());
                    cmd.Parameters.AddWithValue("@m", textBox2.Text.Trim());
                    if (year > 0)
                        cmd.Parameters.AddWithValue("@y", year);
                    else
                        cmd.Parameters.AddWithValue("@y", DBNull.Value);
                    cmd.Parameters.AddWithValue("@p", textBox4.Text.Trim());
                    cmd.Parameters.AddWithValue("@c", price);
                    if (carId != 0)
                        cmd.Parameters.AddWithValue("@id", carId);
                    cmd.ExecuteNonQuery();
                }

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка",
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
