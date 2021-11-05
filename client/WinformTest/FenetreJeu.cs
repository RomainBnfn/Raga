using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Media;

namespace WinformTest
{
    public partial class FenetreJeu : Form
    {
        private static FenetreJeu fenetreJeu;

        Thread receptionMessage;

        public delegate void TraiterLeMessage(Byte[] message);

        public static SoundPlayer soundPlayerCri = new SoundPlayer("cri.wav");
        public static SoundPlayer soundPlayerEpee = new SoundPlayer("epee.wav");

        public String playerDataFile;
        public String playerData;
        public int meilleurScore;

        public static bool isFreeze = true;


        public FenetreJeu()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// Initialise les éléments et valeurs pour la Winform
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FenetreJeu_Load(object sender, EventArgs e)
        {
            fenetreJeu = this;
            this.WindowState = FormWindowState.Maximized;
            //
            Classement.InitialiserClassement();
            String directoryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            playerDataFile = System.IO.Path.Combine(directoryPath,"playerData.txt");
            playerData = System.IO.File.ReadAllText(playerDataFile);
            System.Diagnostics.Debug.WriteLine(playerData);
            meilleurScore = int.Parse(playerData.Substring(playerData.LastIndexOf(';')+1));
            this.label13.Text = this.label13.Text + meilleurScore;
            //
            Joueur.InitialiserListeJoueurs();
            //
            Boule.InitialiserListeBoules();
            Boule.InitialiserListeAnciennesBoules();
            //
            receptionMessage = new Thread(BoucleEcoute);
            receptionMessage.IsBackground = true;
            receptionMessage.Start();
            //
            Thread iAmHereBoucle = new Thread(BoucleIAmHere);
            iAmHereBoucle.IsBackground = true;
            iAmHereBoucle.Start();
            //
            Timer.Start();  // Pour le déplacement
        }


        /// <summary>
        /// Thread d'écoute. Boucle infinie pour receptionner les messages.
        /// </summary>
        public void BoucleEcoute()
        {
            IPEndPoint ipReponse;
            Byte[] messageRecuByte;
            while (Thread.CurrentThread.IsAlive)
            {
                try
                {
                    ipReponse = new IPEndPoint(IPAddress.Any, 0);
                    messageRecuByte = Program.client.Receive(ref ipReponse);
                    Invoke((TraiterLeMessage)fenetreJeu.TraiterMessage, messageRecuByte);

                }
                catch (Exception e)
                {

                }

            }

        }

