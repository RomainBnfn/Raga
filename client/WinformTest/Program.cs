using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformTest
{
    static class Program
    {
        static public bool connexion = false;
        static public UdpClient client;

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Création de l'UdpClient qui sera responsable de la communication avec le serveur
            client = new UdpClient(0);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new DemandeConnexion());
            if (connexion)
            {
                Application.Run(new FenetreJeu());
            }
        }
    }
}
