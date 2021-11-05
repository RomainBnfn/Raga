using ServeurBiblio;
using ServeurConsole;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServeurConsole
{
    public class Program
    {
        public const int tailleEcranX = 5000; // Pour les limites lors de la création
        public const int tailleEcranY = 5000;  // d'une boule / joueur
        public const int nbBoulesMax = 250;  // d'une boule / joueur

        static bool accessible = true;
        static bool isFreeze = true;

        static public UdpClient serveur;
        static public Random random;
        static int port;

        static public ConsoleColor[] Couleurs = { ConsoleColor.White, ConsoleColor.Blue, ConsoleColor.DarkRed, ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Magenta };

        //
        //  - - U T I L I T A I R E S   P R I N C I P A U X  - -
        //

        /// <summary>
        /// Initialise toutes les listes et lance le thread de la boucle de réception de messages.
        /// Ensuite, lance la console de commande.
        /// </summary>
        static void Main()
        {
            // Création du serveur
            // Port d'écoute : 12345
            port = 12345;
            serveur = new UdpClient(port);
            //
            JoueurS.InitialiserListe();
            BouleS.InitialiserListe();
            random = new Random();
            //
            BouleS.AjouterBoule(nbBoulesMax);
            //
            Console.Write("- - - [");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write("Console : Serveur");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("] - - -\n");

            Thread ecoute = new Thread(BoucleEcoute);
            ecoute.Start();
            Thread areTheyHere = new Thread(BoucleClientActifs);
            areTheyHere.Start();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(">>>");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" Serveur Lancé !\n");

            //
            //  Console, entrée de commandes :
            //
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(">>>");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" En attente d'une commande. Utilisez \"help\" pour avoir la liste des commandes.\n");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write("<<< ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                string[] commande = Console.ReadLine().Split(' ');
                switch (commande[0])
                {
                    case "help":
                        #region help
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(">>>");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" Notice :\n");
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(">>>");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("  - \"");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("lock\"");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(": Pour vérouiller l'accès au serveur.\n");
                        }
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(">>>");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("  - \"");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("unlock\"");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(": Pour dévérouiller l'accès au serveur.\n");
                        }
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(">>>");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("  - \"");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("stop\"");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(": Pour arrêter le serveur.\n");
                        }
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(">>>");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("  - \"");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("list\"");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(": Pour avoir la liste des joueurs actuellement en partie.\n");
                        }
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(">>>");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("  - \"");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("kick <joueur>\"");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(": Pour déconnecter un joueur en ligne.\n");
                        }
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(">>>");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("  - \"");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("freeze\"");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(": Pour geler tous les joueurs.\n");
                        }
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(">>>");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("  - \"");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("unfreeze\"");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(": Pour dé-geler tous les joueurs.\n");
                        }

                        break;
                    #endregion

                    case "lock":
                        #region lock
                        if (!accessible)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(">>>");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(" Le serveur est déjà vérouillé ! Pour l'ouvrir, utilisez la commande \"unlock\".\n");
                            break;
                        }
                        accessible = false;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(">>>");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(" Le serveur est désormais vérrouillé, aucun joueur ne peut se connecter. (unlock pour le réouvrir)\n");
                        break;
                    #endregion

                    case "unlock":
                        #region unlock
                        if (accessible)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(">>>");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(" Le serveur est déjà accessible ! Pour le vérouiller, utilisez la commande \"lock\".\n");
                            break;
                        }
                        accessible = true;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(">>>");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(" Le serveur est désormais accessible. (lock pour le vérrouiller)\n");
                        break;
                    #endregion

                    case "stop":
                        #region stop
                        KillAll();
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(">>>");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(" Le serveur s'est arrêté. Tous les joueurs ont été déconnectés.\n");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(">>>");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(" Appuyez sur une touche pour terminer le programme.\n");
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("<<< ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Read();
                        Environment.Exit(0);
                        break;
                    #endregion

                    case "list":
                        #region list
                        if (JoueurS.GetListe().ToArray().Length == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(">>>");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(" Aucun joueur n'est connecté...\n");
                            break ;
                        }
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(">>>");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" La liste des joueurs est :\n");
                        foreach (JoueurS j in JoueurS.GetListe())
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(">>> ");
                            Console.ForegroundColor = Couleurs[j.GetColor()];
                            Console.WriteLine(j.GetPseudo());
                        }
                        break;
                    #endregion

                    case "kick":
                        #region kick
                        if (commande.Length == 1)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(">>>");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(" La commande correcte est \"kick <nom joueur>\"\n");
                            continue;
                        }
                        JoueurS joueurToKick = JoueurS.GetJoueur(commande[1]);
                        if (joueurToKick == null)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(">>>");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(" Le joueur n'existe pas ou n'est pas connecté. La liste des joueurs est disponible avec \"list\"'\n");
                            continue;
                        }
                        SendKillMessage(joueurToKick.GetIp());
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(">>>");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(" Le joueur a bien été kick.\n");
                        break;
                    #endregion

                    case "freeze":
                        #region freeze
                        if (isFreeze)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(">>>");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(" Le serveur est déjà freeze ! Pour l'unfreeze, utilisez la commande \"unfreeze\".\n");
                            continue;
                        }
                        isFreeze = true;
                        SendFreezeMessage(true);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(">>>");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(" Les joueurs ont bien été freeze.\n");
                        break;
                    #endregion

                    case "unfreeze":
                        #region unfreeze
                        if (!isFreeze)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(">>>");
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(" Le serveur n'est pas freeze ! Pour le freeze, utilisez la commande \"freeze\".\n");
                            continue;
                        }
                        isFreeze = false;
                        SendFreezeMessage(false);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(">>>");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write(" Les joueurs ont bien été unfreeze.\n");
                        break;
                    #endregion

                    default:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write(">>>");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" Commande inconue, entrez \"help\" pour avoir la notice.\n");
                        break;
                }
            }
        }


        /// <summary>
        /// Thread d'écoute. Boucle infinie pour receptionner les messages.
        /// </summary>
        static public void BoucleEcoute()
        {
            Byte[] messageRecu;
            while (true)
            {
                try
                {
                    IPEndPoint ipConnect = new IPEndPoint(IPAddress.Any, port);
                    messageRecu = serveur.Receive(ref ipConnect);
                    TraiterMessage(messageRecu, ipConnect);
                }
                catch (Exception e)
                {

                }
            }
        }

        /// <summary>
        /// Supprime les clients qui se sont déconnectés
        /// </summary>
        static public void BoucleClientActifs()
        {
            while (true)
            {
                List<JoueurS> ARetirer = new List<JoueurS>();
                foreach (JoueurS joueur in JoueurS.GetListe())
                {
                    if (joueur.isHere())
                    {
                        joueur.removeIAmHereNumber();
                    }
                    else
                    {
                        Byte[] code = { 4 };
                        Byte[] pseudo = Encoding.UTF8.GetBytes(joueur.GetPseudo());
                        Outils.SendAll(Outils.FusionTableau(code, pseudo));
                        ARetirer.Add(joueur);
                    }
                }
                foreach (JoueurS joueur in ARetirer)
                {
                    JoueurS.RemoveJoueur(joueur);
                    SendKillMessage(joueur.GetIp());
                }
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// A effectuer lors d'une réception d'un message.
        /// Cette fonction réagit en fonction du message reçu.
        /// </summary>
        /// <param name="messageRecu">Le message reçu</param>
        /// <param name="ipExpediteur">L'Ip de l'expéditeur</param>
        static public void TraiterMessage(Byte[] messageRecu, IPEndPoint ipExpediteur)
        {
            JoueurS joueur;
            string pseudoString;
            string messageString;
            int position;
            int start;
            int x;
            int y;
            //
            switch (messageRecu[0])
            {
                // Envoie d'un I Am Here (Le client est toujours en ligne)
                case 0:
                    joueur = JoueurS.GetJoueur(ipExpediteur);
                    if (joueur != null) joueur.SetIAmHereNumber();
                    break;

                // Un joueur s'est dépacé
                // Format : [1][string: x,y "...[,]...[,]"]
                case 1:
                    joueur = JoueurS.GetJoueur(ipExpediteur);

                    if (joueur == null)
                    {
                        // Au cas où un joueur n'est pas connecté mais a un client qui peut envoyer des messages (i.e. : le serveur se ferme sans un "stop")
                        SendKillMessage(ipExpediteur);
                        break;
                    }
                    if (isFreeze)
                    {
                        // Au cas où un joueur n'a pas reçu le message de freeze
                        SendFreezeMessage(true, ipExpediteur);
                        break;
                    }
                    messageString = Encoding.UTF8.GetString(Outils.RecupererTableau(messageRecu, 1));
                    pseudoString = joueur.GetPseudo();


                    // Trouver le x, y depuis le message String
                    {
                        start = 0;
                        position = messageString.IndexOf(',', start);
                        x = Int32.Parse(messageString.Substring(start, position));
                        start = position + 1;
                        position = messageString.IndexOf(',', start);
                        y = Int32.Parse(messageString.Substring(start, position - start));
                    }
                    joueur.SetPosition(x, y);

                    Byte[] code = { 1 };
                    messageString = string.Format("{0},{1},{2},", x, y, pseudoString);
                    Byte[] message = Encoding.UTF8.GetBytes(messageString);

                    // Message envoye : [1][string: x,y,pseudo "...[,]...[,]...[,]"]
                    Outils.SendAllExecpt(Outils.FusionTableau(code, message), ipExpediteur);

                    break;

                // Un joueur grossi
                case 2:
                    #region Un joueur a grossi
                    joueur = JoueurS.GetJoueur(ipExpediteur);
                    if (joueur == null)
                    {
                        // Au cas où un joueur n'est pas connecté mais a un client qui peut envoyer des messages (i.e. : le serveur se ferme sans un "stop")
                        SendKillMessage(ipExpediteur);
                        break;
                    }
                    int taille = Int32.Parse(Encoding.UTF8.GetString(Outils.RecupererTableau(messageRecu, 1)));
                    joueur.SetRayon(taille);

                    // Message envoye : [Code][string: taille,pseudo "...[,]...[,]"]
                    Byte[] code2 = { 2 };
                    messageString = string.Format("{0},{1},", taille, joueur.GetPseudo());
                    message = Encoding.UTF8.GetBytes(messageString);
                    //
                    Outils.SendAllExecpt(Outils.FusionTableau(code2, message), ipExpediteur);

                    joueur.SetRayon(messageRecu[1]);
                    break;
                #endregion

                // Un joueur a mangé une boule
                case 3:
                    Outils.SendAllExecpt(messageRecu, ipExpediteur);

                    int idBoule = Int32.Parse(Encoding.UTF8.GetString(Outils.RecupererTableau(messageRecu, 1)));
                    BouleS.RemoveBoule(BouleS.GetBoule(idBoule));

                    BouleS.AjouterBoule(); //Pour avoir le même nombre de boule sur le terrain.
                    break;

                // Un joueur a mangé un joueur
                case 4:
                    Outils.SendAllExecpt(messageRecu, ipExpediteur);

                    pseudoString = Encoding.UTF8.GetString(Outils.RecupererTableau(messageRecu, 1));
                    JoueurS.RemoveJoueur(JoueurS.GetJoueur(pseudoString));
                    break;

                // Le case 5 est réservé pour la création d'une nouvelle boule
                // (Pour ne pas se mélanger on ne vas pas l'utiliser en reception
                // non plus)


                // Demande de connexion
                case 255:
                    if (!accessible)
                    {
                        serveur.Send(new byte[] { 255, 1 }, 2, ipExpediteur);
                        return;
                    }
                    pseudoString = Encoding.UTF8.GetString(Outils.RecupererTableau(messageRecu, 2));

                    if (JoueurS.ExisteJoueur(pseudoString))
                    {
                        serveur.Send(new byte[] { 255, 2 }, 2, ipExpediteur);
                        return;
                    }
                    serveur.Send(new byte[] { 255, 0 }, 2, ipExpediteur);

                    Byte idColor = messageRecu[1];
                    x = random.Next(5, tailleEcranX - 5);
                    y = random.Next(5, tailleEcranY - 5);

                    JoueurS.AddNouveauJoueur(ipExpediteur, pseudoString, x, y, idColor);

                    if (isFreeze)
                    {
                        SendFreezeMessage(true, ipExpediteur);
                    }
                    SendFreezeMessage(false, ipExpediteur);

                    break;

                // Erreur
                default:
                    Console.WriteLine("Un message incompréhensible a été reçu.");
                    return;
            }
        }

        /// <summary>
        /// Envoie le message de kill à tous les joueurs connectés.
        /// </summary>
        static public void KillAll()
        {
            Byte[] Kill = { 254 };
            Outils.SendAll(Kill);
        }

        /// <summary>
        /// Envoie le message de kill à une ip en particulier.
        /// </summary>
        /// <param name="ip">L'ip à kill.</param>
        static public void SendKillMessage(IPEndPoint ip)
        {
            Byte[] Kill = { 254 };
            serveur.Send(Kill, 1, ip);
        }

        /// <summary>
        /// Envoyer le message de freeze ou d'unfreeze à tous les joueurs actuels et les prochains
        /// </summary>
        /// <param name="value">Si le client sera freeze</param>
        private static void SendFreezeMessage(bool value)
        {
            Byte[] Freeze = { 253, 0 };
            if (value)
            {
                Outils.SendAll(Freeze);
                return;
            }
            Freeze[1] = 1;
            Outils.SendAll(Freeze);
        }

        /// <summary>
        /// Envoyer le message de freeze ou d'unfreeze à une ip uniquement.
        /// </summary>
        /// <param name="value">Si le client sera freeze</param>
        /// <param name="ip">L'ip de destination</param>
        private static void SendFreezeMessage(bool value, IPEndPoint ip)
        {
            Byte[] Freeze = { 253, 0 };
            if (value)
            {
                serveur.Send(Freeze, Freeze.Length, ip);
                return;
            }
            Freeze[1] = 1;
            Outils.SendAll(Freeze);

        }
    }
}

