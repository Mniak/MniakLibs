﻿using Mniak.Network.Sockets;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace Mniak.Network.Sockets
{
    public class SecureSocketFactory
    {
        public static SecureSocket CreateUnsecureSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            return new SecureSocket(false, addressFamily, socketType, protocolType);
        }
        public static SecureSocket CreateSecureSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType, X509Certificate2 certificate)
        {
            return new SecureSocket(true, addressFamily, socketType, protocolType)
            {
                clientCertificate = certificate,
                serverCertificate = certificate
            };
        }

    }
}
