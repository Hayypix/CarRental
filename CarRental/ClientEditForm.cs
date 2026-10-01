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
    public partial class ClientEditForm : Form
    {
        private int clientId = 0;   // если 0 - добавление нового клиента

        public ClientEditForm()
        {
            InitializeComponent();
        }

        // конструктор для редактирования
        public ClientEditForm(int id) : this()
        {
            clientId = id;
            Text = "Редактирование клиента";

            try
            {
                using (SQLiteConnection con = Database.GetConnection())
                {
                    con.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM Clients WHERE Id = @id", con);
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            textBox1.Text = dr["FullName"].ToString();
                            textBox2.Text = dr["Phone"].ToString();
                            textBox3.Text = dr["Passport"].ToString();
                            textBox4.Text = dr["Address"].ToString();
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
            if (textBox1.Text.Trim() == "")
            {
                MessageBox.Show("Заполните ФИО клиента", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SQLiteConnection con = Database.GetConnection())
                {
                    con.Open();

                    string sql;
                    if (clientId == 0)
                        sql = "INSERT INTO Clients (FullName, Phone, Passport, Address) " +
                              "VALUES (@f, @ph, @pa, @ad)";
                    else
                        sql = "UPDATE Clients SET FullName = @f, Phone = @ph, " +
                              "Passport = @pa, Address = @ad WHERE Id = @id";

                    SQLiteCommand cmd = new SQLiteCommand(sql, con);
                    cmd.Parameters.AddWithValue("@f", textBox1.Text.Trim());
                    cmd.Parameters.AddWithValue("@ph", textBox2.Text.Trim());
                    cmd.Parameters.AddWithValue("@pa", textBox3.Text.Trim());
                    cmd.Parameters.AddWithValue("@ad", textBox4.Text.Trim());
                    if (clientId != 0)
                        cmd.Parameters.AddWithValue("@id", clientId);
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
