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
using TicTacToe;
using ButtonMadness;

namespace ButtonMadness
{
    public partial class MainMenu : Form
    {
        //Initialisierung der Forms Erweiterungen
        private Leveldesigner ld;
        private ld_success cmb;

        //Geschwindigkeit der Bewegung und Richtungsweiser zugleich
        public double speed_left;
        public double speed_top;

        //Für Wiederherstellung der Geschwindigkeit nach Verwendung der Zeitlupenfunktion
        public double speed_left_backup;
        public double speed_top_backup;

        //Startposition des Buttons
        public double x = 400;
        public double y = 150;

        //Levelindekator für Standardlevel
        public int level = 1;

        //Erlaubt Alphabetische Zeichen für die Benutzerdefinierten Level
        public string custom_level;


        //Button Coordinaten

        public int btn_x;
        public int btn_y;

        //Button Größenfestlegung

        public int btn_height = 80;
        public int btn_width = 80;

        //Variablen für den Zufall
        public int Zufall = 0;

        //Zeitlupe
        public bool zeitlupe = false;
        public bool saver = true;
        public double y_slow;
        public double x_slow;
        public double slowdown_Counter = 0;

        //Unsichtabar
        public int invisible_button = 0;
        public int invisible_time = 500;

        //Vollbild
        public bool fullscreen = false;

        //Spieletipps
        public bool tipps_e = true;
        public int tipps_c = 0;

        public string updateData;
        public string currentUser;


        public MainMenu()
        {
            InitializeComponent();
            ld = new Leveldesigner();
            button1.Location = new Point(-1000, 0);
            levelSettings(0);
            playground.BackColor = Color.White;
            this.BackColor = Color.White;

        }

        //Generiert bei Start von jedem Level eine Zufällige neue Location
        public void randomButtonPosition()
        {
            Random Button = new Random();
            btn_x = Button.Next(1, 723);
            btn_y = Button.Next(1, 480);
        } 

        public void make_Fullscreen ()
        {
            if (fullscreen)
            {
                this.Bounds = Screen.PrimaryScreen.Bounds; //Vollbildfunktion
            }
        }

        //Nimmt einen SQL String als eingabe und Updatet die Datenbank
        public void updateDatabase(string dataString)
        {
            string connect = "Server=127.0.0.1; Port=5432; User Id=postgres; Password=Telekom; Database=postgres";

            NpgsqlConnection conn = new NpgsqlConnection(connect);
            conn.Open();
            NpgsqlCommand com = new NpgsqlCommand(dataString, conn);
            NpgsqlDataReader data = com.ExecuteReader();

            HUB_Trade.TradeCenter tc = (HUB_Trade.TradeCenter)this.Owner;
            tc.updateForm(); //Aktualisiert die Haupform mit den Werten aus der Datenbank
        }


        //Ist für Standard Einstellungen beim Starten des Levels zuständig, 1 --> true, 0 --> false
        // Wird weiter unten im Switch Case verwendet
        public void levelSettings(int operation)
        {
            //Für das starten des Levels
            if (operation == 1)
            {
                //Anzeigetext entfernen
                Start_Text.Visible = false;
                zl_ex.Visible = false;
                pause.Visible = false;
                ld_text.Visible = false;

                //Aktiviert den Button um vorzeitiges Klicken zu vermeiden
                button1.Enabled = true;

                //Aktiviert die Timerschleife um den Ball in Bewegung zu setzen
                timer1.Enabled = true;
                tipps.Enabled = false;
                lbl_tipps.Visible = false;
                lbl_settings.Visible = false;
            } else {
                //Beenden
                this.TopMost = false;
                button1.Location = new Point(-1000, 0);
                playground.BackColor = Color.White;
                this.Size = new Size(543, 391);
                this.Location = new Point(0,0);
                this.CenterToScreen();
                //Anzeigetext einblenden
                Start_Text.Visible = true;
                zl_ex.Visible = true;
                pause.Visible = true;
                ld_text.Visible = true;

                //Deaktiviert den Button
                //button1.Enabled = false;

                button1.Focus();

                //Deaktiviert den Timer
                timer1.Enabled = false;
                lbl_tipps.Visible = true;
                tipps.Enabled = true;
                lbl_settings.Visible = true;

            }
        }


