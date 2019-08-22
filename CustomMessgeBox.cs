using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ButtonMadness
{
    public partial class ld_success : Form
    {
        //Indikator für die Aktion die nach dem beenden des aktuellen Levels ausgeführt wird
        //Nimmt Wert zwischen 0 und 2 an --> 0 = Weiter, 1 = Neustart des Levels, 2 = Zurück zum Hauptmenü
        public int mb_state;

        public ld_success()
        {
            InitializeComponent();
        }

        //Weiter Button
        private void rt_Click(object sender, EventArgs e)
        {
            mb_state = 0;
            this.Close();
        }

        private void rs_Click(object sender, EventArgs e)
        {
            mb_state = 1;
            this.Close();
        }

        private void qu_Click(object sender, EventArgs e)
        {
            mb_state = 2;
            this.Close();
        }
    }
}
