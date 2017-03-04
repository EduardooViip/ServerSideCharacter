﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSideCharacter.GroupManage
{
	public class Group
	{
		public string GroupName { get; set; }
		public List<PermissionInfo> permissions = new List<PermissionInfo>();

		public Group(string name)
		{
			GroupName = name;
		}

		public bool HasPermission(string name)
		{
			if (GroupName == "spadmin") return true;
			bool hasperm = false;
			for(int i = 0;i < permissions.Count; i++)
			{
				if(permissions[i].Name == name)
				{
					hasperm = true;
					break;
				}
			}
			return hasperm;
		}
	}
}
