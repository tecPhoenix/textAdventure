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
using TicTacToe;
using ButtonMadness;

namespace HUB_Trade
{
    public partial class TradeCenter : Form
    {
        List<char> splittedText = new List<char>();
        public int currentLetter = 0;
        public bool shopActive = false; //Bool dient dazu, den Kauf von Items nur zu ermöglichen, wenn der Shop aktiv ist
        public int state = 1; //1 Hauptmenü, 2 Minispiele, 3 Shop, 0 Eingabe Erwartet, 4 Auswahl
        public string currentUser; //Derzeit angemeldeter user
        public int skipTut = 0; //0 --> Tutorial noch nicht absolviert, 1 --> absolviert

        public string itemAmount;

        //Instancen
        public SellBox cmb = new SellBox();
        public TicTacToe.main tct = new TicTacToe.main();
        public ButtonMadness.MainMenu bm = new ButtonMadness.MainMenu();
        

        //Standardtexte zum Wiederverwenden, werden später vom TexFader ausgegeben

        //Aufsplitten der Texte aufgrund der Übersichtlichkeit

        public string willkommensnachricht = "------------------------------Willkommen bei HUB Trade------------------------------ " +
        "\n\nSpiele Minispiele um Items zu Sammeln und sie gegen die Virtuelle Währung MA (Materie) im Shop einzutauschen.\nSie " + 
        "verschaffen dir einen guten Vorteil in der Hauptstory. \n\nViel Spaß!\n\n\n Klicke an einer belieben Stelle um Fortzufahren...";

        public string auswahl = "Was möchtest du als nächstes tun?\n\n\n\n\n\n      [1]Minispiel starten     [2]Shop aufrufen     [3]Programm verlassen";

        public string willkommen_Shop = "--------------------------------Willkommen im Shop--------------------------------\n\nVerkaufe " + 
        "Gegenstände aus deinem Inventar, indem du sie mit Linksklick auswählst.\n\n\n                        [1] Zurück";

        public string errorMsgShopInactive = "Öffen bitte zunächst den Shop, um deine Items zu verkaufen................................";
        public string gameChoice = "----------------------------------------Spieleauswahl---------------------------------------\n\n\nWähle" + 
        " ein Spiel deiner Wahl aus und verdiene dir Silber und Gold dazu\n\n[1] TicTacToe      [2] ButtonMadness      [3] Zurück";

        public string settings = "------------------------------------------Einstellungen-------------------------------------------";

        public int tutComp = 0;

        public TradeCenter()
        {
            InitializeComponent();
        }

        //Allgemeine Funktion zum aktualisieren der Form
        public void updateForm()
        {
            string register = "SELECT * FROM public.user WHERE username = '" + currentUser + "';";

            string connstring = "Server=127.0.0.1; Port=5432; User Id=postgres; Password=Telekom; Database=postgres";
            NpgsqlConnection connection = new NpgsqlConnection(connstring);
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand(register, connection);
            NpgsqlDataReader dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                ma_Count.Text = dataReader["Materie"].ToString();
                silver_amount.Text = dataReader["Silber"].ToString() + "x";
                gold_amount.Text = dataReader["Gold"].ToString() + "x";
                fadeInTimer.Interval = Convert.ToInt32(dataReader["fadeInSpeed"]);
            }
        }


        //Beim Start ausgeführt
        private void TradeCenter_Load(object sender, EventArgs e)
        {
            //Update der Form
            string register = "SELECT * FROM public.user WHERE username = '" + currentUser + "';";

            string connstring = "Server=127.0.0.1; Port=5432; User Id=postgres; Password=Telekom; Database=postgres";
            NpgsqlConnection connection = new NpgsqlConnection(connstring);
            connection.Open();
            NpgsqlCommand command = new NpgsqlCommand(register, connection);
            NpgsqlDataReader dataReader = command.ExecuteReader();

            while (dataReader.Read())
            {
                ma_Count.Text = dataReader["Materie"].ToString();
                silver_amount.Text = dataReader["Silber"].ToString() + "x";
                gold_amount.Text = dataReader["Gold"].ToString() + "x";
                fadeInTimer.Interval = Convert.ToInt32(dataReader["fadeInSpeed"]);

                //Anzeigen des Tutorialtextes beim ersten Login
                if (Convert.ToInt32(dataReader["tutComp"]) == 0)
                {
                    textFader(willkommensnachricht);
                    connection.Close();

                    //Setzen der tutComp Variable auf eins um erneutes anzeigen beim nächsten Login zu verhindern
                    string update = "UPDATE public.user SET " +'"' + "tutComp" + '"' + "= 1 WHERE username = '" + currentUser +"';";
                    string connect = "Server=127.0.0.1; Port=5432; User Id=postgres; Password=Telekom; Database=postgres";

                    NpgsqlConnection conn = new NpgsqlConnection(connect);
                    conn.Open();
                    NpgsqlCommand com = new NpgsqlCommand(update, conn);
                    NpgsqlDataReader data = com.ExecuteReader();
                    conn.Close();
                } else
                {
                    //Falls schon mehr als einmal eingeloggt, sofort zur Modulauswahl springen
                    Navigator(state);

                }
            }
            dataReader.Close();
        }



