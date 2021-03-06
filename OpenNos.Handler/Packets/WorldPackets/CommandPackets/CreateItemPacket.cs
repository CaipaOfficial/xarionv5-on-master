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

using System.Linq;
using OpenNos.Core;
using OpenNos.Core.Serializing;
using OpenNos.Domain;
using OpenNos.GameObject;
using OpenNos.GameObject.Helpers;
using OpenNos.GameObject.Networking;

namespace OpenNos.Handler.Packets.WorldPackets.CommandPackets
{
    [PacketHeader("$CreateItem", Authority = AuthorityType.GameMaster)]
    public class CreateItemPacket
    {
        #region Members

        private bool _isParsed;

        #endregion

        #region Properties

        public byte? Design { get; set; }

        public byte? Upgrade { get; set; }

        public short VNum { get; set; }

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
                CreateItemPacket packetDefinition = new CreateItemPacket();
                if (short.TryParse(packetSplit[2], out short vnum))
                {
                    packetDefinition._isParsed = true;
                    packetDefinition.VNum = vnum;
                    if (packetSplit.Length > 3 && byte.TryParse(packetSplit[3], out byte design))
                    {
                        packetDefinition.Design = design;
                    }
                    if (packetSplit.Length > 4 && byte.TryParse(packetSplit[4], out byte upgrade))
                    {
                        packetDefinition.Upgrade = upgrade;
                    }
                }
                packetDefinition.ExecuteHandler(sess);
            }
        }

        public static void Register() => PacketFacility.AddHandler(typeof(CreateItemPacket), HandlePacket, ReturnHelp);

        public static string ReturnHelp() => "$CreateItem ITEMVNUM DESIGN/RARE/AMOUNT/WINGS UPDATE";

        private void ExecuteHandler(ClientSession session)
        {
            if (_isParsed)
            {
                Logger.LogUserEvent("GMCOMMAND", session.GenerateIdentity(),
                    $"[CreateItem]ItemVNum: {VNum} Amount/Design: {Design} Upgrade: {Upgrade}");

                short vnum = VNum;
                sbyte rare = 0;
                byte upgrade = 0, amount = 1, design = 0;
                if (vnum == 1046)
                {
                    return; // cannot create gold as item, use $Gold instead
                }

                Item iteminfo = ServerManager.GetItem(vnum);
                if (iteminfo != null)
                {
                    if (iteminfo.IsColored || (iteminfo.ItemType == ItemType.Box && iteminfo.ItemSubType == 3))
                    {
                        if (Design.HasValue)
                        {
                            design = Design.Value;
                        }
                    }
                    else if (iteminfo.Type == InventoryType.Equipment)
                    {
                        if (Upgrade.HasValue)
                        {
                            upgrade = Upgrade.Value;
                        }

                        if (iteminfo.EquipmentSlot != EquipmentType.Sp && upgrade == 0
                                                                       && iteminfo.BasicUpgrade != 0)
                        {
                            upgrade = iteminfo.BasicUpgrade;
                        }

                        if (Design.HasValue)
                        {
                            if (iteminfo.EquipmentSlot == EquipmentType.Sp)
                            {
                                design = Design.Value;
                            }
                            else
                            {
                                rare = (sbyte)Design.Value;
                            }
                        }
                    }

                    if (Design.HasValue && !Upgrade.HasValue
                        && (iteminfo.Type == InventoryType.Main || iteminfo.Type == InventoryType.Etc))
                    {
                        amount = Design.Value > 99 ? (byte)99 : Design.Value;
                    }

                    ItemInstance inv = session.Character.Inventory
                        .AddNewToInventory(vnum, amount, rare: rare, upgrade: upgrade, design: design).FirstOrDefault();
                    if (inv != null)
                    {
                        ItemInstance wearable = session.Character.Inventory.LoadBySlotAndType(inv.Slot, inv.Type);
                        if (wearable != null)
                        {
                            switch (wearable.Item.EquipmentSlot)
                            {
                                case EquipmentType.Armor:
                                case EquipmentType.MainWeapon:
                                case EquipmentType.SecondaryWeapon:
                                    bool isPartner = (wearable.ItemVNum >= 990 && wearable.ItemVNum <= 992) || (wearable.ItemVNum >= 995 && wearable.ItemVNum <= 997);
                                    wearable.SetRarityPoint(isPartner);
                                    break;

                                case EquipmentType.Boots:
                                case EquipmentType.Gloves:
                                    wearable.FireResistance = (short)(wearable.Item.FireResistance * upgrade);
                                    wearable.DarkResistance = (short)(wearable.Item.DarkResistance * upgrade);
                                    wearable.LightResistance = (short)(wearable.Item.LightResistance * upgrade);
                                    wearable.WaterResistance = (short)(wearable.Item.WaterResistance * upgrade);
                                    break;
                            }
                        }

                        session.SendPacket(session.Character.GenerateSay(
                            $"{Language.Instance.GetMessageFromKey("ITEM_ACQUIRED")}: {iteminfo.Name} x {amount}", 12));
                    }
                    else
                    {
                        session.SendPacket(
                            UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NOT_ENOUGH_PLACE"),
                                0));
                    }
                }
                else
                {
                    UserInterfaceHelper.GenerateMsg(Language.Instance.GetMessageFromKey("NO_ITEM"), 0);
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