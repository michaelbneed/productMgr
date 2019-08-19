using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace PM.Business.Dto
{
	public static class UserDto
	{
		public static string Role { get; set; }

		public static string UserId { get; set; }

		public static void SetUserRole(string userGroupId, IConfiguration configuration)
		{
			var securityGroups = configuration.GetSection("SecurityGroups").GetChildren()
				.Select(item => new KeyValuePair<string, string>(item.Key, item.Value))
				.ToDictionary(x => x.Key, x => x.Value);

			foreach (var securityGroup in securityGroups)
			{
				if (userGroupId == securityGroup.Value)
				{
					Role = securityGroup.Key;
				}
			}
		}
	}
}
