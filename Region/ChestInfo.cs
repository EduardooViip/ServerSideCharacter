﻿using System;
using System.Collections.Generic;
using Terraria;

namespace ServerSideCharacter.Region
{
	public class ChestManager
	{
		[Flags]
		public enum Pending
		{
			Protect = 1,
			DeProtect = 2,
			Public = 4,
			UnPublic = 8,
			AddFriend = 16,
			RemoveFriend = 32,
			Info = 64
		}
		public readonly ChestInfo[] ChestInfo = new ChestInfo[Main.chest.Length];
		private readonly Dictionary<int, Pending> _pendings = new Dictionary<int, Pending>();
		private readonly Dictionary<int, int> _friendPendings = new Dictionary<int, int>();
		public ChestManager Initialize()
		{
			for (int i = 0; i < Main.chest.Length; i++)
			{
				ChestInfo[i] = new ChestInfo();
			}
			return this;
		}
		private void SetFriendP(ServerPlayer player, ServerPlayer friend)
		{
			if (_friendPendings.ContainsKey(player.UUID))
				if (friend == null)
					_friendPendings.Remove(player.UUID);
				else
					_friendPendings[player.UUID] = friend.UUID;
			else if (friend != null)
				_friendPendings.Add(player.UUID, friend.UUID);
		}
		public ServerPlayer GetFriendP(ServerPlayer player)
		{
			int uuid = _friendPendings.ContainsKey(player.UUID) ? _friendPendings[player.UUID] : -1;
			return ServerPlayer.FindPlayer(uuid);

		}
		public void AddPending(ServerPlayer player, Pending pending, ServerPlayer friend = null)
		{

			if (_pendings.ContainsKey(player.UUID))
				_pendings[player.UUID] |= pending;
			else
				_pendings.Add(player.UUID, pending);
			if (pending.HasFlag(Pending.AddFriend) || pending.HasFlag(Pending.RemoveFriend))
				SetFriendP(player, friend);
		}
		public void SetPendings(ServerPlayer player, Pending pending, ServerPlayer friend = null)
		{
			if (_pendings.ContainsKey(player.UUID))
				_pendings[player.UUID] = pending;
			else
				_pendings.Add(player.UUID, pending);
			if (pending.HasFlag(Pending.AddFriend) || pending.HasFlag(Pending.RemoveFriend))
				SetFriendP(player, friend);

		}
		public void RemovePending(ServerPlayer player, Pending pending)
		{
			if (_pendings.ContainsKey(player.UUID))
				_pendings[player.UUID] &= ~pending;
			if (pending.HasFlag(Pending.AddFriend) || pending.HasFlag(Pending.RemoveFriend))
				SetFriendP(player, null);
		}
		public void AddFriend(int chestID, ServerPlayer friend)
		{
			ChestInfo[chestID].AddFriend(friend);
		}
		public void RemoveFriend(int chestID, ServerPlayer friend)
		{
			ChestInfo[chestID].RemoveFriend(friend);
		}
		public void RemoveAllPendings(ServerPlayer player)
		{
			if (_pendings.ContainsKey(player.UUID))
				_pendings[player.UUID] = new Pending();
			SetFriendP(player, null);
		}
		public Pending GetPendings(ServerPlayer player)
		{
			return _pendings.ContainsKey(player.UUID) ? _pendings[player.UUID] : new Pending();

		}
		public void SetOwner(int chestID, int ownerID, bool isPublic)
		{
			ChestInfo[chestID].OwnerID = ownerID;
			ChestInfo[chestID].IsPublic = isPublic;
		}

		public bool IsNull(int chestID)
		{
			var id = ChestInfo[chestID].OwnerID;
			return id == -1;
		}
		public bool IsOwner(int chestID, ServerPlayer player)
		{
			var id = ChestInfo[chestID].OwnerID;
			return id == player.UUID || player.PermissionGroup.HasPermission("chest") && id != -1;
		}
		public bool IsPublic(int chestID)
		{
			var isPublic = ChestInfo[chestID].IsPublic;
			return isPublic;
		}
		public bool CanOpen(int chestID, ServerPlayer player)
		{
			var id = ChestInfo[chestID].OwnerID;
			var isPublic = ChestInfo[chestID].IsPublic;
			var friends = ChestInfo[chestID].Friends;
			return id == -1 || id == player.UUID || player.PermissionGroup.HasPermission("chest") || isPublic || friends.Contains(player.UUID);
		}
	}
	public class ChestInfo
	{
		private int ownerID = -1;
		private bool isPublic = false;
		private List<int> friends = new List<int>();
		public ChestInfo()
		{

		}
		public void AddFriend(ServerPlayer player)
		{
			if (!friends.Contains(player.UUID) && ownerID > -1 && player.UUID != ownerID)
				friends.Add(player.UUID);
		}
		public void RemoveFriend(ServerPlayer player)
		{
			if (friends.Contains(player.UUID) && ownerID > -1)
				friends.RemoveAll(id => id == player.UUID);
		}
		public int OwnerID
		{
			get { return ownerID; }
			set
			{
				if (value <= -1)
				{
					isPublic = false;
					friends.Clear();
					value = -1; //Just in case if value is < -1
				}
				ownerID = value;
			}
		}
		public bool IsPublic
		{
			get { return isPublic; }
			set { isPublic = value; }
		}
		public List<int> Friends
		{
			get { return friends; }
		}
	}
}