        //Generiert zufällige Werte zwischen 0 und 255 die Zufällige Farbdarstellung, die weiter unten verwendet wird.
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();

        /* Generiert eine Zufallszahl zwischen 0 und 2 für das unsichtbar machen des Button wenn eine Seite berührt wurde -->
         * 0 = Ausgeschalten --- 1 = Bei Berührung der oberen ODER unteren Seite --- 2 = Bei Berührung der linken ODER rechten Seite des Spielfeldes*/
        Random wall_visibility = new Random();

        public static int RandomNumberRgb()
        {
            lock (syncLock)
            { // synchronize
                return random.Next(0, 255);
            }
        }

        public static int rnd_tf()
        {
            lock (syncLock)
            {
                return random.Next(0, 1);
            }
        }


        async public void btn_invisble()
        {
            button1.Visible = false;
            await Task.Delay(invisible_time);
            button1.Visible = true;
        }

        /* Der Haupttimer, der ab drücken der F1 Taste läuft um den Button zu bewegen und andere wichtige Funktionaläten auszuführen */

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Zufallszahl, 0 wenn Zufall im aktuellen Level deaktiviert ist, ansonst ein Wert zwischen 0 und 3
            int rnd;

            if (Zufall == 1)
            {
                rnd = random.Next(0, 3);
            }
            else
            {
                rnd = 0;
            }

            //Setzt den Button in Bewegung
            x += speed_left;
            y += speed_top;

            button1.Location = new Point(Convert.ToInt16(x), Convert.ToInt16(y));


            //Dient zur überprüfung ob der Button eine Seite berührt, wenn ja --> abprallen und Farbe des Panels ändern

            //Unterseite
            if (button1.Bottom >= playground.Bottom || button1.Top <= playground.Top)
            {
                speed_top = -speed_top - rnd;

                if (!ld.transp)
                {
                    //Ändern der Farbe
                    playground.BackColor = Color.FromArgb(RandomNumberRgb(), RandomNumberRgb(), RandomNumberRgb());
                }

                //Switch Case für das unsichtbar machen des Buttons, falls in Levelsettings aktiviert
                switch (invisible_button)
                {
                    // Aktiviert für oben und unten
                    case 1:
                        btn_invisble();
                        break;

                    //Oben und unten, links und rechts
                    case 3:
                        btn_invisble();
                        break;
                }
            }

            if (button1.Left <= playground.Left || button1.Right >= playground.Right)
            {
                speed_left = -speed_left - rnd;

                if (!ld.transp)
                {
                    playground.BackColor = Color.FromArgb(RandomNumberRgb(), RandomNumberRgb(), RandomNumberRgb());
                }
                // Wenn kleiner als 1 --> Aus; 2 --> links und rechts; 3 --> links und rechts, oben und unten
                if (invisible_button > 1)
                {
                    btn_invisble();
                }
                }
            }


        /*------------------------------------------------------------------------------------------------------------------------------*/


        //Zuständig für die Verifizierung  ob der Button geklickt wurde, wenn ja -> Level abgeschlossen -> Spiel beenden -> Datenbankupdate
        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            // Timer deaktivieren
            timer1.Enabled = false;
            cmb = new ld_success(); //Benutzerdefinierte MessageBox anzeigen


