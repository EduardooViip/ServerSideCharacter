﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Terraria;
using Terraria.ID;

namespace ServerSideCharacter.ServerCommand
{
	public static class CommandBoardcast
	{ 

		public static void ShowSaveInfo()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			string info = string.Format("[SSC {0}] Saved all player data", ServerSideCharacter.Version);
			Console.WriteLine(info);
			LogInfo(info);
			Console.ResetColor();
		}

		public static void ShowSavePlayer(ServerPlayer p)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			string info = string.Format("[SSC {0}] Saved {1}'s data", ServerSideCharacter.Version, p.Name);
			Console.WriteLine(info);
			LogInfo(info);
			Console.ResetColor();
		}
		public static void ShowNormalText(string msg)
		{
			string info = string.Format("[SSC {0}] {1}", ServerSideCharacter.Version, msg);
			Console.WriteLine(info);
			LogInfo(info);
		}
		public static void ShowMessage(string msg)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			string info = string.Format("[SSC {0}] {1}", ServerSideCharacter.Version, msg);
			Console.WriteLine(info);
			LogInfo(info);
			Console.ResetColor();
		}
		public static void ShowError(Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			string info = string.Format("[SSC {0}] {1}", ServerSideCharacter.Version, ex.ToString());
			Console.WriteLine(info);
			LogInfo(info);
			Console.ResetColor();
		}
		public static void ShowError(string msg)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			string info = string.Format("[SSC {0}] {1}", ServerSideCharacter.Version, msg);
			Console.WriteLine(info);
			LogInfo(info);
			Console.ResetColor();
		}
		public static void LogInfo(string msg)
		{
			string dateTime = DateTime.Now.ToLongTimeString();
			string text = dateTime + "\n" + msg + "\n";
			ServerSideCharacter.Logger.WriteToFile(text);
		}

		public static void SendErrorToPlayer(int plr, string msg)
		{
			NetMessage.SendData(MessageID.ChatText, plr, -1,
							msg,
							255, 255, 20, 0);
		}
		public static void SendInfoToPlayer(int plr, string msg)
		{
			NetMessage.SendData(MessageID.ChatText, plr, -1,
							msg,
							255, 255, 255, 0);
		}
		public static void SendSuccessToPlayer(int plr, string msg)
		{
			NetMessage.SendData(MessageID.ChatText, plr, -1,
							msg,
							255, 50, 255, 50);
		}
		public static void SendInfoToAll(string msg)
		{
			NetMessage.SendData(MessageID.ChatText, -1, -1,
							msg,
							255, 255, 255, 0);
		}
	}
}
