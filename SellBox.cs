using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using MySql.Data;
using MySql.Data.MySqlClient;
using Npgsql;

namespace HUB_Trade
{
    public partial class SellBox : Form
    {   
        //Bools zum schnellen Ändern des Wertes (Interval)

        public bool increaseDown = false;
        public bool decreaseDown = false;
        //
        public int timerTickDecrease = 5; //Verändert Geschwindigkeit des Timers, siehe unten
        //TradeCenter tc = new TradeCenter();


        //Item Eigenschaften Übernehmen
        public int anzahl; //Seperater string für die Item-Anzahl um Überladungen zu vermeiden
        public int potVerdienst;
        public string currentUser;
        public string ma_amount;

        public void transferValue(int data)
        {
            anzahl = data;
            amount.Text = "0x";
        }

        public SellBox()
        {
            InitializeComponent();
        }

        private void CustomMessageBox_Load(object sender, EventArgs e)
        {
            updateCurrentEarnings();
        }

        public void updateCurrentEarnings()
        {
            switch (name.Text)
            {
                case "Silber":
                    total_earnings.Text = "für " + Convert.ToString(Convert.ToInt32(removeXFromValue(amount.Text)) * 5) + " MA";
                    potVerdienst = Convert.ToInt32(removeXFromValue(amount.Text) * 5);
                    break;

                case "Gold":
                    total_earnings.Text = "für " + Convert.ToString(Convert.ToInt32(removeXFromValue(amount.Text)) * 10) + " MA";
                    potVerdienst = Convert.ToInt32(removeXFromValue(amount.Text) * 10);
                    break;
            }
        }

        //Manuelle Änderung des Wertes durch Textfeld
        private void changeAmount(object sender, EventArgs e)
        {
            string new_value = Interaction.InputBox("Neuen Wert eingenben", "Wert ändern") + "x";

            if (verifyValue(new_value, 2))
            {
                amount.Text = new_value;
                updateCurrentEarnings();
            } else
            {
                MessageBox.Show("Ungültige Eingabe!");
            }
        }

        private void closeTrade(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lessObjects(object sender, MouseEventArgs e)
        {

            if (verifyValue(amount.Text, 0)) //Stellt sicher, dass der Wert nicht kleiner als 1 und nicht größer als die zur Verfügung stehende Menge an Materie ist.
            {
                //Veringert die Anzahl der Objekte um 1
                int items = Convert.ToInt16(amount.Text.Substring(0, amount.Text.Length - 1)) - 1;


                amount.Text = items.ToString() + "x";

                //Zum schnelleren ändern der Anzahl, Maus gedrückt halten
                decreaseDown = true;
                itemChanger.Start(); //Startet den Timer
                updateCurrentEarnings();
            }
            else {
                Console.WriteLine("Ungültiger Wert");
            }
        }


        private void moreObjects(object sender, MouseEventArgs e)
        {

            if (verifyValue(amount.Text, 1))
            {
                int items = Convert.ToInt16(amount.Text.Substring(0, amount.Text.Length - 1)) + 1;
                amount.Text = items.ToString() + "x";

                increaseDown = true;
                itemChanger.Start();
                updateCurrentEarnings();
            }
            else
            {
                Console.WriteLine("Ungültiger Wert");
            }
        }

        //Verändert die Geschwindigkeit, mit der die Items beim gedrückt halten der Maustaste herunter bzw. heraufgezählt werden.
        private void increaseChangingSpeed(object sender, EventArgs e)
        {
                if (itemChanger.Interval - timerTickDecrease >= 20) //Verhinder OutOfRange Exception
                {
                    itemChanger.Interval -= timerTickDecrease; //Veringert Intervall mit der Zeit
                }

                timerTickDecrease += 20;

            //Hinzufügen
            if (increaseDown)
            {
                if (verifyValue(amount.Text, 1)) //Stellt sicher, dass der Wert nicht kleiner als 0 und nicht größer als die zur Verfügung stehende Menge ist.
                {
                    int items = Convert.ToInt16(amount.Text.Substring(0, amount.Text.Length - 1)) + 1;
                    amount.Text = items.ToString() + "x";
                    updateCurrentEarnings();
                }
                else
                {
                    Console.WriteLine("Ungültiger Wert");
                }
            }

            //Abziehen
            if (decreaseDown)
            {
                if (verifyValue(amount.Text, 0)) //Stellt sicher, dass der Wert nicht kleiner als 0 und nicht größer als die zur Verfügung stehende Menge ist.
                {
                    int items = Convert.ToInt16(amount.Text.Substring(0, amount.Text.Length - 1)) - 1;
                    amount.Text = items.ToString() + "x";
                    updateCurrentEarnings();

                } else
                {
                    Console.WriteLine("Ungültiger Wert");
                }
            }
        }

        //Setzt Werte wie Interval, bool zurück und Stoppt Timer
        private void setDecBool(object sender, MouseEventArgs e)
        {
            decreaseDown = false;
            itemChanger.Stop();
            timerTickDecrease = 5;
            itemChanger.Interval = 500;
        }

        //Siehe darüber
        private void setInrBool(object sender, MouseEventArgs e)
        {
            increaseDown = false;
            itemChanger.Stop();
            timerTickDecrease = 5;
            itemChanger.Interval = 500;
        } 

        //Funktion zum Überprüfen ob der angewählte Wert dem Gültigkeitsbereich entspricht
        public bool verifyValue(string value, int type) //Value == aktueller Wert, type 0 = decreasement, 1 = increasement, 2 = Eingabe per Textbox
        {
                if (type == 0)
                {
                    if (Convert.ToInt32(removeXFromValue(value)) == 0)
                    {
                        return false;
                    }
                }
                else if (type == 1)
            {
                switch (name.Text)
                {
                    case "Gold":
                        if (Convert.ToInt32(removeXFromValue(value)) * 10 >= Convert.ToInt32(ma_amount))
                        {
                            return false;
                        }
                        break;
                    case "Silber":
                        if (Convert.ToInt32(removeXFromValue(value)) * 5 >= Convert.ToInt32(ma_amount))
                        {
                            return false;
                        }
                        break;
                }
            }

                else if (type == 2)
                {

                    switch (name.Text)
                {
                    case "Gold":
                        if (Convert.ToInt32(removeXFromValue(amount.Text)) * 10 > Convert.ToInt32(ma_amount))
                        {
                            return false;
                        }
                        break;
                    case "Silber":
                        if (Convert.ToInt32(removeXFromValue(amount.Text)) * 5 > Convert.ToInt32(ma_amount))
                    {
                            return false;
                        }
                        break;
                }
                    
                }
                return true;
        }

        public void sell_Click(object sender, EventArgs e)
        {
            this.Close();
            TradeCenter tc = (TradeCenter)Owner;
            tc.updateMa(potVerdienst, Convert.ToInt32(removeXFromValue(amount.Text)), name.Text); //Aktualisiert den Materie-Wert
            tc.updateForm();
        }

        public int removeXFromValue(string value)
        {
            return Convert.ToInt32(value.Substring(0, value.Length - 1));
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
    }
}
