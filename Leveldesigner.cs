using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ButtonMadness
{
    public partial class Leveldesigner : Form
    {
        private Live_Vorschau___ButtonMadness lv;

        //Variablen aus Leveldesigner werden hier zwischengespeichert, und bei Bedarf in Form1 = Hauptprogramm übertragen

        public string levelname;
        public int heigth;
        public int width;
        public int s_top;
        public int s_left;
        public int invisible;
        public double duration;

        public bool rbp;
        public int bounce;


        public int btn_inv;
        public int btn_inv_rnd;


        public bool ld_play;
        public bool transp;

        //Vollbild
        public bool fs = false;
        public static MainMenu form;

        public TextBox text;
        public Leveldesigner()
        {
            InitializeComponent();
        }

        public string customLevelActivate
        {
            get { return ln.Text; }
        }

        public void save_settings_Click(object sender, EventArgs e)
        {
            try
            {
                if (ln.Text == "")
                {
                    levelname = "Benutzerdefiniert";
                }
                else
                {
                    levelname = ln.Text;
                }

                heigth = Convert.ToInt16(btn_heigth.Text);
                width = Convert.ToInt16(btn_width.Text);
                s_top = Convert.ToInt16(btn_speed_top.Text);
                s_left = Convert.ToInt16(btn_speed_left.Text);

                //Zufällige Startposition? Ja:Nein

                if (rnd_sp.Checked)
                {
                    rbp = true;
                }
                else
                {
                    rbp = false;
                }

                //Zufälliger Abbrallwinkell? Ja:Nein

                if (rnd_bounce.Checked)
                {
                    bounce = 1;
                }
                else
                {
                    bounce = 0;
                }

                //Ermittelt Status des Zufallwertes auf Grundlage der Ausgewählten Checkboxen im Abschnitt "Unsichtbar machen des Buttons"

                if (invisible_btn.Checked)
                {
                    if (invisible_btn_tb.Checked && invisible_btn_lr.Checked == false)
                    {
                        btn_inv = 1; //Bei Aufprall oben und unten
                    }

                    if (invisible_btn_lr.Checked && invisible_btn_tb.Checked == false)
                    {
                        btn_inv = 2; //Bei Aufbrall links und rechts
                    }

                    if (invisible_btn_tb.Checked && invisible_btn_lr.Checked)
                    {
                        btn_inv = 3; //Bei Aufprall an allen Seiten
                    }

                    if (!double.TryParse(invisible_btn_duration.Text, out duration))
                    {
                        MessageBox.Show("Bitte korrigiere deine Eingaben!");
                        return;
                    }
                    else
                    {
                        //Berechnet die Dauer, die der Button unsichbar bleibt und ersetzt den möglichen Punkt mit einem Komma um das Multiplizieren möglich zu machen
                        duration = Convert.ToDouble(invisible_btn_duration.Text.Replace('.', ',')) * 1000;
                    }

                }
                else
                {

                    btn_inv = 0;
                }

                //Vollbildfunktion aktiviert?
                if (ld_fs.Checked)
                {
                    fs = true;
                    //Verdoppelt die Vertikale und Horizontale Geschwindigkeit um den Formatunterschied auszugleichen
                    s_top = s_top * 2;
                    s_left = s_left * 2;
                } else
                {
                    fs = false;
                }

                //Transparenter Spielbereich?

                if (backTrans.Checked == true)
                {
                    transp = true;
                } else
                {
                    transp = false;
                }

                ld_play = true;
                MessageBox.Show("Eingaben gespeichert!", "Gespeichert");

                //Leeren aller Textboxen und deaktivieren aller Checkboxen
                foreach (Control x in this.Controls)
                {
                    if (x is TextBox)
                    {
                        ((TextBox)x).Text = String.Empty;
                    }
                }
                foreach (Control x in this.Controls)
                {
                    if (x is CheckBox)
                    {
                        ((CheckBox)x).Checked = false;
                    }
                }

                this.Close();
            }
            catch
            {
                MessageBox.Show("Irgendetwas ist schief gelaufen... Bitte überprüfe deine Eingaben!");
            }
        }


        /*---------------------------------------------------------------------------------------------------------*/


        //Logic für die GUI des Leveldesigners, um die Kontrollboxen zu steuern

        private void invisible_btn_CheckedChanged(object sender, EventArgs e)
        {
            if (invisible_btn.Checked)
            {
                invisible_btn_lr.Enabled = true;
                invisible_btn_tb.Enabled = true;
                //invisible_btn_rnd.Enabled = true;
                invisible_btn_duration.Enabled = true;
            }
            else
            {
                invisible_btn_lr.Enabled = false;
                invisible_btn_lr.Checked = false;

                invisible_btn_tb.Enabled = false;
                invisible_btn_tb.Checked = false;

                //invisible_btn_rnd.Enabled = false;
                //invisible_btn_rnd.Checked = false;

                invisible_btn_duration.Enabled = false;
                invisible_btn_duration.Text = "";
            }
        }

        //Verknüpfung der Tooltips mit den Eingabefeldern
        private void Leveldesigner_Load(object sender, EventArgs e)
        {
            this.SetStyle(System.Windows.Forms.ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = System.Drawing.Color.Transparent;

            ToolTip toolTip1 = new ToolTip();

            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 100;
            toolTip1.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            toolTip1.SetToolTip(this.h, "Höhe des Buttons in Pixel");
            toolTip1.SetToolTip(this.b, "Breite des Buttons in Pixel");
            toolTip1.SetToolTip(this.su, "1 langsam, < 10 schnell");
            toolTip1.SetToolTip(this.sl, "1 langsam, < 10 schnell");
            toolTip1.SetToolTip(this.sp, "Zufällige Startposition des Buttons innerhalb des Spielfeldes");
            toolTip1.SetToolTip(this.aw, "Zufälliger Abbrallwinkel von Seiten mit einem Wert von 0-3");
            toolTip1.SetToolTip(this.d, "Dauer, die der Button unsichtbar ist wenn er eine Kante berührt hat\nKommazahlen Möglich");
            toolTip1.SetToolTip(this.lbl_trans, "Macht den Spielbereich tranparent bzw durchsichtig");
        }

        private void live_Click(object sender, EventArgs e)
        {
            lv = new Live_Vorschau___ButtonMadness();
            lv.Show();

        }

        private void add_level_Click(object sender, EventArgs e)
        {
            
        }

        private void rnd_bounce_CheckedChanged(object sender, EventArgs e)
        {
            if (rnd_bounce.Checked)
            {
                var confirmMessage = MessageBox.Show("Diese Funktion könnte zu Fehlern im Programm führen. Dennoch aktivieren?",
                                                       "Hinweis",
                                                       MessageBoxButtons.YesNo,
                                                       MessageBoxIcon.Warning,
                                                       MessageBoxDefaultButton.Button2);

                if (confirmMessage == DialogResult.No)
                {
                    rnd_bounce.Checked = false;
                }
            }
        }


        private void new_lvl_Click(object sender, EventArgs e)
        {
            foreach (Control x in this.Controls)
            {
                if (x is TextBox)
                {
                    ((TextBox)x).Text = String.Empty;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txt_h(object sender, EventArgs e)
        {
            if (btn_heigth.TextLength != 0)
            {
                btn_live_preview.Height = Convert.ToInt16(btn_heigth.Text);
            }
        }

        private void txt_b(object sender, EventArgs e)
        {
            if (btn_width.TextLength != 0)
            {
                btn_live_preview.Width = Convert.ToInt16(btn_width.Text);
            }
        }

        private void txt_st(object sender, EventArgs e)
        {
           
        }

        private void txt_sl(object sender, EventArgs e)
        {

        }
    }
}
 