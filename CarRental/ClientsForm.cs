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
    public partial class ClientsForm : Form
    {
        public ClientsForm()
        {
            InitializeComponent();
            LoadClients();
        }

        // загрузка списка клиентов
        private void LoadClients()
        {
            try
            {
                DataTable dt = new DataTable();
                using (SQLiteConnection con = Database.GetConnection())
                {
                    SQLiteDataAdapter da = new SQLiteDataAdapter(
                        "SELECT Id, FullName, Phone, Passport, Address FROM Clients", con);
                    da.Fill(dt);
                }
                dataGridView1.DataSource = dt;

                dataGridView1.Columns["Id"].Visible = false;
                dataGridView1.Columns["FullName"].HeaderText = "ФИО";
                dataGridView1.Columns["Phone"].HeaderText = "Телефон";
                dataGridView1.Columns["Passport"].HeaderText = "Паспорт";
                dataGridView1.Columns["Address"].HeaderText = "Адрес";
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
            ClientEditForm f = new ClientEditForm();
            if (f.ShowDialog() == DialogResult.OK)
                LoadClients();
        }

        // изменить
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите клиента в списке", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
            ClientEditForm f = new ClientEditForm(id);
            if (f.ShowDialog() == DialogResult.OK)
                LoadClients();
        }

        // удалить
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Выберите клиента в списке", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);

            if (MessageBox.Show("Удалить этого клиента?", "Удаление",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    using (SQLiteConnection con = Database.GetConnection())
                    {
                        con.Open();

                        // если по клиенту есть договоры - удалять нельзя
                        SQLiteCommand cmdCheck = new SQLiteCommand(
                            "SELECT COUNT(*) FROM Rentals WHERE ClientId = @id", con);
                        cmdCheck.Parameters.AddWithValue("@id", id);
                        int n = Convert.ToInt32(cmdCheck.ExecuteScalar());
                        if (n > 0)
                        {
                            MessageBox.Show("Нельзя удалить клиента, по нему есть договоры проката",
                                "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        SQLiteCommand cmd = new SQLiteCommand("DELETE FROM Clients WHERE Id = @id", con);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                    LoadClients();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении: " + ex.Message, "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // поиск по ФИО
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            DataTable dt = dataGridView1.DataSource as DataTable;
            if (dt == null) return;

            string s = textBox1.Text.Replace("'", "''");
            dt.DefaultView.RowFilter = string.Format("FullName LIKE '%{0}%'", s);
        }
    }
}
