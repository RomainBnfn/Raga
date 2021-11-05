using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Drawing;

namespace WinformTest
{
    public class Joueur
    {
        // Toutes les const sont en static, car pas de risque de modification dans le programme.
        public const int rayonMax = 400;
        public const int grossissementApresMangerBoule = 2;
        public const double kInitial = 12;
        public const double kFinal = 4;

        public const int maxXPosition = 5000;
        public const int maxYPosition = 5000;

        static private List<Joueur> listeJoueurs; //Contients tous les joueurs
        static private Joueur joueurClient;
        static double kJoueurClient = kInitial; //Constante de déplacement du joueur Client.

        //
        // --- Attributs ---
        //
        private string pseudo;
        private int xPosition;
        private int yPosition;
        private int rayon;
        private int couleur;
        private CustomCircle customCircle;

        /// <summary>
        /// Un joueur est élément permettant de stoquer les positions, rayons, couleur et forme Winform de tout joueur
        /// (celui du client et ceux connectés sur le serveur).
        /// </summary>
        /// <param name="pseudo">Le pseudo du joueur.</param>
        /// <param name="x">La position en X.</param>
        /// <param name="y">La position en Y.</param>
        /// <param name="rayon">Le rayon du joueur.</param>
        /// <param name="intColor">L'indice de la Couleur. (Voir Outils)</param>
        public Joueur(string pseudo, int x, int y, int rayon, int intColor)
        {
            // 
            this.pseudo = pseudo;
            this.xPosition = x;
            this.yPosition = y;
            this.rayon = rayon;
            this.couleur = intColor;
            AjouterJoueur(this);
            //
            customCircle = new CustomCircle();
            customCircle.BackColor = Outils.Couleurs[intColor];
            customCircle.Location = new System.Drawing.Point(xPosition - rayon - Camera.GetX(), yPosition - rayon - Camera.GetY());
            customCircle.Size = new System.Drawing.Size(rayon * 2, rayon * 2);
            customCircle.Text = pseudo;
            FenetreJeu.GetFenetreJeu().Controls.Add(customCircle);
            Display();

            Classement.AjoutPointEquipe(couleur, rayon);
        }

        /// <summary>
        /// Grossit le joueur et l'affiche correctement. (Sans aucun envoie au serveur)
        /// </summary>
        /// <param name="taille">La nouvelle taille</param>
        public void Grossir(int taille)
        {
            if (FenetreJeu.isFreeze) return;
            if (rayon + taille > rayonMax)
            {
                this.rayon = rayonMax;
                Display();
                return;
            }
            //
            this.rayon += taille;
            Display();
        }

        /// <summary>
        /// Affiche correctement le joueur en fonction de la position de la Camera du client.
        /// </summary>
        public void Display()
        {
            customCircle.Size = new System.Drawing.Size(rayon * 2, rayon * 2);
            customCircle.Location = new System.Drawing.Point(xPosition - rayon - Camera.GetX(), yPosition - rayon - Camera.GetY());
        }


        /* 
         *  -  -   Getter   -   -
         */

        public int GetCouleur()
        {
            return couleur;
        }

        public CustomCircle GetCustomCircle()
        {
            return customCircle;
        }

        public int GetXPosition()
        {
            return xPosition;
        }

        public int GetYPosition()
        {
            return yPosition;
        }

        public int GetRayon()
        {
            return this.rayon;
        }

        public string GetPseudo()
        {
            return this.pseudo;
        }


        /*
         *   -  -   Setter   -   -
         */

        /// <summary>
        /// Définit le rayon du joueur
        /// </summary>
        /// <param name="rayon">Le rayon.</param>
        public void SetRayon(int rayon)
        {
            this.rayon = rayon;
        }

        /// <summary>
        /// Définit les positions X et Y du joueur.
        /// </summary>
        /// <param name="x">Position X</param>
        /// <param name="y">Position Y</param>
        public void SetPosition(int x, int y)
        {
            xPosition = x;
            yPosition = y;
        }


        /*
        *   - - - - - - S T A T I C +  J O U E U R    C L I E N T - - - - - - - 
        */

