using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace PM.Common.EntityModels
{
	public class PmUser
	{
		public string Name { get; set; }

		public string Email { get; set; }
		
		public string Token { get; set; }

		public DateTime TokenExpires { get; set; }

		[NotMapped]
		public IList<Claim> Claims { get; set; } = new List<Claim>();
	}
}
