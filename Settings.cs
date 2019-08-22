using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace HUB_Trade
{
    public partial class Settings : Form
    {
        string connect = "Server=127.0.0.1; Port=5432; User Id=postgres; Password=Telekom; Database=postgres";
        string update;
        public string currentUser;
        public Settings()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        Point lastPoint;
        private void dragM(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;
            }
        }

        private void dragD(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void updateValue(object sender, EventArgs e)
        {
            //currentSpeed.Text = textSpeed.Value.ToString();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            update = $@"SELECT * FROM public.user WHERE ""username"" = '{currentUser}';";

            NpgsqlConnection conn = new NpgsqlConnection(connect);
            conn.Open();
            NpgsqlCommand com = new NpgsqlCommand(update, conn);
            NpgsqlDataReader data = com.ExecuteReader();

            while (data.Read())
            {
                textSpeed.Value = Convert.ToInt32(data["fadeInSpeed"]);
                //currentSpeed.Text = textSpeed.Value.ToString();
            }

            //currentSpeed.Text = textSpeed.Value.ToString();
        }

        private void save_Click(object sender, EventArgs e)
        {
            TradeCenter tc = (TradeCenter)Owner;

            update = $@"UPDATE public.user SET ""fadeInSpeed"" = '{textSpeed.Value}' WHERE ""username"" = '{currentUser}'";
            NpgsqlConnection conn = new NpgsqlConnection(connect);
            conn.Open();
            NpgsqlCommand com = new NpgsqlCommand(update, conn);
            NpgsqlDataReader data = com.ExecuteReader();

            MessageBox.Show("Einstellungen gespeichert!");
            tc.speedUpdate(textSpeed.Value);
            this.Close();
        }
    }
}