        /// <summary>
        /// Déplacer le joueur principal (celui du client) de dx, dy
        /// </summary>
        /// <param name="dx">Déplacement en x</param>
        /// <param name="dy">Déplacement en y</param>
        /// 
        static public void DeplacerJoueurClient(int dx, int dy)
        {
            if (joueurClient == null) return;
            if (FenetreJeu.isFreeze) return;

            int x = joueurClient.GetXPosition() + dx,
                y = joueurClient.GetYPosition() + dy;

            // Pour ne pas sortir du cadre de jeu
            if (x <= 0 || x >= maxXPosition || y <= 0 || y >= maxYPosition) return;

            joueurClient.SetPosition(x, y);

            ToutMangerJoueurClient();

            // On n'envoit pas le pseudo : le serveur nous connait déjà
            string message = string.Format("{0},{1},", x, y);
            Outils.SendToServer(1, message);
        }

        /// <summary>
        /// Grossir le joueur principal du client et envoyer l'information au serveur
        /// </summary>
        /// <param name="grossissement">La taille à ajouter au rayon de base</param>
        static public void GrossirJoueurClient(int grossissement)
        {
            if (FenetreJeu.isFreeze) return;

            if (grossissement == 0)
                return;

            if (joueurClient.GetRayon() + grossissement > Joueur.rayonMax)
                grossissement = Joueur.rayonMax - joueurClient.GetRayon();

            joueurClient.Grossir(grossissement);

            Outils.SendToServer(2, joueurClient.GetRayon().ToString());

            kJoueurClient = (20 - joueurClient.GetRayon()) * (kInitial-kFinal) / (rayonMax-20) + kInitial;
            Classement.AjoutPointEquipe(joueurClient.GetCouleur(), grossissement);
            joueurClient.GetCustomCircle().Refresh();
        }

        /// <summary>
        /// A lancer après un déplacement, permet au client de manger toutes les boules et joueurs
        /// possibles après un déplacement.
        /// </summary>
        static public void ToutMangerJoueurClient()
        {
            if (FenetreJeu.isFreeze) return;
            int grossissement = 0;
            List<Joueur> joueursAKill = new List<Joueur>();
            //
            //
            foreach (Joueur joueurFor in Joueur.GetListeJoueurs())
            {
                if (joueurClient != joueurFor)
                {
                    if ((GetDistance(joueurClient, joueurFor) < joueurClient.GetRayon())
                        && (joueurClient.GetRayon() > joueurFor.GetRayon())
                        && (joueurClient.GetCouleur() != joueurFor.GetCouleur()))
                    {
                        grossissement += joueurFor.GetRayon() / 3;
                        //
                        joueursAKill.Add(joueurFor);
                    }
                }
            }
            foreach (Joueur joueurFor in joueursAKill)
            {
                // Une demande de type 4 correspond à kill d'un joueur.
                Outils.SendToServer(4, joueurFor.GetPseudo());
                //
                Kill(joueurFor);
                FenetreJeu.soundPlayerCri.Play();
                Classement.RetraitPointEquipe(joueurFor.GetCouleur(), joueurFor.GetRayon());
            }
            //
            //

            List<Boule> boulesAKill = new List<Boule>();

            foreach (Boule boule in Boule.GetListeBoules())
            {
                if (GetDistance(joueurClient, boule) < joueurClient.GetRayon())
                {
                    grossissement += grossissementApresMangerBoule;
                    //
                    //
                    boulesAKill.Add(boule);
                }
            }
            foreach (Boule boule in boulesAKill)
            {
                Byte[] code = { 3 };  // Une demande de type 3 correspond à un kill d'une boule.
                Byte[] idBouleByte = Encoding.UTF8.GetBytes(boule.GetId().ToString());
                Program.client.Send(Outils.FusionTableau(code, idBouleByte), idBouleByte.Length + 1);
                //
                Boule.Kill(boule);
                FenetreJeu.soundPlayerEpee.Play();
            }
            //
            if (grossissement != 0)
            {
                GrossirJoueurClient(grossissement);
            }
        }

        /// <summary>
        /// Cette fonction désaffiche et SUPPRIME un joueur
        /// </summary>
        /// <param name="joueur">Le joueur à supprimer</param>
        static public void Kill(Joueur joueur)
        {
            joueur.GetCustomCircle().Hide();
            joueur.GetCustomCircle().Dispose();
            Joueur.RetirerJoueur(joueur);
        }

