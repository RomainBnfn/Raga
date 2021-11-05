using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ServeurBiblio
{
    public class JoueurS
    {
        IPEndPoint ip;
        private string pseudo;
        private double rayon;
        private double xPosition;
        private double yPosition;

        public JoueurS(string pseudo, double rayon, double x, double y)
        {
            this.pseudo = pseudo;
            this.rayon = rayon;
            xPosition = x;
            yPosition = y;

        }

        public string getPseudo()
        {
            return this.pseudo;
        }

    }
}
