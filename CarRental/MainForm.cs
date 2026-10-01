using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarRental
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(string login) : this()
        {
            пользовательToolStripStatusLabel.Text = "Пользователь: " + login;
        }

        private void автомобилиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CarsForm f = new CarsForm();
            f.Show();
        }

        private void клиентыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClientsForm f = new ClientsForm();
            f.Show();
        }

        private void договорыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RentalsForm f = new RentalsForm();
            f.Show();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Система управления прокатом автомобилей\nВерсия 1.0\n\nКурсовой проект",
                "О программе", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
