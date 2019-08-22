using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;
using Npgsql;

namespace TicTacToe
{
    public partial class main : Form
    {
        public int playerFlag = 1; //Legt fest, welcher Spieler derzeit dran ist (1 oder 2)
        public int round = 0; //Aktuelle Runde (0-9) dient zum überprüfen, ob es letztendlich Unentschieden steht
        public int gameFinished = 0; //Spiel abgeschlossen?
        public bool ki_switch = false; //Ki an oder aus?
        public string currentUser;
        public string up;
        public main()
        {
            InitializeComponent();
        }

        //Setzt das Spiel zurück
        public void resetGame(object sender, EventArgs e)
        {
            playerFlag = 1;
            round = 0;
            this.Text = "Spieler 1 -- X";

            //Löscht x und o's aus den Buttons
            foreach (Control x in this.Controls)
            {
                if (x is Button && x.Text != "Neues Spiel" && x.Text != "Ki (easy)") {
                    x.Text = String.Empty;
                    x.BackColor = Color.Transparent;
                }
            }
        }


        //Setzt die Kreuze bzw. Kreise
        private void setChar(object sender, EventArgs e)
        {
            var btn = (Button)sender; //Button ermitteln, der eine Aktion ausgelöst hat

            //Wenn ein unbelegter Button gedrückt wurde, eine Runde weiter
            if (btn.Text != "X" && btn.Text != "O")
            {
                round++;
            }

            //Identifizierung des aktuellen Spielmoduses
            if (ki_switch)
            {
                //Zweig falls Ki aktiviert wurde
                if (btn.Text == "")
                {
                    if (playerFlag == 1)
                    {
                        btn.Text = "X";
                        playerFlag = 2;
                        this.Text = "Spieler 1 --  X";

                        Random r = new Random();

                        //Stellt die derzeit zur Verfügung stehenden Felder für den Bot dar -- Anhand dieser Liste wird letztendlich ein Feld zufällig áusgewählt
                        List<Control> availableSlots = new List<Control>();

                        //Fügt die zur Verfügung stehenden Felder der Liste hinzu
                        foreach (Control x in Controls)
                        {
                            if (x.Text != "Neues Spiel" && x.Text != "Ki (easy)")
                            {
                                if (x.Text == "")
                                {
                                    availableSlots.Add(x);
                                }
                            }
                        }

                        if (availableSlots.Count != 0)
                        {
                            availableSlots[r.Next(0, availableSlots.Count)].Text = "O"; //Wählt zufällig ein Feld aus der Liste aus und setzt ein X
                            round++;
                        }

                        checkWinningState(); //Überprüft, ob die Vorraussetzungen für eine Sieg erfüllt wurden

                        playerFlag = 1;
                    }
                }
            }
            else
            {
                //Zweig für den normalen Modus, ohne Ki

                if (btn.Text == "")
                {
                    if (playerFlag == 1)
                    {
                        btn.Text = "X";
                        playerFlag = 2;
                        this.Text = "Spieler 2 --  O";
                    }
                    else
                    {
                        btn.Text = "O";
                        playerFlag = 1;
                        this.Text = "Spieler 1 --  X";
                    }
                }
                checkWinningState();
            }
        }
        
