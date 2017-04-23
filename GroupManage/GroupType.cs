﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSideCharacter.GroupManage
{
	public static class GroupType
	{

		public static Dictionary<string, Group> Groups = new Dictionary<string, Group>();
		
		private static void AddToGroup(Group g)
		{
			Groups.Add(g.GroupName, g);
		}

		internal static void SetupGroups()
		{
			Group crminalGroup = new Group("criminal")
			{
				ChatColor = Color.Gray,
				ChatPrefix = "Criminal"
			};
			AddToGroup(crminalGroup);

			Group defaultGroup = new Group("default")
			{
				ChatPrefix = "Default"
			};
			defaultGroup.permissions.Add(new PermissionInfo("tp", "Teleport player"));
			defaultGroup.permissions.Add(new PermissionInfo("ls", "List online player's info"));
			defaultGroup.permissions.Add(new PermissionInfo("auth", "Authorize as super admin"));
			AddToGroup(defaultGroup);


			Group admin = new Group("admin")
			{
				ChatColor = Color.Red,
				ChatPrefix = "Admin",
				permissions = new List<PermissionInfo>(defaultGroup.permissions)
				{
					new PermissionInfo("time", "Changing times"),
					new PermissionInfo("butcher", "Kill all monsters"),
					new PermissionInfo("ls -al", "List all player's info"),
					new PermissionInfo("lock", "Lock a player"),
					new PermissionInfo("sm", "Summon monsters"),
					new PermissionInfo("tphere", "Force teleport a player to your place"),
					new PermissionInfo("region", "Manage regions"),
					new PermissionInfo("region-create", "Create region"),
					new PermissionInfo("region-remove", "Remove regions"),
					new PermissionInfo("expert", "toggle expert"),
					new PermissionInfo("hardmode", "toggle hardmode"),
					new PermissionInfo("region-share", "Share regions"),
					new PermissionInfo("ban-item", "Ban certain item")
				}
			};
			AddToGroup(admin);


			Group superAdmin = new Group("spadmin")
			{
				ChatColor = Color.Cyan,
				ChatPrefix = "Super Admin",
			};
			superAdmin.permissions.Add(new PermissionInfo("all", "all commands"));
			AddToGroup(superAdmin);
		}
	}
}
