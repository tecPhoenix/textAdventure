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
using EASendMail;

namespace HUB_Trade
{
    public partial class Register : Form
    {
        private string userMail;
        private string currentUser;
        private bool userValidated = true;
        string connstring = "Server=127.0.0.1; Port=5432; User Id=postgres; Password=Telekom; Database=postgres";

        public Register()
        {
            InitializeComponent();
        }

        //Überprüft das eingegebene Passwort und gibt wahr bzw. Falsch und eine Fehlermeldung zurück
        public bool passwortChecker(string pass)
        {
            if (pass.Length < 6)
            {
                return false;
            }

            return true;
        }

        private void showTerms(object sender, EventArgs e)
        {
            var msg = MessageBox.Show("Hinweis: Bei der Registrierung werden deine Daten lokal auf unserem Server gespeichert und liegen dort unverschlüsselt vor. Bedingungen annehmen?", "Nutzungsbedingungen", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);

            if (msg == DialogResult.Yes)
            {
                cb_terms.Checked = true;

            } else
            {
                this.Close();
            }
        }

        private void validateUser(object sender, EventArgs e)
        {
            int emptyTb = 0;

            foreach (Control x in Controls)
            {
                if (x is TextBox)
                {
                    if (x.Text != "")
                    {
                        emptyTb++;
                    }
                }
            }

            if (emptyTb == 7)
            {
                if (pswd.Text == pswd_check.Text)
                {
                    if (pswd.Text.Length > 5)
                    {
                        if (cb_terms.Checked == true)
                        {
                            NpgsqlConnection connect = new NpgsqlConnection(connstring);
                            connect.Open();
                            string dat = "SELECT * FROM public.user WHERE username =" + "'" + username.Text + "';";

                            NpgsqlCommand comm = new NpgsqlCommand(dat, connect);
                            NpgsqlDataReader read = comm.ExecuteReader();


                            while (read.Read())
                            {

                                if (read.HasRows)
                                {
                                    MessageBox.Show("Ein Benutzer mit diesem Namen existiert bereits, bitte wähle einen anderen oder setze dein Passwort zurück!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    connect.Close();
                                    userValidated = false;

                                }
                            }


                            if (userValidated)
                            {
                                string register = "INSERT INTO public.user(" + '"' + "username" + '"' + "," + '"' + "password" + '"' + "," + '"' + "vorname" + '"' + "," + '"' + "nachname" + '"' + "," + '"' + "frage" + '"' + "," + '"' + "email" + '"' + ") VALUES ('" + username.Text + "','" + pswd.Text + "','" + surname.Text + "','" + lastname.Text + "','" + question_answer.Text + "','" + mail.Text + "');";
                                userMail = mail.Text;
                                Console.WriteLine(register);
                                NpgsqlConnection connection = new NpgsqlConnection(connstring);
                                connection.Open();

                                Console.WriteLine(register);
                                NpgsqlCommand command = new NpgsqlCommand(register, connection);
                                NpgsqlDataReader dataReader = command.ExecuteReader();

                                MessageBox.Show("Registrierung erfolgreich! Vor dem ersten einloggen, bestätige bitte deine Email-Adresse.");
                                this.Close();
                                //Versenden der Email zum bestätigen des Accounts

                                try
                                {
                                    /*SmtpMail mail = new SmtpMail("TryIt");
                                    SmtpClient client = new SmtpClient();

                                    mail.From = "tradetextadventure@gmail.com";
                                    mail.To = email.Text;
                                    mail.Subject = "Testsubject";
                                    mail.TextBody = "This is a test body";

                                    SmtpServer server = new SmtpServer("smtp.gmail.com");
                                    server.Port = 587;
                                    server.ConnectType = SmtpConnectType.ConnectSSLAuto;
                                    server.User = "tradetextadventure@gmail.com";
                                    server.Password = "HUB_Trade";

                                    client.SendMail(server, mail);*/

                                }



                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                            userValidated = true;
                        }
                        else
                        {
                            MessageBox.Show("Bitte bestätige die Nutzungsbedingungen!");
                        }
                    } else
                    {
                        MessageBox.Show("Dein Passwort ist zu kurz! Es sollte mindestens 6 Zeichen lang sein.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Eingegebene Passwörter stimmen nicht überein!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Bitte erforderliche Felder ausfüllen!");

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
