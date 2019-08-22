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

namespace HUB_Trade
{
    public partial class Login_Menu : Form
    {

        
        Register reg = new Register();
        TradeCenter tc = new TradeCenter();
        public string currentUser;
        public string currentPassword;
        string connstring = "Server=127.0.0.1; Port=5432; User Id=postgres; Password=Telekom; Database=postgres";

        public Login_Menu()
        {
            InitializeComponent();
        }

        //Login
        private void login_Click(object sender, EventArgs e)
        {
            Console.WriteLine(currentPassword);

            //Überprüfen ob eingegebener Nutzername nicht leer ist
            if (username.Text == "")
            {
                MessageBox.Show("Der Benutzername kann nicht leer sein!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //Überpüfen ob eingegebenes Passwort nicht leer ist
            else if (password.Text == "")
            {
                MessageBox.Show("Das Password kann nicht leer sein!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //Verbindungsherstellung Datenbank
                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                connection.Open();


                string data = "SELECT * FROM public.user WHERE username = " + "'" + username.Text + "';"; //Aktuellen User aus Datenbank abrufen

                NpgsqlCommand command = new NpgsqlCommand(data, connection);
                NpgsqlDataReader dataReader = command.ExecuteReader();

                if (dataReader.HasRows) //Überpüfen ob eingegebener User existiert
                {
                    while (dataReader.Read()) //Datenstream starten, um Werte auszulesen
                    {
                        if (Convert.ToInt16(dataReader["wrongPasswordCount"]) <= 3)
                        {
                            if (password.Text == dataReader["password"].ToString()) //Stimmt das eingegebene Passwort für den gewünschten User mit dem eingegebenen Überein?
                            { //ja      
                                currentUser = username.Text; //CurrentUser auf aktuellen User setzten
                                dataReader.Close();
                                string update = $@"UPDATE public.user SET ""wrongPasswordCount"" = 0 WHERE ""username"" = '{currentUser}'";
                                NpgsqlCommand comm = new NpgsqlCommand(update, connection);
                                NpgsqlDataReader reader = comm.ExecuteReader();

                                logSignin(); //Ruft funktion zum loggen der Anmeldungen auf, siehe unten
                                tc.currentUser = username.Text; //Übergibt den aktuellen user an das nächste Formular (in diesem Falle TradeCenter)
                                tc.ShowDialog(); //Neues Formular anzeigen
                            } else
                            {
                                dataReader.Close();
                                MessageBox.Show("Passwort ist falsch, überprüfe deine Eingabe", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                password.Text = ""; //Passwortfeld zur Neueingabe leeren

                                string update = $@"UPDATE public.user SET ""wrongPasswordCount"" = ""wrongPasswordCount"" + 1 WHERE ""username"" = '{username.Text}'";
                                NpgsqlCommand comm = new NpgsqlCommand(update, connection);
                                NpgsqlDataReader read = comm.ExecuteReader();
                            }

                        } else
                        {
                            MessageBox.Show("Dieser Account wurde gesperrt, da du dein Passwort zu oft falsch eingegeben hast. Bitte wende dich an einen Administrator", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    
                }
                else
                {
                    //Eingegebener User existiert nicht --> Fehlermeldung
                    MessageBox.Show("Dieser Nutzername existiert nicht! Überpüfe deine Schreibweise oder Registriere dich.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);                 
                }
                    }
                        }


        //Erhöht den Counter, wie oft sich ein User angemeldet hat, in der Datenbank
        public void logSignin()
        {
            string updateDatabase = $@"UPDATE public.user SET ""logInTimes"" = ""logInTimes"" + 1 WHERE username = '{currentUser}';";
            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            conn.Open();

            NpgsqlCommand command = new NpgsqlCommand(updateDatabase, conn);
            NpgsqlDataReader dataReader = command.ExecuteReader();
            conn.Close();


        }

        //Öffnet Registrierungsformular
        private void register_Click(object sender, EventArgs e)
        {
            reg.ShowDialog();
        }

        //Dient zum verschieben der Forms mit der Maus

        Point lastPoint;
        private void dragForm(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }

        private void dragFormM(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;
            }
        }

        //Schließen des Formulars
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Beim klicken von Enter im Passwort Feld --> Login ausführen
        private void logIn(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                log.PerformClick();
            }
        }
    }
}
