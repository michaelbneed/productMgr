using System;

namespace PM.DatabaseOperations.DtoModels
{
	public class UserDto
	{
		public int UserId { get; set; }

		public string UserName { get; set; }

		public string Password { get; set; }

		public string AuthId { get; set; }

		public string EmailAddress { get; set; }

		public int SupplierId { get; set; }
		public DateTime CreatedOn { get; set; }
		public string CreatedBy { get; set; }
		public DateTime UpdatedOn { get; set; }
		public string UpdatedBy { get; set; }
	}
}
