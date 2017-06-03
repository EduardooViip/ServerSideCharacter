﻿using ServerSideCharacter.GroupManage;
using ServerSideCharacter.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;

namespace ServerSideCharacter
{
	public class PlayerData
	{
		public Dictionary<string, ServerPlayer> Data = new Dictionary<string, ServerPlayer>();

		public PlayerData()
		{

		}

		private static string ReadNext(XmlNodeList info, ref int i)
		{
			var xmlNode = info.Item(i++);
			if (xmlNode != null) return xmlNode.InnerText;
			throw new NullReferenceException("XMLNode is null");
		}


		private void TryReadItemInfo(Dictionary<string, Mod> modTable, XmlNodeList info,
			ServerPlayer player, ref int i, int id, ref Item[] slots)
		{
			int type;
			string text = ReadNext(info, ref i);
			//如果是mod物品
			if (text[0] == '$')
			{
				text = text.Substring(1);

				string modName = text.Substring(0, text.IndexOf('.'));
				string itemName = text.Substring(text.LastIndexOf('.') + 1);
				//解析物品id，字典中有mod名字
				if (modTable.ContainsKey(modName))
				{
					type = modTable[modName].ItemType(itemName);
					//如果数据合法
					if (type > 0)
					{
						slots[id].netDefaults(type);
						foreach (var pair in ModDataHooks.ItemExtraInfoTable)
						{
							ModDataHooks.InterpretStringTable[pair.Key].Invoke(
								((info.Item(i - 1) as XmlElement).GetAttribute(pair.Key)),
								slots[id]);
						}
					}
					else
					{
						slots[id].netDefaults(ServerSideCharacter.Instance.ItemType("TestItem"));
						((TestItem)slots[id].modItem).SetUp(text);
						//物品数据会丢失
					}
				}
				else
				{
					slots[id].netDefaults(ServerSideCharacter.Instance.ItemType("TestItem"));
					((TestItem)slots[id].modItem).SetUp(text);
					//物品数据会丢失

				}
			}
			else
			{
				type = Convert.ToInt32(text);

				if (type != 0)
				{
					slots[id].netDefaults(type);
				}
				foreach (var pair in ModDataHooks.ItemExtraInfoTable)
				{
					ModDataHooks.InterpretStringTable[pair.Key].Invoke(
						((info.Item(i - 1) as XmlElement).GetAttribute(pair.Key)),
						slots[id]);
				}
			}
		}

		public PlayerData(string path)
		{
			ServerPlayer.ResetUUID();
			if (File.Exists(path))
			{
				XmlReaderSettings settings = new XmlReaderSettings { IgnoreComments = true };
				//忽略文档里面的注释
				XmlDocument xmlDoc = new XmlDocument();
				XmlReader reader = XmlReader.Create(path, settings);
				xmlDoc.Load(reader);
				XmlNode xn = xmlDoc.SelectSingleNode("Players");
				var list = xn.ChildNodes;
				Dictionary<string, Mod> modTable = new Dictionary<string, Mod>();
				foreach (var mod in ModLoader.LoadedMods)
				{
					modTable.Add(mod.Name, mod);
				}
				foreach (var node in list)
				{
					XmlElement pData = (XmlElement)node;
					ServerPlayer player = new ServerPlayer();
					var info = pData.ChildNodes;
					int i = 0;
					player.Name = pData.GetAttribute("name");
					player.UUID = int.Parse(pData.GetAttribute("uuid"));
					try
					{
						player.PermissionGroup = ServerSideCharacter.GroupManager.Groups[pData.GetAttribute("group")];
					}
					catch
					{
						player.PermissionGroup = ServerSideCharacter.GroupManager.Groups["default"];
					}
					foreach (var pair in ModDataHooks.InterpretPlayerStringTable)
					{
						pair.Value(pData.GetAttribute(pair.Key), player);
					}
					player.HasPassword = Convert.ToBoolean(ReadNext(info, ref i));
					player.Password = ReadNext(info, ref i);
					player.LifeMax = Convert.ToInt32(ReadNext(info, ref i));
					player.StatLife = Convert.ToInt32(ReadNext(info, ref i));
					player.ManaMax = Convert.ToInt32(ReadNext(info, ref i));
					player.StatMana = Convert.ToInt32(ReadNext(info, ref i));
					for (int id = 0; id < player.inventory.Length; id++)
					{
						TryReadItemInfo(modTable, info, player, ref i, id, ref player.inventory);
						//player.inventory[id].Prefix(Convert.ToByte((info.Item(i - 1) as XmlElement).GetAttribute("prefix")));
						//player.inventory[id].stack =
						//	Convert.ToInt32((info.Item(i - 1) as XmlElement).GetAttribute("stack"));
					}
					for (int id = 0; id < player.armor.Length; id++)
					{
						TryReadItemInfo(modTable, info, player, ref i, id, ref player.armor);
					}
					for (int id = 0; id < player.dye.Length; id++)
					{
						TryReadItemInfo(modTable, info, player, ref i, id, ref player.dye);
					}
					for (int id = 0; id < player.miscEquips.Length; id++)
					{
						TryReadItemInfo(modTable, info, player, ref i, id, ref player.miscEquips);
					}
					for (int id = 0; id < player.miscDye.Length; id++)
					{
						TryReadItemInfo(modTable, info, player, ref i, id, ref player.miscDye);
					}
					for (int id = 0; id < player.bank.item.Length; id++)
					{
						TryReadItemInfo(modTable, info, player, ref i, id, ref player.bank.item);
					}
					for (int id = 0; id < player.bank2.item.Length; id++)
					{
						TryReadItemInfo(modTable, info, player, ref i, id, ref player.bank2.item);
					}
					for (int id = 0; id < player.bank3.item.Length; id++)
					{
						TryReadItemInfo(modTable, info, player, ref i, id, ref player.bank3.item);
					}

					Data.Add(player.Name, player);
					ServerPlayer.IncreaseUUID();
				}
				ServerSideCharacter.MainWriter = new XMLWriter(path);
				reader.Close();
			}
			else
			{
				XMLWriter writer = new XMLWriter(path);
				writer.Create();
				ServerSideCharacter.MainWriter = writer;
			}
		}
	}
}
