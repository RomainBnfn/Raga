using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using ServeurConsole;
using System.Threading;

namespace ServeurBiblio
{
    public class JoueurS
    {
        const int tailleInitiale = 20;

        private static List<JoueurS> listeJoueurs;

        IPEndPoint ip;
        public int iAmHereNumber;
        private string pseudo;
        private int rayon;
        private int xPosition;
        private int yPosition;
        private int color;

        /// <summary>
        /// Permet de stoquer les informations sur les Joueur. Le JoueurS (Serveur) est une version "simplifiée" 
        /// qui ne comporte pas les éléments d'affichage, propres aux clients. Chaque JoueurS a un pseudo qui
        /// est unique.
        /// </summary>
        /// <param name="ip">L'Ip de connexion du client associé au joueur.</param>
        /// <param name="pseudo">Le pseudo du joueur.</param>
        /// <param name="rayon">La taille du joueur.</param>
        /// <param name="x">La position x du joueur.</param>
        /// <param name="y">La position y du joueur.</param>
        /// <param name="color">Le numéro de la couleur du joueur.</param>
        public JoueurS(IPEndPoint ip, string pseudo, int rayon, int x, int y, int color)
        {
            this.ip = ip;
            this.pseudo = pseudo;
            this.rayon = rayon;
            xPosition = x;
            yPosition = y;
            this.color = color;
            this.iAmHereNumber = 4;
        }

        //
        // - - Getter - -
        //

        public IPEndPoint GetIp()
        {
            return ip;
        }

        public string GetPseudo()
        {
            return pseudo;
        }

        public int GetRayon()
        {
            return this.rayon;
        }

        public int GetXPosition()
        {
            return xPosition;
        }

        public int GetYPosition()
        {
            return yPosition;
        }

        public int GetColor()
        {
            return color;
        }

        /// <summary>
        /// Retourne si le client a reccemment envoyé un message pour indiquer sa présence (si le 'iAmHereNumber' est positif)
        /// </summary>
        /// <returns>Si le client est toujours actif. (Connecté)</returns>
        public bool isHere()
        {
            return iAmHereNumber > 0;
        }

        //
        // - - Setter - -
        //

        public void SetRayon(int newRayon)
        {
            this.rayon = newRayon;
        }

        public void SetPosition(int x, int y)
        {
            this.xPosition = x;
            this.yPosition = y;
        }

        public void removeIAmHereNumber()
        {
            iAmHereNumber--;
        }

        public void SetIAmHereNumber()
        {
            iAmHereNumber = 4;
        }

        //
        //  - - - S t a t i c   P a r t - - -
        //

        /// <summary>
        /// Initialiser la liste des Joueurs par une liste vide.
        /// </summary>
        public static void InitialiserListe()
        {
            listeJoueurs = new List<JoueurS>();
        }

        /// <summary>
        /// Donne la liste de tous les JoueurS.
        /// </summary>
        /// <returns>La liste de tous les JoueurS.</returns>
        static public List<JoueurS> GetListe()
        {
            return listeJoueurs;
        }

        static public int getRayonInitial()
        {
            return tailleInitiale;
        }

        /// <summary>
        /// Ajoute un JoueurS à la liste de tous les JoueurS du serveur.
        /// </summary>
        /// <param name="joueur">Le JoueurS</param>
        static public void AddJoueur(JoueurS joueur)
        {
            listeJoueurs.Add(joueur);
        }

        /// <summary>
        /// Retire un JoueurS de la liste de tous les JoueurS du serveur.
        /// </summary>
        /// <param name="joueur">Le JoueurS</param>
        static public void RemoveJoueur(JoueurS joueur)
        {
            listeJoueurs.Remove(joueur);
        }