        /// <summary>
        /// Retourne la distance entre les centres de deux joueurs, en utilisant la norme infini
        /// </summary>
        /// <param name="joueurA">Le premier joueur à tester</param>
        /// <param name="joueurB">Le second joueur à tester</param>
        /// <returns>La distance entre les deux entités</returns>
        static public double GetDistance(Joueur joueurA, Joueur joueurB)
        {
            int x = Math.Abs(joueurA.GetXPosition() - joueurB.GetXPosition());
            int y = Math.Abs(joueurA.GetYPosition() - joueurB.GetYPosition());

            return Math.Max(x, y);
        }
        //
        /// <summary>
        /// Retourne la distance entre les centres d'un joueur et d'une boule, en utilisant la norme infini
        /// </summary>
        /// <param name="joueurA">Le premier joueur à tester</param>
        /// <param name="boule">La boule à tester</param>
        /// <returns>La distance entre les deux entités</returns>
        static public double GetDistance(Joueur joueurA, Boule boule)
        {
            int x = Math.Abs(joueurA.GetXPosition() - boule.GetXPosition());
            int y = Math.Abs(joueurA.GetYPosition() - boule.GetYPosition());

            return Math.Max(x, y);
        }

        /// <summary>
        /// Définit le joueur Client. A n'utiliser qu'une fois en début de lancement.
        /// </summary>
        /// <param name="joueur"></param>
        static public void SetJoueurClient(Joueur joueur)
        {
            if (joueurClient != null)
                return;

            joueurClient = joueur;
            Camera.InitialiseCamera();
        }

        /*
         *   -  -   Getter   -   -
         */

        /// <summary>
        /// Retourne la liste de TOUS les joueurs connus actuelement par le Client.
        /// </summary>
        /// <returns>La liste</returns>
        static public List<Joueur> GetListeJoueurs()
        {
            return listeJoueurs;
        }

        /// <summary>
        /// Rejourne le joueur du client.
        /// </summary>
        /// <returns>Le joueur</returns>
        static public Joueur GetJoueurClient()
        {
            return joueurClient;
        }

        /// <summary>
        /// Permet de récupérer le joueur dont le pseudo est pseudo.
        /// </summary>
        /// <param name="pseudo">Le pseudo du joueur à récupérer</param>
        /// <returns>Le joueur concerné</returns>
        static public Joueur GetJoueur(string pseudo)
        {
            foreach (Joueur joueur in Joueur.GetListeJoueurs())
            {
                if (string.Equals(joueur.GetPseudo(), pseudo))
                {
                    return joueur;
                }
            }
            return null;
        }

        /// <summary>
        /// Retourne la variable de vitesse de déplacement du joueur client
        /// </summary>
        /// <returns>La variable de vitesse de déplacement.</returns>
        static public double GetKJoueurClient()
        {
            return kJoueurClient;
        }

        /// <summary>
        /// Retourne la largeur de la map, le max en X (taille de la map)
        /// </summary>
        /// <returns>X</returns>
        static public int GetMaxXPosition()
        {
            return maxXPosition;
        }

        /// <summary>
        /// Retourne la hauteur de la map, le max en Y (taille de la map)
        /// </summary>
        /// <returns>Y</returns>
        static public int GetMaxYPosition()
        {
            return maxYPosition;
        }

        /// <summary>
        ///  Cette fonction est à utiliser avant d'essayer d'ajouter
        ///  ou supprimer un joueur, boule...
        /// </summary>
        static public void InitialiserListeJoueurs()
        {
            listeJoueurs = new List<Joueur>();
        }

        /// <summary>
        /// Ajoute le joueur à la liste des joueurs connus par le client.
        /// </summary>
        /// <param name="joueur">Le (nouveau) joueur à ajouter</param>
        static private void AjouterJoueur(Joueur joueur)
        {
            listeJoueurs.Add(joueur);
        }

        /// <summary>
        /// Retire un joueur de la liste des joueurs connus par le Client.
        /// </summary>
        /// <param name="joueur">Le joueur à retirer</param>
        static public void RetirerJoueur(Joueur joueur)
        {
            listeJoueurs.Remove(joueur);
        }

        /// <summary>
        /// Permet de tester l'égaliter entre deux joueurs. (Basé sur les pseudos)
        /// </summary>
        /// <param name="obj">L'autre joueur à tester</param>
        /// <returns>Si égalité</returns>
        public override bool Equals(Object obj)
        {
            Joueur joueurB = obj as Joueur;
            if (joueurB == null)
                return false;
            else
                return string.Equals(this.pseudo, joueurB.GetPseudo());
        }

    }
}
