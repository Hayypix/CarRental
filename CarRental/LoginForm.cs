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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        // кнопка Войти
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "" || textBox2.Text == "")
            {
                MessageBox.Show("Введите логин и пароль", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SQLiteConnection con = Database.GetConnection())
                {
                    con.Open();
                    SQLiteCommand cmd = new SQLiteCommand(
                        "SELECT COUNT(*) FROM Users WHERE Login = @login AND Password = @pass", con);
                    cmd.Parameters.AddWithValue("@login", textBox1.Text.Trim());
                    cmd.Parameters.AddWithValue("@pass", textBox2.Text);

                    int n = Convert.ToInt32(cmd.ExecuteScalar());
                    if (n > 0)
                    {
                        // вход выполнен
                        Hide();
                        MainForm main = new MainForm(textBox1.Text.Trim());
                        main.ShowDialog();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textBox2.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при входе: " + ex.Message, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // кнопка Выход
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