            if (level == 101) //Wenn Level 101 (Indikator dafür, das ein Benutzerdefiniertes Level gespielt wurde) --> "Weiter" Button deaktiviert lassen
            {
                cmb.ShowDialog();
                Console.WriteLine("Benutzerdefiniertes Level wird neu gestartet!");
                Stage_Text.Text = ld.levelname;
               
                switch (cmb.mb_state)
                {
                    case 1:
                        SendKeys.Send("{F1}");
                        break;
                    case 2:
                        levelSettings(0);
                        break;
                }
            } else
            {
                cmb.rt.Enabled = true; //Der Button für "Weiter" ist Standardgemäß deaktiviert, da es kein "weiter" bei Benutzerdefinierten Level gibt, siehe oben
                cmb.ShowDialog();
                
                switch (cmb.mb_state)
                {
                    //Weiter zum nächsten Level
                    case 0:
                        level++;
                        Stage_Text.Text = "Level " + level;
                        levelSettings(1);
                        SendKeys.Send("{F1}");
                        tipps.Enabled = false;
                        updateDatabase("UPDATE public.user SET" + '"' + "Materie" + '"' + "=" + '"' + "Materie" + '"' + "+ 10 WHERE username = " + "'" + currentUser + "'" + ";");
                        break;
                    //Level neustarten
                    case 1:
                        levelSettings(1);
                        SendKeys.Send("{F1}");
                        break;
                    //Zurück zum Hauptmenü
                    case 2:
                        level++;
                        Stage_Text.Text = "Level " + level;
                        levelSettings(0);
                        tipps.Enabled = true;
                        break;
                }
            }