        /// <summary>
        /// Informe sur l'existence d'un joueur possedant le pseudo
        /// </summary>
        /// <param name="ip">Le pseudo à tester</param>
        /// <returns>Si un client a déjà ce pseudo</returns>
        static public bool ExisteJoueur(string pseudo)
        {
            foreach (JoueurS joueur in JoueurS.GetListe())
            {
                if (string.Equals(joueur.GetPseudo(), pseudo))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Retourne le joueur possédant l'ip
        /// </summary>
        /// <param name="ip">L'ip</param>
        /// <returns>Le joueur possédant l'ip (Null si aucun)</returns>
        static public JoueurS GetJoueur(IPEndPoint ip)
        {
            foreach (JoueurS joueur in JoueurS.GetListe())
            {
                if (IPEndPoint.Equals(joueur.GetIp(), ip))
                {
                    return joueur;
                }
            }
            return null;
        }
        //
        /// <summary>
        /// Retourne le joueur possédant le pseudo
        /// </summary>
        /// <param name="pseudo">Le pseudo</param>
        /// <returns>Le joueur possédant le pseudo (Null si aucun)</returns>
        static public JoueurS GetJoueur(string pseudo)
        {
            foreach (JoueurS joueur in JoueurS.GetListe())
            {
                if (string.Equals(joueur.GetPseudo(), pseudo))
                {
                    return joueur;
                }
            }
            return null;
        }

        /// <summary>
        /// Ajoute un nouveau joueur dans la liste et l'envoie à tous les clients
        /// </summary>
        /// <param name="ip">L'ip du nouveau joueur</param>
        /// <param name="pseudo">Le pseudo du nouveau joueur</param>
        /// <param name="x">La coordonnée x du nouveau joueur</param>
        /// <param name="y">La coordonnée y du nouveau joueur</param>
        static public void AddNouveauJoueur(IPEndPoint ip, string pseudo, int x, int y, int color)
        {
            JoueurS.AddJoueur(new JoueurS(ip, pseudo, JoueurS.getRayonInitial(), x, y, color));

            // Envoie au nouveau client son personnage
            Byte[] debutMessageClient = { 7 };
            Byte[] infoMessageClient = Encoding.UTF8.GetBytes(string.Format("{0},{1},{2},{3},{4},", x, y, JoueurS.getRayonInitial(), color, pseudo));
            Outils.SendToClient(5, string.Format("{0},{1},{2},{3},{4},", x, y, JoueurS.getRayonInitial(), color, pseudo), ip);

            // Envoie aux anciens clients le nouveau personnage
            Byte[] debutMessage = { 6 };
            Byte[] infoMessage = Encoding.UTF8.GetBytes(string.Format("{0},{1},{2},{3},{4},", x, y, JoueurS.getRayonInitial(), color, pseudo));
            Outils.SendAllExecpt(Outils.FusionTableau(debutMessage, infoMessage), ip);
            Thread.Sleep(1);
            SendAllOldJoueurs(ip);
        }

        /// <summary>
        /// Envoie tous les anciens joueurs et boules au client dont l'ip est ip
        /// </summary>
        /// <param name="ip">L'ip du client</param>
        static public void SendAllOldJoueurs(IPEndPoint ip)
        {
            int x, y, rayon, color, id;
            string pseudo;
            foreach (JoueurS joueur in JoueurS.GetListe())
            {
                IPEndPoint ipJoueurFor = joueur.GetIp();
                if (!IPEndPoint.Equals(ip, ipJoueurFor))
                {
                    x = joueur.GetXPosition();
                    y = joueur.GetYPosition();
                    rayon = joueur.GetRayon();
                    pseudo = joueur.GetPseudo();
                    color = joueur.GetColor();
                    Outils.SendToClient(5, string.Format("{0},{1},{2},{3},{4},", x, y, rayon, color, pseudo), ip);
                }
            }

            foreach (BouleS boule in BouleS.GetListeBoule())
            {
                x = boule.GetXPosition();
                y = boule.GetYPosition();
                id = boule.GetId();
                color = boule.GetColor();
                Outils.SendToClient(5, string.Format("{0},{1},{2},{3},", id, x, y, color), ip);
            }
        }
    }
}
