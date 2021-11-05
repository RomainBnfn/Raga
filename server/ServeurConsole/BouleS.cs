using ServeurConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServeurBiblio
{

    public class BouleS
    {
        static private List<BouleS> listeBoules;
        static int idCursor;
        //
        int id;
        int xPosition;
        int yPosition;
        int color;

        /// <summary>
        /// Permet de stoquer les informations sur les Boules. (Petits éléments à manger) La BouleS (Serveur)
        /// est une version "simplifiée" qui ne comporte pas les éléments d'affichage, propres aux clients. 
        /// Chaque BouleS a une id unique pour chacune.
        /// </summary>
        /// <param name="id">L'id de la bouleS</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public BouleS(int id, int x, int y, int color)
        {
            //
            this.id = id;
            xPosition = x;
            yPosition = y;
            this.color = color;
            //
        }
        
        //
        // - - Getter - -
        //

        public int GetXPosition()
        {
            return xPosition;
        }

        public int GetYPosition()
        {
            return yPosition;
        }

        /// <summary>
        /// Retourne l'ID (unique) de la BouleS
        /// </summary>
        /// <returns>L'ID</returns>
        public int GetId()
        {
            return this.id;
        }

        public int GetColor()
        {
            return color;
        }

        //
        //  - - -  S t a t i c   P a r t  - - -
        //

        /// <summary>
        /// Initialiser la liste des BouleS par une liste vide.
        /// </summary>
        public static void InitialiserListe()
        {
            idCursor = 1;
            listeBoules = new List<BouleS>();
        }

        /// <summary>
        /// Retourne la liste des BouleS du serveur.
        /// </summary>
        /// <returns></returns>
        static public List<BouleS> GetListeBoule()
        {
            return listeBoules;
        }

        /// <summary>
        /// Crée et ajoute une nouvelle BouleS à la liste.
        /// </summary>
        /// <param name="x">La position X de la BouleS crée</param>
        /// <param name="y">La position Y de la BouleS crée</param>
        /// <param name="color">Le numéro de la Couleur de la BouleS crée</param>
        /// <returns></returns>
        static public int AddBoule(int x, int y, int color)
        {
            listeBoules.Add(new BouleS(idCursor, x, y, color));
            idCursor++;
            return idCursor - 1;
        }

        /// <summary>
        /// Supprime une BouleS de la liste des Boules.
        /// </summary>
        /// <param name="boule">La BouleS à supprimer.</param>
        static public void RemoveBoule(BouleS boule)
        {
            listeBoules.Remove(boule);
        }

        /// <summary>
        /// Retourne la boule possédant l'identifiant
        /// </summary>
        /// <param name="id">L'identifiant</param>
        /// <returns>La boule possédant l'identifiant (Null si aucune)</returns>
        static public BouleS GetBoule(int id)
        {
            foreach (BouleS boule in BouleS.GetListeBoule())
            {
                if (boule.GetId() == id)
                {
                    return boule;
                }
            }
            return null;
        }

        /// <summary>
        /// Ajoute une nouvelle boule dans la liste et l'envoie à tous les clients.
        /// </summary>
        static public void AjouterBoule()
        {
            if (BouleS.GetListeBoule().ToArray().Length > Program.nbBoulesMax) return;

            int x = Program.random.Next(250, Program.tailleEcranX - 5);
            int y = Program.random.Next(5, Program.tailleEcranY - 5);

            int color = Program.random.Next(1, 7);
            int id = BouleS.AddBoule(x, y, color);
            // Format : [5][string : id, x, y, color "...[,]...[,]...[,]...[,]"]
            Byte[] debutMessage = { 5 };
            Byte[] infoMessage = Encoding.UTF8.GetBytes(string.Format("{0},{1},{2},{3},", id, x, y, color));
            Outils.SendAll(Outils.FusionTableau(debutMessage, infoMessage));
        }
        //

        /// <summary>
        /// Crée une quantite de boule sur le terrain.
        /// </summary>
        static public void AjouterBoule(int quantite)
        {
            for (int i = 0; i < quantite; i++)
            {
                AjouterBoule();
            }
        }
    }
}
