using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformTest
{
    static public class Camera
    {
        static int X;
        static int Y;

        static Rectange[] listeBords;

        static private List<Joueur> listeJoueursADisplay;

        /// <summary>
        /// Déplace la caméra de dX, dY. (Calcule pour qu'elle ne s'éloigne pas trop du joueur)
        /// </summary>
        /// <param name="dX">Déplacement en X.</param>
        /// <param name="dY">Déplacement en Y.</param>
        static public void ChangeCamera(int dX, int dY)
        {
            X += dX;
            Y += dY;
            // Pour ne pas sortir du cadre de jeu
            if (X < -FenetreJeu.GetFenetreJeu().Width / 2
                || X > Joueur.GetMaxXPosition() - FenetreJeu.GetFenetreJeu().Width / 2
                || Y < -FenetreJeu.GetFenetreJeu().Height / 2
                || Y > Joueur.GetMaxYPosition() - FenetreJeu.GetFenetreJeu().Height / 2)
            {
                X -= dX;
                Y -= dY;
                return;
            }

            RedisplayAll();
        }

        /// <summary>
        /// Initialiser la position de la caméra à celle du joueur - les dimensions de l'écran..
        /// Puis créer et affiche les barrières (bords de map)
        /// </summary>
        static public void InitialiseCamera()
        {
            listeJoueursADisplay = new List<Joueur>();
            X = Joueur.GetJoueurClient().GetXPosition() - FenetreJeu.GetFenetreJeu().Width / 2;
            Y = Joueur.GetJoueurClient().GetYPosition() - FenetreJeu.GetFenetreJeu().Height / 2;
            CreationBarrieres();
        }

        /// <summary>
        /// Affiche à la bonne position uniquement les joueurs dans la liste listeJoueursADisplay, puis la clear.
        /// </summary>
        static public void DisplayAllMoovingPlayers()
        {
            foreach (Joueur joueur in listeJoueursADisplay)
            {
                int distX = Math.Abs(FenetreJeu.GetFenetreJeu().Width / 2 - joueur.GetXPosition() + X - joueur.GetRayon()),
                    distY = Math.Abs(FenetreJeu.GetFenetreJeu().Height / 2 - joueur.GetYPosition() + Y - -joueur.GetRayon());

                if (distX < FenetreJeu.GetFenetreJeu().Width / 2 
                    && distY < FenetreJeu.GetFenetreJeu().Height / 2)
                    joueur.Display();

            }
            listeJoueursADisplay.Clear();
        }

        /// <summary>
        /// Affiche tous les éléments AUTOUR du joueur. (Joueurs + Boules)
        /// </summary>
        static private void RedisplayAll()
        {
            int distX, distY;
            listeJoueursADisplay.Clear();
            foreach (Joueur joueur in Joueur.GetListeJoueurs())
            {
                if (joueur != Joueur.GetJoueurClient())
                {
                    distX = Math.Abs(FenetreJeu.GetFenetreJeu().Width / 2 + X - joueur.GetXPosition());
                    distY = Math.Abs(FenetreJeu.GetFenetreJeu().Height / 2 + Y - joueur.GetYPosition());

                    if (distX <= FenetreJeu.GetFenetreJeu().Width  + joueur.GetRayon()
                        && distY <= FenetreJeu.GetFenetreJeu().Height  + joueur.GetRayon())
                        joueur.Display();
                }
            }
            foreach (Boule boule in Boule.GetListeBoules())
            {
                distX = FenetreJeu.GetFenetreJeu().Width / 2 - boule.GetXPosition() + X;
                if (distX < 0)
                    distX = -distX;

                distY = FenetreJeu.GetFenetreJeu().Height / 2 - boule.GetYPosition() + Y;
                if (distY < 0)
                    distY = -distY;

                if (distX <= FenetreJeu.GetFenetreJeu().Width / 2 + 50
                    && distY <= FenetreJeu.GetFenetreJeu().Height / 2 + 50)
                    boule.Display();
            }
            DisplayBarrière();
        }
        
        /// <summary>
        /// Affiche à la bonne position les 4 bords de map.
        /// </summary>
        static void DisplayBarrière()
        {
            listeBords[0].Location = new System.Drawing.Point(-10, -2500 + FenetreJeu.GetFenetreJeu().Height / 2 - GetDistanceYWithJoueurClient()  - Joueur.GetJoueurClient().GetYPosition());
            listeBords[1].Location = new System.Drawing.Point(-2500 + FenetreJeu.GetFenetreJeu().Width / 2 - GetDistanceXWithJoueurClient() - Joueur.GetJoueurClient().GetXPosition(), -10);
            listeBords[2].Location = new System.Drawing.Point(-10, FenetreJeu.GetFenetreJeu().Height/2 - GetDistanceYWithJoueurClient() + (Joueur.maxYPosition - Joueur.GetJoueurClient().GetYPosition()));
            listeBords[3].Location = new System.Drawing.Point(FenetreJeu.GetFenetreJeu().Width/2 - GetDistanceXWithJoueurClient() + (Joueur.maxXPosition - Joueur.GetJoueurClient().GetXPosition()), -10);
        }

        /// <summary>
        /// Créer et affiche les barrières (bords de map)
        /// </summary>
        static public void CreationBarrieres()
        {
            listeBords = new Rectange[4];
            for (int i = 0; i < 4; i++)
            {
                listeBords[i] = new Rectange();
                listeBords[i].BackColor = Color.DarkGray;
                listeBords[i].Size = new System.Drawing.Size(2500, 2500);
                FenetreJeu.GetFenetreJeu().Controls.Add(listeBords[i]);
                listeBords[i].Show();
            }
            DisplayBarrière();
        }

        //
        // - - Getter - -
        //

        /// <summary>
        /// Retourne la distance entre Le joueur Client et la position de la Caméra
        /// </summary>
        /// <returns>La distance.</returns>
        static public int GetDistanceWithJoueurClient()
        {
            Joueur joueurClient = Joueur.GetJoueurClient();
            int x = FenetreJeu.GetFenetreJeu().Width / 2 - joueurClient.GetXPosition() + X, y = FenetreJeu.GetFenetreJeu().Height / 2 - joueurClient.GetYPosition() + Y;
            return (int)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }

        /// <summary>
        /// Retourne la distance entre Le joueur Client et la position de la Caméra, en ne considérant que l'axe X. 
        /// </summary>
        /// <returns>La distance.</returns>
        static public int GetDistanceXWithJoueurClient()
        {
            return FenetreJeu.GetFenetreJeu().Width / 2 - Joueur.GetJoueurClient().GetXPosition() + X;
        }

        /// <summary>
        /// Retourne la distance entre Le joueur Client et la position de la Caméra, en ne uniquement que l'axe Y. 
        /// </summary>
        /// <returns>La distance.</returns>
        static public int GetDistanceYWithJoueurClient()
        {
            return FenetreJeu.GetFenetreJeu().Height / 2 - Joueur.GetJoueurClient().GetYPosition() + Y;
        }

        /// <summary>
        /// Retourne la position X de la camera. (X commence à -Witdh / 2)
        /// </summary>
        /// <returns>X</returns>
        static public int GetX()
        {
            return X;
        }

        /// <summary>
        /// Retourne la position Y de la camera. (Y commence à -Height / 2)
        /// </summary>
        /// <returns>Y</returns>
        static public int GetY()
        {
            return Y;
        }

        //
        //  - - Setter - -
        //

        /// <summary>
        /// Ajoute un joueur à la liste listeJoueursADisplay
        /// </summary>
        /// <param name="joueur">Le joueur à ajouter.</param>
        static public void addMoovingPlayer(Joueur joueur)
        {
            listeJoueursADisplay.Add(joueur);
        }

    }
}
