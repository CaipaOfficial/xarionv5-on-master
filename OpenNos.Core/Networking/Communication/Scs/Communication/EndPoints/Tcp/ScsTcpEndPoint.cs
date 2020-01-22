﻿// This file is part of the OpenNos NosTale Emulator Project.
// 
// This program is licensed under a deviated version of the Fair Source License,
// granting you a non-exclusive, non-transferable, royalty-free and fully-paid-up
// license, under all of the Licensor's copyright and patent rights, to use, copy, prepare
// derivative works of, publicly perform and display the Software, subject to the
// conditions found in the LICENSE file.
// 
// THIS FILE IS PROVIDED "AS IS", WITHOUT WARRANTY OR
// CONDITION, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. THE AUTHORS HEREBY DISCLAIM ALL LIABILITY, WHETHER IN
// AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE.
using OpenNos.Core.Networking.Communication.Scs.Client;
using OpenNos.Core.Networking.Communication.Scs.Client.Tcp;
using OpenNos.Core.Networking.Communication.Scs.Server;
using OpenNos.Core.Networking.Communication.Scs.Server.Tcp;
using System;
using System.Net;
using System.Net.Sockets;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace OpenNos.Core.Networking.Communication.Scs.Communication.EndPoints.Tcp
{
    /// <summary>
    /// Represents a TCP end point in SCS.
    /// </summary>
    public class ScsTcpEndPoint : ScsEndPoint
    {
        #region Members

        private SocketInformation? _existingSocketInformation;

        #endregion

        #region Instantiation

        /// <summary>
        /// Creates a new ScsTcpEndPoint object with specified port number.
        /// </summary>
        /// <param name="tcpPort">Listening TCP Port for incoming connection requests on server</param>
        public ScsTcpEndPoint(int tcpPort) => TcpPort = tcpPort;

        public ScsTcpEndPoint()
        {
        }

        /// <summary>
        /// Creates a new ScsTcpEndPoint object with specified IP address and port number.
        /// </summary>
        /// <param name="ipAddress">IP address of the server</param>
        /// <param name="port">Listening TCP Port for incoming connection requests on server</param>
        /// <param name="socketInformation">The existing socket information.</param>
        public ScsTcpEndPoint(IPAddress ipAddress, int port, SocketInformation? socketInformation = null)
        {
            IpAddress = ipAddress;
            TcpPort = port;
            _existingSocketInformation = socketInformation;
        }

        /// <summary>
        /// Creates a new ScsTcpEndPoint object with specified IP address and port number.
        /// </summary>
        /// <param name="ipAddress">IP address of the server</param>
        /// <param name="port">Listening TCP Port for incoming connection requests on server</param>
        /// <param name="socketInformation">The existing socket information.</param>
        public ScsTcpEndPoint(string ipAddress, int port, SocketInformation? socketInformation = null) : this(IPAddress.Parse(ipAddress), port, socketInformation)
        {
        }

        /// <summary>
        /// Creates a new ScsTcpEndPoint from a string address. Address format must be like
        /// IPAddress:Port (For example: 127.0.0.1:10085).
        /// </summary>
        /// <param name="address">TCP end point Address</param>
        /// <returns>Created ScsTcpEndpoint object</returns>
        public ScsTcpEndPoint(string address)
        {
            string[] splittedAddress = address.Trim().Split(':');
            IpAddress = IPAddress.Parse(splittedAddress[0].Trim());
            TcpPort = Convert.ToInt32(splittedAddress[1].Trim());
        }

        #endregion

        #region Properties

        /// <summary>
        /// IP address of the server.
        /// </summary>
        public IPAddress IpAddress { get; set; }

        /// <summary>
        /// Listening TCP Port for incoming connection requests on server.
        /// </summary>
        public int TcpPort { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a Scs Client that uses this end point to connect to server.
        /// </summary>
        /// <returns>Scs Client</returns>
        public override IScsClient CreateClient()
        {
            ScsTcpClient client = new ScsTcpClient(this, _existingSocketInformation);
            _existingSocketInformation = null;
            return client;
        }

        /// <summary>
        /// Creates a Scs Server that uses this end point to listen incoming connections.
        /// </summary>
        /// <returns>Scs Server</returns>
        public override IScsServer CreateServer() => new ScsTcpServer(this);

        public override bool Equals(object obj) => ((ScsTcpEndPoint)obj)?.IpAddress == IpAddress && ((ScsTcpEndPoint)obj)?.TcpPort == TcpPort;

        public override int GetHashCode() => IpAddress.GetHashCode() + TcpPort.GetHashCode();

        /// <summary>
        /// Generates a string representation of this end point object.
        /// </summary>
        /// <returns>String representation of this end point object</returns>
        public override string ToString() => IpAddress == null ? $"tcp://{TcpPort}" : $"tcp://{IpAddress}:{TcpPort}";

        #endregion
    }
}