        /// <summary>
        /// A effectuer lors d'une réception d'un message.
        /// Cette fonction réagit en fonction du message reçu.
        /// </summary>
        /// <param name="messageRecuByte">Le message reçu</param>
        public void TraiterMessage(Byte[] messageRecuByte)
        {
            string messageString;
            int rayon;
            string pseudo;
            Joueur joueur;
            int position;
            int start;
            int x;
            int y;
            int color;

            switch (messageRecuByte[0])
            {
                // Un joueur s'est déplacé
                // Format = [1][string : x,y,pseudo "...[,]...[,]...[,]"
                case 1:
                    #region Un joueur s'est déplacé
                    // Message envoye : [1][string: x,y,pseudo "...[,]...[,]...[,]"]
                    messageString = Encoding.UTF8.GetString(Outils.RecupererTableau(messageRecuByte, 1));
                    // Trouver le x, y depuis le message String

                    start = 0;
                    position = messageString.IndexOf(',', start);
                    x = Int32.Parse(messageString.Substring(start, position));

                    start = position + 1;
                    position = messageString.IndexOf(',', start);
                    y = Int32.Parse(messageString.Substring(start, position - start));

                    start = position + 1;
                    position = messageString.IndexOf(',', start);
                    pseudo = messageString.Substring(start, position - start);

                    joueur = Joueur.GetJoueur(pseudo);
                    joueur.SetPosition(x, y);

                    Camera.addMoovingPlayer(joueur);
                    break;
                #endregion

                // Un joueur (autre) a grossi
                // Format = [2][NouveauRayon][PseudoJoueur]
                case 2:
                    #region Un joueur (autre) a grossi
                    messageString = Encoding.UTF8.GetString(Outils.RecupererTableau(messageRecuByte, 1));

                    start = 0;
                    position = messageString.IndexOf(',', start);
                    rayon = Int32.Parse(messageString.Substring(start, position));

                    start = position + 1;
                    position = messageString.IndexOf(',', start);
                    pseudo = messageString.Substring(start, position - start);

                    joueur = Joueur.GetJoueur(pseudo);
                    if (joueur == null) return;

                    int delta = rayon - joueur.GetRayon();
                    joueur.SetRayon(rayon);
                    joueur.GetCustomCircle().Refresh();
                    //
                    Classement.AjoutPointEquipe(joueur.GetCouleur(), delta);
                    break;
                #endregion

                // Une boule a été tuée
                // Format = [3][idBoule]
                case 3:
                    #region Une boule a été tuée
                    int idBoule = Int32.Parse(Encoding.UTF8.GetString(Outils.RecupererTableau(messageRecuByte, 1)));
                    Boule boule = Boule.GetBoule(idBoule);
                    if (boule == null) return;

                    Boule.Kill(boule);
                    break;
                #endregion

                // Une joueur a été tuée
                // Format = [4][pseudoJoueur]
                case 4:
                    #region Une joueur a été tuée
                    pseudo = Encoding.UTF8.GetString(Outils.RecupererTableau(messageRecuByte, 1));
                    soundPlayerCri.Play();
                    if (string.Equals(pseudo, Joueur.GetJoueurClient().GetPseudo()))
                    {
                        //FIN DE JEU
                        Timer.Stop();//Il ne peut plus se déplacer
                        MessageBox.Show("Vous avez été tué, votre jeu va se fermer.", "Mort = Fin de jeu");
                        if (meilleurScore < Joueur.GetJoueurClient().GetRayon())
                        {
                            playerData = playerData.Substring(0, playerData.LastIndexOf(';') + 1) + Joueur.GetJoueurClient().GetRayon();
                        }
                        System.IO.File.Delete(playerDataFile);
                        System.IO.StreamWriter file = new System.IO.StreamWriter(playerDataFile);
                        file.Write(playerData);
                        file.Close();
                        fenetreJeu.Close();
                        return;
                    }
                    joueur = Joueur.GetJoueur(pseudo);
                    if (joueur == null) return;
                    Joueur.Kill(joueur);
                    Classement.RetraitPointEquipe(joueur.GetCouleur(), joueur.GetRayon());
                    break;
                #endregion

                // Une nouvelle boule a été créee
                // Format = [5][string : id, x, y, color "...[,]...[,]...[,]...[,]"]
                case 5:
                    #region Une nouvelle boule a été créee
                    messageString = Encoding.UTF8.GetString(Outils.RecupererTableau(messageRecuByte, 1));
                    int id;
                    // Trouver le x, y depuis le message String
                    {
                        start = 0;
                        position = messageString.IndexOf(',', start);
                        id = Int32.Parse(messageString.Substring(start, position));
                        start = position + 1;
                        position = messageString.IndexOf(',', start);
                        x = Int32.Parse(messageString.Substring(start, position - start));
                        start = position + 1;
                        position = messageString.IndexOf(',', start);
                        y = Int32.Parse(messageString.Substring(start, position - start));
                        start = position + 1;
                        position = messageString.IndexOf(',', start);
                        color = Int32.Parse(messageString.Substring(start, position - start));
                    }
                    if (Boule.GetListeAnciennesBoules().ToArray().Length > 0)
                    {
                        Boule.Recreate(Boule.GetListeAnciennesBoules().First(), id, x, y, color);
                    }
                    else
                    {
                        Boule nouvelleBoule = new Boule(id, x, y, color);
                    }
                    break;
                #endregion

                // Un nouveau joueur a été créee
                // Format = [6][string : x, y, taille, color, pseudo "...[,]...[,]...[,]...[,]...[,]"]
                case 6:
                    #region Un nouveau joueur a été créee
                    messageString = Encoding.UTF8.GetString(Outils.RecupererTableau(messageRecuByte, 1));
                    // Trouver le x, y depuis le message String

                    {
                        start = 0;
                        position = messageString.IndexOf(',', start);
                        x = Int32.Parse(messageString.Substring(start, position));

                        start = position + 1;
                        position = messageString.IndexOf(',', start);
                        y = Int32.Parse(messageString.Substring(start, position - start));

                        start = position + 1;
                        position = messageString.IndexOf(',', start);
                        rayon = Int32.Parse(messageString.Substring(start, position - start));

                        start = position + 1;
                        position = messageString.IndexOf(',', start);
                        color = Int32.Parse(messageString.Substring(start, position - start));

                        start = position + 1;
                        position = messageString.IndexOf(',', start);
                        pseudo = messageString.Substring(start, position - start);
                    }
                    Joueur nouveauJoueur = new Joueur(pseudo, x, y, rayon, color);
                    break;
                #endregion

                // Les informations sur notre joueur client
                // Format = [7][string : x, y, taille, color, pseudo "...[,]...[,]...[,]...[,]...[,]"]
                case 7:
                    #region Recevoir les informations sur le joueur Client
                    messageString = Encoding.UTF8.GetString(Outils.RecupererTableau(messageRecuByte, 1));
                    // Trouver le x, y depuis le message String
                    {
                        start = 0;
                        position = messageString.IndexOf(',', start);
                        x = Int32.Parse(messageString.Substring(start, position));
                        start = position + 1;
                        position = messageString.IndexOf(',', start);
                        y = Int32.Parse(messageString.Substring(start, position - start));
                        start = position + 1;
                        position = messageString.IndexOf(',', start);
                        rayon = Int32.Parse(messageString.Substring(start, position - start));
                        start = position + 1;
                        position = messageString.IndexOf(',', start);
                        color = Int32.Parse(messageString.Substring(start, position - start));
                        start = position + 1;
                        position = messageString.IndexOf(',', start);
                        pseudo = messageString.Substring(start, position - start);
                    }
                    Joueur.SetJoueurClient(new Joueur(pseudo, x, y, rayon, color));
                    Joueur.GetJoueurClient().Display();
                    break;
                #endregion

                // Freeze
                case 253:
                    #region Freeze
                    if (messageRecuByte[1] == 0)
                    {
                        isFreeze = true;
                        break;
                    }
                    isFreeze = false;
                    break;
                #endregion

                // Kill
                case 254:
                    #region Kill
                    fenetreJeu.Close();
                    Close();
                    break;
                #endregion

                default:
                    break;
            }
        }


