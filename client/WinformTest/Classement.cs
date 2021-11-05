using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformTest
{
    /*
     *    Cette Class permet de gérer l'affichage et le
     *  rafraichissement du Classement en haut à gauche
     *  de l'écran de jeu.
     *  
     */

    public static class Classement
    {

        public static int nbEquipes = 6;
        private static int[] classementEquipes;

        /// <summary>
        /// Initialiser et Remplir le tableau de classement des équipes
        /// </summary>
        public static void InitialiserClassement()
        {
            classementEquipes = new int[2 * nbEquipes];
            for (int i = 0; i < nbEquipes; i++)
            {
                classementEquipes[2 * i] = i + 1;
                classementEquipes[2 * i + 1] = 0;
            }
            RefreshPointsClassement();
        }

        /// <summary>
        /// Ajoute des points à une équipe, puis réorganise le classement et raffraichit les bonnes valeurs (et positions) sur l'affichage.
        /// </summary>
        /// <param name="numeroEquipe">Le Numéro de l'équipe.</param>
        /// <param name="points">Le nombre de points.</param>
        public static void AjoutPointEquipe(int numeroEquipe, int points)
        {
            int position = GetIndiceEquipe(numeroEquipe);
            classementEquipes[position + 1] += points;
            int score = classementEquipes[position + 1];
            RefreshPointsClassement();

            while (position > 0) //L'équipe n'est pas déjà la première : on la remonte dans le classement si besoin
            {
                if (score > classementEquipes[position - 1]) //Si le score est plus grand que l'équipe qui est devant elle
                {
                    classementEquipes[position] = classementEquipes[position - 2];
                    classementEquipes[position + 1] = classementEquipes[position - 1];
                    classementEquipes[position - 2] = numeroEquipe;
                    classementEquipes[position - 1] = score;
                    RefreshIntituleClassement();
                }
                else
                {
                    break;
                }
                position -= 2;
            }
        }

        /// <summary>
        /// Retire des points à une équipe, puis réorganise le classement et raffraichit les bonnes valeurs (et positions) sur l'affichage.
        /// </summary>
        /// <param name="numeroEquipe">Le Numéro de l'équipe.</param>
        /// <param name="points">Le nombre de points.</param>
        public static void RetraitPointEquipe(int numeroEquipe, int points)
        {
            int position = GetIndiceEquipe(numeroEquipe);
            classementEquipes[position + 1] -= points;
            int score = classementEquipes[position + 1];
            RefreshPointsClassement();

            while (position < nbEquipes * 2 - 1) //L'équipe n'est pas déjà la dernière : on la descend dans le classement si besoin
            {
                if (score < classementEquipes[position + 3]) //Si le score est plus grand que l'équipe qui est devant elle
                {
                    classementEquipes[position] = classementEquipes[position + 2];
                    classementEquipes[position + 1] = classementEquipes[position + 3];
                    classementEquipes[position + 2] = numeroEquipe;
                    classementEquipes[position + 3] = score;
                    RefreshIntituleClassement();
                }
                else
                {
                    break;
                }
                position += 2;
            }
        }

        /// <summary>
        /// Rafraichit les valeurs des points du classement (sans classer)
        /// </summary>
        public static void RefreshPointsClassement()
        {
            FenetreJeu.GetFenetreJeu().label7.Text = classementEquipes[1].ToString();
            FenetreJeu.GetFenetreJeu().label8.Text = classementEquipes[3].ToString();
            FenetreJeu.GetFenetreJeu().label9.Text = classementEquipes[5].ToString();
            FenetreJeu.GetFenetreJeu().label10.Text = classementEquipes[7].ToString();
            FenetreJeu.GetFenetreJeu().label11.Text = classementEquipes[9].ToString();
            FenetreJeu.GetFenetreJeu().label12.Text = classementEquipes[11].ToString();
        }

        /// <summary>
        /// Rafraichit les intitulés des équipes du classement (sans classer)
        /// </summary>
        public static void RefreshIntituleClassement()
        {
            UpdateLigne(FenetreJeu.GetFenetreJeu().label1, FenetreJeu.GetFenetreJeu().label7, classementEquipes[0]);
            UpdateLigne(FenetreJeu.GetFenetreJeu().label2, FenetreJeu.GetFenetreJeu().label8, classementEquipes[2]);
            UpdateLigne(FenetreJeu.GetFenetreJeu().label3, FenetreJeu.GetFenetreJeu().label9, classementEquipes[4]);
            UpdateLigne(FenetreJeu.GetFenetreJeu().label4, FenetreJeu.GetFenetreJeu().label10, classementEquipes[6]);
            UpdateLigne(FenetreJeu.GetFenetreJeu().label5, FenetreJeu.GetFenetreJeu().label11, classementEquipes[8]);
            UpdateLigne(FenetreJeu.GetFenetreJeu().label6, FenetreJeu.GetFenetreJeu().label12, classementEquipes[10]);
        }

        /// <summary>
        /// Rafraichit la bonne ligne avec le nom et le score de l'équipe. 
        /// </summary>
        /// <param name="labelEquipe">Le label où sera affiché le nom de l'équipe</param>
        /// <param name="labelScore">Le label où sera affiché le score de l'équipe</param>
        /// <param name="numeroEquipe">Le numéro de l'équipe.</param>
        static void UpdateLigne(Label labelEquipe, Label labelScore, int numeroEquipe)
        {
            labelScore.Text = GetScoreEquipe(numeroEquipe).ToString();
            //
            labelEquipe.ForeColor = Outils.Couleurs[numeroEquipe];
            labelEquipe.Text = string.Format("Equipe \"{0}\"", GetNomEquipe(numeroEquipe));
        }


        /// <summary>
        /// Donne le score de l'équipe n°numeroEquipe
        /// </summary>
        /// <param name="numeroEquipe">Le n</param>
        /// <returns></returns>
        static public int GetScoreEquipe(int numeroEquipe)
        {
            if (nbEquipes == 0) return 0;
            return classementEquipes[GetIndiceEquipe(numeroEquipe) + 1];
        }

        /// <summary>
        /// Avoir l'indice d'apparition de l'équipe dans le classement.
        /// </summary>
        /// <param name="numeroEquipe">Le numéro de l'équipe</param>
        /// <returns>L'indice dans le tableau classement</returns>
        static public int GetIndiceEquipe(int numeroEquipe)
        {
            if (nbEquipes == 0) return 0;
            for (int i = 0; i < nbEquipes; i++)
            {
                if (classementEquipes[2 * i] == numeroEquipe) return 2 * i;
            }
            return 0;
        }

        /// <summary>
        /// Donne le nom de l'équipe n°numeroEquipe
        /// </summary>
        /// <param name="numeroEquipe">Le numéro de l'équipe</param>
        /// <returns>Le nom de l'équipe</returns>
        static public string GetNomEquipe(int numeroEquipe)
        {
            switch (numeroEquipe)
            {
                case 1:
                    return "Bleu";
                    break;
                case 2:
                    return "Rouge";
                    break;
                case 3:
                    return "Orange";
                    break;
                case 4:
                    return "Vert";
                    break;
                case 5:
                    return "Jaune";
                    break;
                case 6:
                    return "Rose";
                    break;
            }
            return "";
        }






    }
}