        //Überprüft, ob alle Vorraussetzungen für eine Sieg erfüllt wurden
        public void checkWinningState()
        {

            //Vertikale Gewinnmuster

                //Erste Zeile
                if (btn_1.Text == btn_2.Text && btn_2.Text == btn_3.Text && btn_3.Text != "")
                {
                    btn_1.BackColor = Color.GreenYellow;
                    btn_2.BackColor = Color.GreenYellow;
                    btn_3.BackColor = Color.GreenYellow;

                    showResults(btn_1.Text);
                    gameFinished = 1;
                }

                //Zweite Zeile
                else if (btn_4.Text == btn_5.Text && btn_5.Text == btn_6.Text && btn_6.Text != "")
                    {
                    btn_4.BackColor = Color.GreenYellow;
                    btn_5.BackColor = Color.GreenYellow;
                    btn_6.BackColor = Color.GreenYellow;
                    gameFinished = 1;
                    showResults(btn_4.Text);
                }

                //Dritte Zeile
                else if (btn_7.Text == btn_8.Text && btn_8.Text == btn_9.Text && btn_9.Text != "")
                {
                    btn_7.BackColor = Color.GreenYellow;
                    btn_8.BackColor = Color.GreenYellow;
                    btn_9.BackColor = Color.GreenYellow;
                    gameFinished = 1;
                    showResults(btn_7.Text);
            }

             //Horizontale Gewinnmuster

                //Erste Spalte
                else if (btn_1.Text == btn_4.Text && btn_4.Text == btn_7.Text && btn_1.Text != "")
                {
                    btn_1.BackColor = Color.GreenYellow;
                    btn_4.BackColor = Color.GreenYellow;
                    btn_7.BackColor = Color.GreenYellow;
                    gameFinished = 1;
                    showResults(btn_1.Text);
                }

                //Zweite Spalte
                else if (btn_2.Text == btn_5.Text && btn_5.Text == btn_8.Text && btn_2.Text != "")
                {
                    btn_2.BackColor = Color.GreenYellow;
                    btn_5.BackColor = Color.GreenYellow;
                    btn_8.BackColor = Color.GreenYellow;
                    gameFinished = 1;
                    showResults(btn_2.Text);
                }

                //Dritte Spalte
                else if (btn_3.Text == btn_6.Text && btn_6.Text == btn_9.Text && btn_3.Text != "")
                {
                    btn_3.BackColor = Color.GreenYellow;
                    btn_6.BackColor = Color.GreenYellow;
                    btn_9.BackColor = Color.GreenYellow;
                    gameFinished = 1;
                    showResults(btn_3.Text);
            }

            //Vertikale Gewinnmuster
                
                else if (btn_1.Text == btn_5.Text && btn_5.Text == btn_9.Text && btn_1.Text != "")
                {
                    btn_1.BackColor = Color.GreenYellow;
                    btn_5.BackColor = Color.GreenYellow;
                    btn_9.BackColor = Color.GreenYellow;
                    gameFinished = 1;
                    showResults(btn_1.Text);
                }

                else if (btn_3.Text == btn_5.Text && btn_5.Text == btn_7.Text && btn_3.Text != "")
                {
                    btn_3.BackColor = Color.GreenYellow;
                    btn_5.BackColor = Color.GreenYellow;
                    btn_7.BackColor = Color.GreenYellow;
                    gameFinished = 1;
                    showResults(btn_3.Text);
                }

            //Unentschieden
                else if (round == 9)
            {
                //Alle Buttons Rot einfärben
                foreach (Control x in this.Controls)
                {
                    if (x is Button && x.Text != "Neues Spiel" && x.Text != "Ki (easy)")
                    {
                        x.BackColor = Color.Red;
                    }
                }
                showResults("");
            }
        }

        //Ergebnisse anzeigen
        public void showResults(string a) {

            if (a == "")
            {
                var MessageBoxX = MessageBox.Show("Unentschieden! Neues Spiel starten?", "Hinweis", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

                up = "UPDATE public.user SET " + '"' + "Materie" + '"' + "= " + '"' + "Materie" + '"' + " + 10 WHERE username = '" + currentUser + "';";

                if (MessageBoxX == DialogResult.Yes)
                {
                    newGame.PerformClick();
                }
                else
                {
                    this.Close();
                }
            } else
            {
                if (a == "O")
                {
                    if (ki_switch)
                    {
                        var MessageBoxX = MessageBox.Show("Ki hat gewonnen! Nicht dein ernst, oder??! Das muss einfach bestraft werden! Neues Spiel starten?", "Hinweis", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                        if (MessageBoxX == DialogResult.Yes)
                        {
                            newGame.PerformClick();
                        }
                        else
                        {
                            this.Close();
                        }
                    } else
                    {
                        var MessageBoxX = MessageBox.Show("Spieler 2 hat gewonnen! Neues Spiel starten?", "Hinweis", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                        if (MessageBoxX == DialogResult.Yes)
                        {
                            newGame.PerformClick();
                        }
                        else
                        {
                            this.Close();
                        }
                    }
                    up = "UPDATE public.user SET " + '"' + "Materie" + '"' + "= " + '"' + "Materie" + '"' + " - 200 WHERE username = '" + currentUser + "';";

                } else
                {
                    if (a == "X")
                    {
                        var MessageBoxX = MessageBox.Show("Spieler 1 hat gewonnen! Neues Spiel starten?", "Hinweis", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

                        up = "UPDATE public.user SET " + '"' + "Materie" + '"' + "= " + '"' + "Materie" + '"' + " + 10 WHERE username = '" + currentUser + "';";
                        if (MessageBoxX == DialogResult.Yes)
                        {
                            newGame.PerformClick();
                        }
                        else
                        {
                            this.Close();
                        }
                    }
                }
            }


            string connect = "Server=127.0.0.1; Port=5432; User Id=postgres; Password=Telekom; Database=postgres";

            NpgsqlConnection conn = new NpgsqlConnection(connect);
            conn.Open();
            NpgsqlCommand com = new NpgsqlCommand(up, conn);
            NpgsqlDataReader data = com.ExecuteReader();

            HUB_Trade.TradeCenter tc = (HUB_Trade.TradeCenter)Owner;
            tc.updateForm();
        }

        //Dient zum ändern des Spielmoduses
        private void toggleKi(object sender, EventArgs e)
        {
            if (ki_switch)
            {
                ki_swi.BackColor = Color.Transparent;
                ki_switch = false;
            } else
            {
                ki_swi.BackColor = Color.GreenYellow;
                ki_switch = true;
            }
        }

        Point lastPoint;
        private void dragD(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void dragM(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