            //Zeitlupencounter zurücksetzen
            slowdown_Counter = 0;
        }


        /* --------------------------------------------------------------------------------------------------------------------------------- */


        //Zuständig zur Überprüfung ob eine der unten definierten Tasten gedrückt wurde

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            //Verlassen des Programmes durch drücken von Escape
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }

            //Startet das Spiel mit F1 und bestimmt via Switch Case das Level
            if (e.KeyCode == Keys.F1)
            {
                    if (ld.ld_play)
                    {
                        level = 101;
                    }

                    switch (level)
                    {
                        //Level 1
                        case 1:


                            //Button erstellen, Werte festlegen und zufällige Startposition des Buttons erstellen
                            randomButtonPosition();
                            button1.Height = 80;
                            button1.Width = 80;
                            button1.Location = new Point(btn_x, btn_y);

                            //Geschwindigkeitsfestlegung für Level
                            speed_top = 2;
                            speed_left = 2;

                            speed_top_backup = speed_top;
                            speed_left_backup = speed_left;

                            //Festlegen ob ein zusätzlich Zufälliger Abprallwinkel generiert werden soll
                            Zufall = 0;

                            levelSettings(1);

                        //Update der Datenbank
                        //updateDatabase("UPDATE public.user " + '"' + "Materie" + '"' + "=" + '"' + "Materie" + '"' + "+ 1 WHERE username = " + "'" + currentUser + "'" + ";");
                            break;

                        //Level 2
                        case 2:
                            randomButtonPosition();
                            button1.Height = 50;
                            button1.Width = 50;
                            button1.Location = new Point(btn_x, btn_y);
                            speed_top = 3;
                            speed_left = 3;
                            speed_top_backup = speed_top;
                            speed_left_backup = speed_left;
                            Zufall = 0;
                            levelSettings(1);
                            break;

                        case 3:
                            randomButtonPosition();
                            button1.Height = 40;
                            button1.Width = 40;
                            button1.Location = new Point(btn_x, btn_y);
                            speed_top = 3;
                            speed_left = 3;
                            speed_top_backup = speed_top;
                            speed_left_backup = speed_left;
                            Zufall = 0;
                            levelSettings(1);
                            break;

                        case 4:
                            randomButtonPosition();
                            button1.Height = 30;
                            button1.Width = 30;
                            button1.Location = new Point(btn_x, btn_y);
                            speed_top = 4;
                            speed_left = 4;
                            speed_top_backup = speed_top;
                            speed_left_backup = speed_left;
                            Zufall = 0;
                            levelSettings(1);
                            break;

                        case 5:
                            randomButtonPosition();
                            button1.Height = 30;
                            button1.Width = 30;
                            button1.Location = new Point(btn_x, btn_y);
                            speed_top = 5;
                            speed_left = 5;
                            speed_top_backup = speed_top;
                            speed_left_backup = speed_left;
                            Zufall = 0;
                            levelSettings(1);
                            break;

                        case 6:
                            randomButtonPosition();
                            button1.Height = 30;
                            button1.Width = 30;
                            button1.Location = new Point(btn_x, btn_y);
                            speed_top = 6;
                            speed_left = 6;
                            speed_top_backup = speed_top;
                            speed_left_backup = speed_left;
                            Zufall = 0;
                            levelSettings(1);
                            break;
                        case 7:
                            randomButtonPosition();
                            button1.Height = 25;
                            button1.Width = 25;
                            button1.Location = new Point(btn_x, btn_y);
                            speed_top = 5;
                            speed_left = 5;
                            speed_top_backup = speed_top;
                            speed_left_backup = speed_left;
                            Zufall = 0;
                            levelSettings(1);
                            break;

                        case 8:
                            randomButtonPosition();
                            button1.Height = 25;
                            button1.Width = 25;
                            button1.Location = new Point(btn_x, btn_y);
                            speed_top = 7;
                            speed_left = 7;
                            speed_top_backup = speed_top;
                            speed_left_backup = speed_left;
                            Zufall = 0;
                            levelSettings(1);
                            break;

                        case 9:
                            randomButtonPosition();
                            button1.Height = 23;
                            button1.Width = 23;
                            button1.Location = new Point(btn_x, btn_y);
                            speed_top = 8;
                            speed_left = 8;
                            speed_top_backup = speed_top;
                            speed_left_backup = speed_left;
                            Zufall = 0;
                            levelSettings(1);
                            break;

                        case 10:
                            randomButtonPosition();
                            button1.Height = 20;
                            button1.Width = 20;
                            button1.Location = new Point(btn_x, btn_y);
                            speed_top = 10;
                            speed_left = 10;
                            speed_top_backup = speed_top;
                            speed_left_backup = speed_left;
                            Zufall = 0;
                            levelSettings(1);
                            break;

                        case 11:
                            randomButtonPosition();
                            button1.Height = 50;
                            button1.Width = 50;
                            button1.Location = new Point(btn_x, btn_y);
                            speed_top = 4;
                            speed_left = 4;
                            speed_top_backup = speed_top;
                            speed_left_backup = speed_left;
                            Zufall = 0;
                            levelSettings(1);
                            invisible_button = 1;
                            break;

                        case 12:
                            randomButtonPosition();
                            button1.Height = 30;
                            button1.Width = 30;
                            button1.Location = new Point(btn_x, btn_y);
                            speed_top = 6;
                            speed_left = 6;
                            speed_top_backup = speed_top;
                            speed_left_backup = speed_left;
                            Zufall = 0;
                            levelSettings(1);
                            invisible_button = 2;
                            break;

                    }

                    //Aktiviert den "Endless Mode"  / Max Level = 25
                    if (level > 12 && level < 38)
                    {
                        randomButtonPosition();
                        button1.Height -= 1;
                        button1.Width -= 1;
                        button1.Location = new Point(btn_x, btn_y);
                        speed_top += 1;
                        speed_left += 1;
                        speed_top_backup = speed_top;
                        speed_left_backup = speed_left;
                        Zufall = rnd_tf();
                        levelSettings(1);
                        invisible_button = wall_visibility.Next(0, 3);

                    }

                    //Starten des Benutzerdefinierten Levels was zuvor über den Leveldesigner angelegt wurde
                    if (level == 101)
                    {
                        Console.WriteLine("Level aus Leveldesigner wurde gestartet!");


                        if (ld.rbp)
                        {
                            randomButtonPosition();
                        }

                        button1.Height = ld.heigth;
                        button1.Width = ld.width;
                        button1.Location = new Point(btn_x, btn_y);
                        speed_top = ld.s_top;
                        speed_left = ld.s_left;
                        speed_top_backup = speed_top;
                        speed_left_backup = speed_left;
                        Zufall = ld.bounce;
                        levelSettings(1);
                        invisible_button = ld.btn_inv;
                        invisible_time = Convert.ToInt16(ld.duration);
                        Stage_Text.Text = ld.levelname;
                        fullscreen = ld.fs;
                        make_Fullscreen();
                        
                        if (ld.transp)
                    {
                        playground.BackColor = Color.Transparent;
                        this.TransparencyKey = Color.LimeGreen;
                        this.BackColor = Color.LimeGreen;
                        this.TopMost = true; //Form nach vorne bringen
                    }

                    }
                }

            //Beendet die Runde
            if (e.KeyCode == Keys.F2)
            {
                button1.Location = new Point(-1000, 0);
                levelSettings(0);
                button1.Focus();
            }

            if (e.KeyCode == Keys.F6)
            {
                if (timer1.Enabled == false)
                {
                    Console.WriteLine("Starten des Leveldesigners!");
                    ld.ShowDialog();
                }
            }


            /* -------------------------------------------------------------------------------------------------------------------------------------------- */

            //Kurzzeitige Zeitlupenfunktion, kann ab Stage 3 einmal pro Runde eingesetzt werden
            if (e.KeyCode == Keys.F5)
            {
                if (timer1.Enabled)
                {

                    if (speed_left >= 5 || speed_top >= 5)
                    {

                        //Saver schützt vor spammen der F5 Taste
                        if (saver)
                        {
                            //Nur wenn die Zeitlupenfunktion weniger als 4 mal in der Runde verwendet wurde --> fortfahren
                            if (slowdown_Counter < 4)
                            {
                                //zl_text.Visible = true;
                                x_slow = speed_left / 2;
                                y_slow = speed_top / 2;
                                speed_left -= x_slow;
                                speed_top -= y_slow;
                                Console.WriteLine(speed_left);
                                Console.WriteLine(speed_top);
                                saver = false;
                                delay_func();
                            }
                        }
                    }
                }
            }

            if (e.KeyCode == Keys.F3)
            {
                Console.WriteLine("Benutzerdefiniertes Level gestartet!");
                using (var ld = new Leveldesigner())
                {
                    Console.WriteLine(ld.customLevelActivate);
                    //playground.BackColor = Color.FromArgb(100, 88, 44, 55);
                }
            }
        }

        //Delay für die Zeitlupenfunktion
        public void delay_func()
        {
            Task.Delay(3000).ContinueWith(t => slowDownReset());
        }

        //Zurücksetzen der Zeitlupe, nach ablaufen des Zeitupentimers
        public void slowDownReset() {
            //zl_text.Visible = false;
            speed_left = speed_left_backup;
            speed_top -=  speed_top_backup;
            Console.WriteLine(speed_left);
            Console.WriteLine(speed_top);

            //Erhöhen des Counters zum sicherstellen, dass die Hilfefunktion nicht mehr als dreimal in der Runde Verwendet wird
            slowdown_Counter++;
            saver = true;

        }

        //Anzeigen von Spieletipps im Hauptmenü, wechsel aller 5 Sekunden
        private void tipps_Tick(object sender, EventArgs e)
        {
            if (tipps_e)
            {
                    switch (tipps_c)
                    {
                        case 0:
                            lbl_tipps.Text = "Tipp: Die Zeitlupe kann 3 mal pro Runde verwendet werden.";
                            tipps_c++;
                            break;
                        case 1:
                            lbl_tipps.Text = "Tipp: Benutze den Leveldesigner um Level ganz einfach zu erstellen.";
                            tipps_c++;
                            break;
                        case 2:
                            lbl_tipps.Text = "Tipp: Benutze die Zeitlupe, wenn du nicht weiter kommst.";
                            tipps_c++;
                            break;
                        case 3:
                            lbl_tipps.Text = "Tipp: Die Zeitlupe ist als Unterstützung ab Level 5 verfügbar.";
                            tipps_c++;
                            break;
                        case 4:
                            lbl_tipps.Text = "Tipp: Hast du Level 12 erreicht, startet der \"Endless\" Modus.";
                            tipps_c++;
                            break;
                        case 5:
                            lbl_tipps.Text = "Tipp: Aktiviere Vollbild und Transparenz im Leveldesigner,\num das Spiel als Bildschirmschoner auszuführen!";
                            tipps_c = 0;
                            break;

                }
                    }
                }
            }

        }