        /// <summary>
        /// Cette boucle envoie au serveur que le client existe encore toute les 0.5 secondes,
        /// afin qu'il sache qui est connecté et qui ne l'est pas.
        /// </summary>
        public void BoucleIAmHere()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                Byte[] iAmHere = { 0 };
                Program.client.Send(iAmHere, 1);
                Thread.Sleep(500);
            }
        }


        /// <summary>
        /// Calcule la nouvelle position du joueur Client et le déplace, et déplace la caméré si besoin.
        /// Affiche également les autres joueurs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (isFreeze)
            {
                return;
            }

            try
            {
                //Calcul nouvelle position du joueur
                int dX = (Cursor.Position.X - fenetreJeu.Left - (Joueur.GetJoueurClient().GetXPosition() - Camera.GetX()));

                int dY = (Cursor.Position.Y - fenetreJeu.Top  - (int)(Cursor.Size.Height*2/3.0) - (Joueur.GetJoueurClient().GetYPosition() - Camera.GetY()));

                double dist = Math.Sqrt(Math.Pow(dX, 2) + Math.Pow(dY, 2));

                if (dist <= Joueur.GetJoueurClient().GetRayon()*0.7
                    || dX == 0 && dY == 0)
                {
                    Camera.DisplayAllMoovingPlayers();
                    return;
                }

                double p = Joueur.GetKJoueurClient() / dist;

                Joueur.DeplacerJoueurClient((int)(dX * p), (int)(dY * p));
                xPos.Text = string.Format("x: {0}", Joueur.GetJoueurClient().GetXPosition());
                yPos.Text = string.Format("y: {0}", Joueur.GetJoueurClient().GetYPosition());
                //
                Camera.addMoovingPlayer(Joueur.GetJoueurClient());

                if (Camera.GetDistanceWithJoueurClient() > 250)
                {
                    Camera.ChangeCamera((int)(dX * p), (int)(dY * p));
                    return;
                }

                Joueur.GetJoueurClient().Display();
                Camera.DisplayAllMoovingPlayers();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Donne l'instance de la Winform de jeu
        /// </summary>
        /// <returns>La Winform du jeu</returns>
        public static FenetreJeu GetFenetreJeu()
        {
            return fenetreJeu;
        }
    }
}