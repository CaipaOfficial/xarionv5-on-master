// This file is part of the OpenNos NosTale Emulator Project.
//
// This program is licensed under a deviated version of the Fair Source License, granting you a
// non-exclusive, non-transferable, royalty-free and fully-paid-up license, under all of the
// Licensor's copyright and patent rights, to use, copy, prepare derivative works of, publicly
// perform and display the Software, subject to the conditions found in the LICENSE file.
//
// THIS FILE IS PROVIDED "AS IS", WITHOUT WARRANTY OR CONDITION, EXPRESS OR IMPLIED, INCLUDING BUT
// NOT LIMITED TO WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. THE AUTHORS HEREBY DISCLAIM ALL LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT
// OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE.

using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.Packets.WorldPackets.CommandPackets
{
    [PacketHeader("$SetPerfection", Authority = AuthorityType.GameMaster)]
    public class SetPerfectionPacket
    {
        #region Members

        private bool _isParsed;

        #endregion

        #region Properties

        public short Slot { get; set; }

        public byte Type { get; set; }

        public byte Value { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            if (session is ClientSession sess)
            {
                string[] packetSplit = packet.Split(' ');
                if (packetSplit.Length < 5)
                {
                    sess.SendPacket(sess.Character.GenerateSay(ReturnHelp(), 10));
                    return;
                }
                SetPerfectionPacket packetDefinition = new SetPerfectionPacket();
                if (short.TryParse(packetSplit[2], out short slot) && byte.TryParse(packetSplit[3], out byte type)
                    && byte.TryParse(packetSplit[4], out byte value))
                {
                    packetDefinition._isParsed = true;
                    packetDefinition.Slot = slot;
                    packetDefinition.Type = type;
                    packetDefinition.Value = value;
                }

                packetDefinition.ExecuteHandler(sess);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(SetPerfectionPacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$SetPerfection SLOT TYPE VALUE";

        private void ExecuteHandler(ClientSession session)
        {
            if (_isParsed)
            {
                Logger.LogUserEvent("GMCOMMAND", session.GenerateIdentity(),
                    $"[SetPerfection]Slot: {Slot} Type: {Type} Value: {Value}");

                if (Slot >= 0)
                {
                    ItemInstance specialistInstance =
                        session.Character.Inventory.LoadBySlotAndType(Slot, 0);

                    if (specialistInstance != null)
                    {
                        switch (Type)
                        {
                            case 0:
                                specialistInstance.SpStoneUpgrade = Value;
                                break;

                            case 1:
                                specialistInstance.SpDamage = Value;
                                break;

                            case 2:
                                specialistInstance.SpDefence = Value;
                                break;

                            case 3:
                                specialistInstance.SpElement = Value;
                                break;

                            case 4:
                                specialistInstance.SpHP = Value;
                                break;

                            case 5:
                                specialistInstance.SpFire = Value;
                                break;

                            case 6:
                                specialistInstance.SpWater = Value;
                                break;

                            case 7:
                                specialistInstance.SpLight = Value;
                                break;

                            case 8:
                                specialistInstance.SpDark = Value;
                                break;

                            default:
                                session.SendPacket(session.Character.GenerateSay(ReturnHelp(),
                                    10));
                                break;
                        }
                    }
                    else
                    {
                        session.SendPacket(session.Character.GenerateSay(ReturnHelp(), 10));
                    }
                }
            }
            else
            {
                session.SendPacket(session.Character.GenerateSay(ReturnHelp(), 10));
            }
        }

        #endregion
    }
}