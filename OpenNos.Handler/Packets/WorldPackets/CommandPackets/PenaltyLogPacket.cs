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

using System.Collections.Generic;
using System.Linq;
using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Data;
using OpenNos.DAL;
using OpenNos.Domain;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.Packets.WorldPackets.CommandPackets
{
    [PacketHeader("$PenaltyLog", Authority = AuthorityType.GameMaster)]
    public class PenaltyLogPacket
    {
        #region Members

        private bool _isParsed;

        #endregion

        #region Properties

        public string CharacterName { get; set; }

        #endregion

        #region Methods

        public static void HandlePacket(object session, string packet)
        {
            if (session is ClientSession sess)
            {
                string[] packetSplit = packet.Split(' ');
                if (packetSplit.Length < 3)
                {
                    sess.SendPacket(sess.Character.GenerateSay(ReturnHelp(), 10));
                    return;
                }
                PenaltyLogPacket packetDefinition = new PenaltyLogPacket();
                if (!string.IsNullOrWhiteSpace(packetSplit[2]))
                {
                    packetDefinition._isParsed = true;
                    packetDefinition.CharacterName = packetSplit[2];
                }
                packetDefinition.ExecuteHandler(sess);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(PenaltyLogPacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$PenaltyLog CHARACTERNAME";

        private void ExecuteHandler(ClientSession session)
        {
            if (_isParsed)
            {
                Logger.LogUserEvent("GMCOMMAND", session.GenerateIdentity(),
                    $"[PenaltyLog]CharacterName: {CharacterName}");

                CharacterDTO character = DAOFactory.CharacterDAO.LoadByName(CharacterName);
                if (character != null)
                {
                    bool separatorSent = false;

                    void WritePenalty(PenaltyLogDTO penalty)
                    {
                        session.SendPacket(session.Character.GenerateSay($"Type: {penalty.Penalty}", 13));
                        session.SendPacket(session.Character.GenerateSay($"AdminName: {penalty.AdminName}", 13));
                        session.SendPacket(session.Character.GenerateSay($"Reason: {penalty.Reason}", 13));
                        session.SendPacket(session.Character.GenerateSay($"DateStart: {penalty.DateStart}", 13));
                        session.SendPacket(session.Character.GenerateSay($"DateEnd: {penalty.DateEnd}", 13));
                        session.SendPacket(session.Character.GenerateSay("----- ------- -----", 13));
                        separatorSent = true;
                    }

                    IEnumerable<PenaltyLogDTO> penaltyLogs = ServerManager.Instance.PenaltyLogs
                        .Where(s => s.AccountId == character.AccountId).ToList();

                    //PenaltyLogDTO penalty = penaltyLogs.LastOrDefault(s => s.DateEnd > DateTime.UtcNow);
                    session.SendPacket(session.Character.GenerateSay("----- PENALTIES -----", 13));

                    #region Warnings

                    session.SendPacket(session.Character.GenerateSay("----- WARNINGS -----", 13));
                    foreach (PenaltyLogDTO penaltyLog in penaltyLogs.Where(s => s.Penalty == PenaltyType.Warning)
                        .OrderBy(s => s.DateStart))
                    {
                        WritePenalty(penaltyLog);
                    }

                    if (!separatorSent)
                    {
                        session.SendPacket(session.Character.GenerateSay("----- ------- -----", 13));
                    }

                    separatorSent = false;

                    #endregion

                    #region Mutes

                    session.SendPacket(session.Character.GenerateSay("----- MUTES -----", 13));
                    foreach (PenaltyLogDTO penaltyLog in penaltyLogs.Where(s => s.Penalty == PenaltyType.Muted)
                        .OrderBy(s => s.DateStart))
                    {
                        WritePenalty(penaltyLog);
                    }

                    if (!separatorSent)
                    {
                        session.SendPacket(session.Character.GenerateSay("----- ------- -----", 13));
                    }

                    separatorSent = false;

                    #endregion

                    #region Bans

                    session.SendPacket(session.Character.GenerateSay("----- BANS -----", 13));
                    foreach (PenaltyLogDTO penaltyLog in penaltyLogs.Where(s => s.Penalty == PenaltyType.Banned)
                        .OrderBy(s => s.DateStart))
                    {
                        WritePenalty(penaltyLog);
                    }

                    if (!separatorSent)
                    {
                        session.SendPacket(session.Character.GenerateSay("----- ------- -----", 13));
                    }

                    #endregion

                    session.SendPacket(session.Character.GenerateSay("----- SUMMARY -----", 13));
                    session.SendPacket(session.Character.GenerateSay(
                        $"Warnings: {penaltyLogs.Count(s => s.Penalty == PenaltyType.Warning)}", 13));
                    session.SendPacket(
                        session.Character.GenerateSay(
                            $"Mutes: {penaltyLogs.Count(s => s.Penalty == PenaltyType.Muted)}", 13));
                    session.SendPacket(
                        session.Character.GenerateSay(
                            $"Bans: {penaltyLogs.Count(s => s.Penalty == PenaltyType.Banned)}", 13));
                    session.SendPacket(session.Character.GenerateSay("----- ------- -----", 13));
                }
                else
                {
                    session.SendPacket(
                        session.Character.GenerateSay(Language.Instance.GetMessageFromKey("USER_NOT_FOUND"), 10));
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