        //Bereitet eingegebenen Text zur Ausgabe in der Console vor
        public void textFader(string text)
        {

            foreach (var x in text)
            {
                splittedText.Add(x); //Hinzufügen der einzelnen Buchstaben zur Liste um sie danch einzeln auszugeben
            }
            console_out.Text = String.Empty; //Alten Consolentext löschen
            fadeInTimer.Start(); //Den timer für die Ausgabe starten
        }


        //Experimentelle Funktion zum Schreibmaschinenarten "einfliegen" von Text
        private void textFadeIn(object sender, EventArgs e)
        {
            //Wenn der letzte Buchstabe des Textes erreicht wurde --> ausgabe Stoppen, Liste zurücksetzen
            if (currentLetter == splittedText.Count)
            {
                fadeInTimer.Stop();
                splittedText.RemoveRange(0, splittedText.Count);
                currentLetter = 0;
                Console.WriteLine("Timer stopped!");
            }
            else
            {
                //Ausgabe des Textes in der Console
                console_out.AppendText(splittedText[currentLetter].ToString());
                currentLetter++;

            }
        }


        private void consoleNext(object sender, EventArgs e)
        {
            if (fadeInTimer.Enabled == false)
            Navigator(state);
        }


        public void Navigator(int nav)
        {

            switch (nav)
            {
                case 1: //User kommt vom Hahuptmenü
                    textFader(auswahl); //Zeigt neuen Text an
                    console_in.ReadOnly = false; //Ermöglichst Eingabe durch den User
                    console_in.Focus(); //Focus auf Eingabefenster
                    console_in.SelectionStart = 2; //Änder der Cursorposition
                    break;

                case 2:
                    textFader(gameChoice);
                    console_in.ReadOnly = false;
                    console_in.Focus();
                    console_in.SelectionStart = 2;
                    break;

                case 3:
                    textFader(willkommen_Shop);
                    console_in.ReadOnly = false; //Ermöglichst Eingabe durch den User
                    //state = 0; //Setzt den State auf Eingabe erwatet
                    console_in.Focus(); //Focus auf Eingabefenster
                    console_in.SelectionStart = 2; //Änder der Cursorposition
                    break;
            }
        }


        //Modulauswahl
        private void send(object sender, KeyEventArgs e)
        {
            if (console_in.ReadOnly == false)
            {
                if (e.KeyCode == Keys.Enter) //Falls Enter in dem Eingabefenster gedrückt wurde
                {
                    switch (state) //Überpüft, woher der User kommt
                    {
                        case 1: //Er kommt vom Hauptmenü

                            switch (console_in.Text)
                            {
                                //Minispiel starten
                                case ">>1":
                                case ">>Minispiel starten":
                                    state = 2;
                                    Navigator(state);
                                    Console.WriteLine("Minspielauswahl");

                                    break;

                                //Shop aufrufen
                                case ">>2":
                                case ">>Shop aufrufen":
                                    Console.WriteLine("Shop");
                                    state = 3; //Wechsel zum Shop
                                    Navigator(state);
                                    shopActive = true;
                                    break;

                                //Programm verlassen
                                case ">>3":
                                case ">>Programm verlassen":
                                    Application.Exit();
                                    break;
                            }
                            break;
                        
                        //Auswahl in der Minispiele Sektion
                        case 2:
                            if (console_in.Text == ">>1" || console_in.Text == ">>TicTacToe") {
                                tct.currentUser = currentUser;
                                tct.Owner = this;
                                tct.ShowDialog();
                            }
                            else if (console_in.Text == ">>2" || console_in.Text ==">>ButtonMadness")
                            {
                                bm.currentUser = currentUser;
                                bm.ShowDialog();
                            }
                            else if (console_in.Text == ">>3" || console_in.Text == "Zurück")
                            {
                                state = 1;
                                textFader(auswahl);
                            }
                            break;

                        case 3: //Er kommt vom Shop
                            if (console_in.Text == ">>1" || console_in.Text == ">>Zurück")
                            {
                                state = 1;
                                textFader(auswahl);
                            }
                            break;
                    }

                    //Setzt Konsolenfenster nach Eingabe zurück
                    console_in.Text = ">>";
                    console_in.Focus();
                    console_in.SelectionStart = 2;
                }
            }

            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }


