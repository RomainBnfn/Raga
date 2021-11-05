using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WinformTest
{
    public partial class DemandeConnexion : Form
    {
        private Byte couleur;
        private String initPlayerData = "localhost;12345;pseudo;1;0";
        String meilleurScore;
        String playerDataFile;

        /// <summary>
        /// La form de Demande de connexion
        /// </summary>
        public DemandeConnexion()
        {
            InitializeComponent();

            String dirctoryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            playerDataFile = System.IO.Path.Combine(dirctoryPath,"playerData.txt");
            //données stockées: "ipServeur;portServeur;dernierPseudo;dernièreCouleur;scoreMaxPerso"
            if(!System.IO.File.Exists(playerDataFile))
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(playerDataFile);
                file.Write(initPlayerData);
                file.Close();
            }

            String[] playerData = System.IO.File.ReadAllText(playerDataFile).Split(';');
            textBox1.Text = playerData[0];
            textBox2.Text = playerData[1];
            textBox3.Text = playerData[2];
            this.couleur = byte.Parse(playerData[3]);
            comboBox1.SelectedIndex = this.couleur-1;
            switch(this.couleur)
            {
                case 1:
                    button1.Text = "Se connecter : Equipe \"Bleu\"";
                    button1.BackColor = Color.SkyBlue;
                    break;
                case 2:
                    button1.Text = "Se connecter : Equipe \"Rouge\"";
                    button1.BackColor = Color.Tomato;
                    break;
                case 3:
                    button1.Text = "Se connecter : Equipe \"Orange\"";
                    button1.BackColor = Color.Orange;
                    break;
                case 4:
                    button1.Text = "Se connecter : Equipe \"Vert\"";
                    button1.BackColor = Color.LightGreen;
                    break;
                case 5:
                    button1.Text = "Se connecter : Equipe \"Jaune\"";
                    button1.BackColor = Color.Yellow;
                    break;
                case 6:
                    button1.Text = "Se connecter : Equipe \"Rose\"";
                    button1.BackColor = Color.Pink;
                    break;
            }

            meilleurScore = playerData[4];
        }

        /// <summary>
        /// Bouton de demande de connexion:
        /// Envoie une requête de connexion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("Aucun champ ne doit être nul.", "Demande invalide", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string ip = textBox1.Text;
            int port = Int32.Parse(textBox2.Text);
            string pseudo = textBox3.Text;

            Outils.DemanderConnexion(ip, port, pseudo, Program.client, couleur);

            if (Program.connexion)
            {
                StringBuilder stringBuilder = new StringBuilder("");
                stringBuilder.Append(ip.ToString())
                    .Append(";")
                    .Append(port.ToString())
                    .Append(";")
                    .Append(pseudo)
                    .Append(";")
                    .Append(couleur.ToString())
                    .Append(";")
                    .Append(meilleurScore);
                
                System.IO.File.Delete(playerDataFile);
                System.IO.StreamWriter file = new System.IO.StreamWriter(playerDataFile);
                file.Write(stringBuilder.ToString());
                file.Close();
                Program.connexion = true;
                DemandeConnexion.ActiveForm.Close();
                return;
            }
        }

        /// <summary>
        /// Permet de mettre à jour la sélection et le label/couleur du bouton pour rejoindre une équipe.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox1.Text = string.Format("Se connecter : Equipe \"{0}\"", comboBox1.Text);
            switch (comboBox1.Text)
            {
                case "Bleu":
                    this.couleur = 1;
                    button1.Text = "Se connecter : Equipe \"Bleu\"";
                    button1.BackColor = Color.SkyBlue;
                    break;
                case "Rouge":
                    this.couleur = 2;
                    button1.Text = "Se connecter : Equipe \"Rouge\"";
                    button1.BackColor = Color.Tomato;
                    break;
                case "Orange":
                    this.couleur = 3;
                    button1.Text = "Se connecter : Equipe \"Orange\"";
                    button1.BackColor = Color.Orange;
                    break;
                case "Vert":
                    this.couleur = 4;
                    button1.Text = "Se connecter : Equipe \"Vert\"";
                    button1.BackColor = Color.LightGreen;
                    break;
                case "Jaune":
                    this.couleur = 5;
                    button1.Text = "Se connecter : Equipe \"Jaune\"";
                    button1.BackColor = Color.Yellow;
                    break;
                case "Rose":
                    this.couleur = 6;
                    button1.Text = "Se connecter : Equipe \"Rose\"";
                    button1.BackColor = Color.Pink;
                    break;

            }
        }

        //
        //
        //  - - - Fonctions générées par WinForm - - -
        //
        //

        private void DemandeConnexion_Load(object sender, EventArgs e)
        {

        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void Label2_Click(object sender, EventArgs e)
        {

        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Label5_Click(object sender, EventArgs e)
        {

        }

        private void TextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}
