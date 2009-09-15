using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Inquisitor.Db
{
    [Serializable]
    public class ConnectionDetails
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Sid { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string PreScript { get; set; }
        public Color FrameColour { get; set; }

        // for XML serialisation
        public ConnectionDetails() { }

        public ConnectionDetails(string name, string host, int port, string sid, string user, string password, string preScript, Color frameColour)
        {
            Name = name;
            Host = host;
            Port = port;
            Sid = sid;
            User = user;
            Password = password;
            PreScript = preScript;
            FrameColour = frameColour;
        }

        public override bool Equals(Object o)
        {
            if (!(o is ConnectionDetails)) return false;
            ConnectionDetails cd = (ConnectionDetails)o;
            return cd.Host == Host && cd.Port == Port && cd.Sid == Sid && cd.User == User && cd.Password == Password && cd.PreScript == PreScript && cd.FrameColour == FrameColour;
        }

        public override int GetHashCode()
        {
            return Host.GetHashCode() + Port.GetHashCode() + Sid.GetHashCode() + User.GetHashCode() + FrameColour.GetHashCode() + (PreScript == null ? 0 : PreScript.GetHashCode());
        }
    }
}
