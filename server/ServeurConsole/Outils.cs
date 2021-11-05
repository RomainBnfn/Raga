using ServeurBiblio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServeurConsole
{
    static public class Outils
    {


        //
        //  - - - O U T I L S   S E N D   M E S S A G E S - - -
        //

        /// <summary>
        /// Envoie un message en Byte[] à tous les joueurs dans la liste des joueurs.
        /// </summary>
        /// <param name="message">Le message à envoyer</param>
        static public void SendAll(Byte[] message)
        {
            foreach (JoueurS joueur in JoueurS.GetListe())
            {
                IPEndPoint ip = joueur.GetIp();
                Program.serveur.Send(message, message.Length, ip);
            }
        }

        /// <summary>
        /// Envoie un message à tous les joueurs sauf un seul.
        /// </summary>
        /// <param name="message">Le message à envoyer</param>
        /// <param name="ipException">L'ip qui ne recevera pas le message</param>s
        static public void SendAllExecpt(Byte[] message, IPEndPoint ipException)
        {
            foreach (JoueurS joueur in JoueurS.GetListe())
            {
                IPEndPoint ip = joueur.GetIp();
                if (!IPEndPoint.Equals(ip, ipException))
                {
                    Program.serveur.Send(message, message.Length, ip);
                }
            }
        }

        /// <summary>
        /// Envoie le message au serveur avec son l'indentifiant/code
        /// /!\ Attention : Les arguments sont séparés par des virgules.
        /// </summary>
        /// <param name="code">Le code/identifiant</param>
        /// <param name="message">Le message à envoyer</param>
        /// <param name="ip">L'ip du destinataire</param>
        public static void SendToClient(int code, string message, IPEndPoint ip)
        {
            Byte[] codeByte = { (Byte)code };
            Byte[] messageByte = Encoding.UTF8.GetBytes(message);
            Program.serveur.Send(Outils.FusionTableau(codeByte, messageByte), messageByte.Length + 1);
        }

        //
        //  - - - O U T I L S   T A B L E A U X - - -
        //

        /// <summary>
        /// Permet de récupérer un tableau concaténant le tableau 1 puis 2.
        /// </summary>
        /// <param name="tableau1">Le premier tableau</param>
        /// <param name="tableau2">Le second tableau</param>
        /// <returns>La fusion des deux tableaux</returns>
        static public Byte[] FusionTableau(Byte[] tableau1, Byte[] tableau2)
        {
            int len = tableau1.Length + tableau2.Length;
            Byte[] tableauFusion = new Byte[len];
            for (int i = 0; i < tableau1.Length; i++)
            {
                tableauFusion[i] = tableau1[i];
            }
            for (int i = 0; i < tableau2.Length; i++)
            {
                tableauFusion[tableau1.Length + i] = tableau2[i];
            }
            return tableauFusion;
        }


        /// <summary>
        /// Permet de récupérer une partie d'un tableau à partie de l'indice indice jusqu'à la fin du tableau.
        /// </summary>
        /// <param name="tableau">Le tableau initial</param>
        /// <param name="indice">L'indice de départ</param>
        /// <returns></returns>
        static public Byte[] RecupererTableau(Byte[] tableau, int indice)
        {
            int len = tableau.Length - indice;
            Byte[] nouveauTab = new Byte[len];
            for (int i = indice; i < len + indice; i++)
            {
                nouveauTab[i - indice] = tableau[i];
            }
            return nouveauTab;
        }


        /// <summary>
        /// Permet de récupérer une partie d'un tableau à partie de l'indice indice jusqu'à l'indice de fin.
        /// </summary>
        /// <param name="tableau">Le tableau initial</param>
        /// <param name="indiceDebut">L'indice de départ</param>
        /// <param name="indiceFin">L'indice de fin</param>
        /// <returns></returns>
        static public Byte[] RecupererTableau(Byte[] tableau, int indiceDebut, int indiceFin)
        {
            if (indiceFin <= indiceDebut) return null;

            int len = indiceFin - indiceDebut;
            Byte[] nouveauTab = new Byte[len];
            for (int i = indiceDebut; i < indiceFin + indiceDebut; i++)
            {
                nouveauTab[i - indiceDebut] = tableau[i];
            }
            return nouveauTab;
        }
    }
}
