using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformTest
{

    public class Boule
    {
        static public int sizeBouleInitiale = 7;
        static private List<Boule> listeBoules;
        static private List<Boule> listeAnciennesBoules;
        //
        int id;
        int xPosition;
        int yPosition;
        //
        private CustomCircle customCircle;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="intColor"></param>
        public Boule(int id, int x, int y, int intColor)
        {
            //
            this.id = id;
            xPosition = x;
            yPosition = y;
            AjouterListe(this);
            //
            customCircle = new CustomCircle();
            customCircle.BackColor = Outils.Couleurs[intColor];
            Display();
            customCircle.Size = new System.Drawing.Size(sizeBouleInitiale*2, sizeBouleInitiale*2);
            customCircle.Text = "";
            FenetreJeu.GetFenetreJeu().Controls.Add(customCircle);
        }

        /// <summary>
        /// Affiche la Boule à la bonne position par rapport à la Camera du Client.
        /// </summary>
        public void Display()
        {
            customCircle.Location = new System.Drawing.Point(xPosition - sizeBouleInitiale - Camera.GetX(), yPosition - sizeBouleInitiale - Camera.GetY());
        }

        //
        // - - Getter - -
        //

        /// <summary>
        /// Retourne le "customCircle", l'élément graphique du Winform.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Retourne l'Id unique de la Boule.
        /// </summary>
        /// <returns></returns>
        public int GetId()
        {
            return this.id;
        }





        //
        // - - - S t a t i c  P a r t - - -
        //

        /// <summary>
        /// /!\ Ne doit pas être manipulé n'importe comment !
        /// Cette fonction permet de recrééer une boule avec des nouvelles informations. 
        /// (Au lieu de créer un nouvel objet, et donc de saturer les élements présents sur le Winform.
        /// </summary>
        /// <param name="boule">La boule à écraser.</param>
        /// <param name="id">L'id de la nouvelle boule.</param>
        /// <param name="x">La nouvelle position x de la boule.</param>
        /// <param name="y">La nouvelle position y de la boule.</param>
        /// <param name="color">La nouvelle couleur de la boule.</param>
        internal static void Recreate(Boule boule, int id, int x, int y, int color)
        {
            boule.id = id;
            boule.xPosition = x;
            boule.yPosition = y;
            AjouterListe(boule);
            //
            boule.customCircle.BackColor = Outils.Couleurs[color];
            boule.customCircle.Location = new System.Drawing.Point(x - sizeBouleInitiale, y - sizeBouleInitiale);
            boule.customCircle.Size = new System.Drawing.Size(sizeBouleInitiale * 2, sizeBouleInitiale * 2);
            boule.customCircle.Text = "";
            FenetreJeu.GetFenetreJeu().Controls.Add(boule.customCircle);
            boule.customCircle.Update();
            boule.customCircle.Refresh();
            listeAnciennesBoules.Remove(boule);
            boule.Display();
        }

        //
        /// <summary>
        /// Cette fonction désaffiche et SUPPRIME une boule
        /// </summary>
        /// <param name="boule">La boule à supprimer</param>
        public static void Kill(Boule boule)
        {
            boule.GetCustomCircle().Location = new System.Drawing.Point(-4000, -4000);
            Boule.RetirerListe(boule);
            listeAnciennesBoules.Add(boule);
            boule.customCircle.BackColor = Color.Gray;
        }

        /// <summary>
        /// Initialise la Liste des Boules par une liste Vide.
        /// </summary>
        public static void InitialiserListeBoules()
        {
            listeBoules = new List<Boule>();
        }

        /// <summary>
        /// Initialise la Liste des Anciennes Boules par une liste Vide.
        /// </summary>
        public static void InitialiserListeAnciennesBoules()
        {
            listeAnciennesBoules = new List<Boule>();
        }

        //
        //  - - Getter - -
        //

        static public List<Boule> GetListeAnciennesBoules()
        {
            return listeAnciennesBoules;
        }


        public static List<Boule> GetListeBoules()
        {
            return listeBoules;
        }

        /// <summary>
        /// Permet de récupérer la boule dont l'id est id
        /// </summary>
        /// <param name="id">L'id de la boule</param>
        /// <returns>La boule en question</returns>
        public static Boule GetBoule(int id)
        {
            foreach (Boule boule in Boule.GetListeBoules())
            {
                if (boule.GetId() == id)
                {
                    return boule;
                }
            }
            return null;
        }

        //
        // - - Setter - -
        //

        /// <summary>
        /// Ajoute une Boule dans la liste des Boules Actuelles.
        /// </summary>
        /// <param name="boule">La boule</param>
        public static void AjouterListe(Boule boule)
        {
            listeBoules.Add(boule);
        }

        /// <summary>
        /// Retire une Boule de la liste des Boules Actuelles.
        /// </summary>
        /// <param name="boule">La boule</param>
        public static void RetirerListe(Boule boule)
        {
            listeBoules.Remove(boule);
        }
    }
}