        //Öffnet die Form zum Traden und überträgt aktuelle Werkstände
        private void openTradeMenu(object sender, EventArgs e)
        {
            if (shopActive) //Shop kann erst geöffnet werden, wenn Modul vorher ausgewählt wurde
            {
                PictureBox pb = (PictureBox)sender; //Ermittelt Bild des auslösers und überträgt dieses

                //Übertragung der Daten in neue Form
                if (pb.Name == "pb_gold")
                {
                    cmb.currentUser = currentUser;
                    cmb.name.Text = "Gold";
                    cmb.ma_amount = ma_Count.Text;
                    cmb.transferValue(removeXFromValue(gold_amount.Text));
                    cmb.icon.Image = Properties.Resources.Gold_Bar_icon;
                    cmb.Owner = this;
                    cmb.ShowDialog();


                }
                else if (pb.Name == "pb_iron")
                {
                    cmb.currentUser = currentUser;
                    cmb.name.Text = "Silber";
                    cmb.ma_amount = ma_Count.Text;
                    cmb.transferValue(removeXFromValue(silver_amount.Text));
                    cmb.icon.Image = Properties.Resources.Silver_Efex_Pro_icon;
                    cmb.Owner = this;
                    cmb.ShowDialog();
                }
               
            } else
            {
                MessageBox.Show("Bitte öffen zunächst den Shop!"); //Wenn Shop Modul nicht angewählt wurde -> Fehler
            }
        }


        //Funktion zum entfernen des "x" nach manchen Werten, wie z.B. die Wertanzeige von Gold und Silber im Inventarfenster
        public int removeXFromValue(string value)
        {
            return Convert.ToInt16(value.Substring(0, value.Length - 1));
        }

        //Programm verlassen durch drücken von ESC, wenn console_in Focus hat
        private void leave(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) {

                this.Close();
            }
        }


        //Aktualisiert nach einem verifizierten Kauf die Werte im Program, sowie in der Datenbank
        public void updateMa(int money_made, int object_selled, string item)
        {
            ma_Count.Text = Convert.ToString(Convert.ToInt32(ma_Count.Text) - money_made); //Materie aktualisieren

            switch (item)
            {
                case "Silber":
                    silver_amount.Text = Convert.ToString(Convert.ToInt32(removeXFromValue(silver_amount.Text) + object_selled)) + "x"; //An den Aktualisierten Wert, wird wieder das x angehangen
                    break;
                case "Gold":
                    gold_amount.Text = Convert.ToString(Convert.ToInt32(removeXFromValue(gold_amount.Text) + object_selled)) + "x";
                    break;
            }

            //Update der Werte in der Datenbank

            string update = "UPDATE public.user SET " + '"' + "Materie" + '"' + "=" + ma_Count.Text + "," + '"' + "Silber" + '"' + "=" + removeXFromValue(silver_amount.Text) + "," + '"' + "Gold" + '"' + "=" + "'" + removeXFromValue(gold_amount.Text) + "'" +" WHERE username = '" + currentUser + "';";
            string connect = "Server=127.0.0.1; Port=5432; User Id=postgres; Password=Telekom; Database=postgres";

            NpgsqlConnection conn = new NpgsqlConnection(connect);
            conn.Open();
            NpgsqlCommand com = new NpgsqlCommand(update, conn);
            NpgsqlDataReader data = com.ExecuteReader();
        }

        /*Die unteren zwei Funktionen, dienen zum verschieben der Form per Maus, da die Form durch die Designanpassung "Borderless" gemacht wurde
         *  und so nicht mehr verschoben werden konnte
         */

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

        //Öffnet das Einstellungsfenster
        private void sett_Click(object sender, EventArgs e)
        {

        }
    }
}
