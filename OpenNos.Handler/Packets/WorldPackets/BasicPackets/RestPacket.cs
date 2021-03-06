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

using System;
using System.Collections.Generic;
using OpenNos.Core.Serializing;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.Packets.WorldPackets.BasicPackets
{
    [PacketHeader("rest")]
    public class RestPacket
    {
        #region Properties

        public byte Amount { get; set; }

        public List<Tuple<byte, long>> Users { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            string[] packetSplit = packet.Split(' ');
            if (packetSplit.Length < 4)
            {
                return;
            }
            RestPacket packetDefinition = new RestPacket();
            if (byte.TryParse(packetSplit[2], out byte amount))
            {
                packetDefinition.Users = new List<Tuple<byte, long>>();
                for (int i = 3; i < packetSplit.Length - 1; i += 2)
                {
                    if (byte.TryParse(packetSplit[i], out byte userType) && long.TryParse(packetSplit[i + 1], out long userId))
                    {
                        packetDefinition.Users.Add(new Tuple<byte, long>(userType, userId));
                    }
                }
                packetDefinition.Amount = amount;
                packetDefinition.ExecuteHandler(session as ClientSession);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(RestPacket), HandlePacket);

        private void ExecuteHandler(ClientSession session)
        {
            session.Character.IsAfk = false;
            if (session.Character.MeditationDictionary.Count != 0)
            {
                session.Character.MeditationDictionary.Clear();
            }
            foreach (Tuple<byte, long> user in Users)
            {
                if (user.Item1 == 1)
                {
                    session.Character.Rest();
                }
                else
                {
                    session.CurrentMapInstance.Broadcast(session.Character.Mates
                        .Find(s => s.MateTransportId == (int)user.Item2)?.GenerateRest());
                }
            }
        }

        #endregion
    }
}