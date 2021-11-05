using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformTest
{
    static public class Outils
    {
        static public Color[] Couleurs = { Color.White, Color.SkyBlue, Color.Tomato, Color.Orange, Color.LightGreen, Color.Yellow, Color.Pink };
        
        /// <summary>
        /// Cette méthode retourne si une connexion avec un potentiel serveur est possible
        /// avec un pseudo donné.
        /// </summary>
        /// <param name="ipServer">L'adresse ip du serveur potentiel</param>
        /// <param name="port">Le port d'écoute de la machine du serveur</param>
        /// <param name="pseudo">Le pseudo à tester</param>
        /// <param name="client">Le client</param>
        /// <returns>Si la connexion est possible</returns>
        public static bool DemanderConnexion(string ipServer, int port, string pseudo, UdpClient client, Byte idColor)
        {
            if (port == -1) return false;
            try
            {
                client.Connect(ipServer, port);

                Byte[] messageEnvoyeByte = { 255, idColor };  // Une demande de type 255 correspond à une demande de connexion pour un pseudo précis.
                Byte[] pseudoByte = Encoding.UTF8.GetBytes(pseudo);

                client.Send(FusionTableau(messageEnvoyeByte, pseudoByte), pseudoByte.Length + 2);

                IPEndPoint ipReponse = new IPEndPoint(IPAddress.Any, 0);
                Byte[] messageRecuByte = client.Receive(ref ipReponse);

                if (messageRecuByte[0] == 255)
                {
                    if (messageRecuByte[1] == 0)
                    {
                        Program.connexion = true;
                        return true; 
                    }
                    if (messageRecuByte[1] == 1)
                    {
                        MessageBox.Show("Le serveur est vérouillé.", "Serveur innaccessible");
                        return false;
                    }
                    if (messageRecuByte[1] == 2)
                    {
                        MessageBox.Show("Le pseudo est déjà prit.", "Pseudo déjà utilisé !");
                        return false;
                    }

                }
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show("Le serveur n'existe pas.", "Serveur innaccessible");
                return false;
            }
        }

        
        /// <summary>
        /// Permet de récupérer un tableau concaténant le tableau 1 puis 2.
        /// </summary>
        /// <param name="tableau1">Le premier tableau</param>
        /// <param name="tableau2">Le second tableau</param>
        /// <returns>La fusion des deux tableaux</returns>
        public static Byte[] FusionTableau(Byte[] tableau1, Byte[] tableau2)
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
        /// Permet de récupérer une partie d'un tableau à partie de l'indice indice (inclu) jusqu'à la fin du tableau.
        /// </summary>
        /// <param name="tableau">Le tableau initial</param>
        /// <param name="indice">L'indice de départ</param>
        /// <returns></returns>
        public static Byte[] RecupererTableau(Byte[] tableau, int indice)
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
        /// Envoie le message au serveur avec son l'indentifiant/code
        /// /!\ Attention : Les arguments sont séparés par des virgules.
        /// </summary>
        /// <param name="code">Le code/identifiant</param>
        /// <param name="message">Le message à envoyer</param>
        public static void SendToServer(int code, string message)
        {
            Byte[] codeByte = { (Byte)code };
            Byte[] messageByte = Encoding.UTF8.GetBytes(message);
            Program.client.Send(Outils.FusionTableau(codeByte, messageByte), messageByte.Length + 1);
        }


    }
}
