using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace PM.Common.DtoModels
{
	public class NoteDto
	{
		public int NoteId { get; set; }

		public string NoteText { get; set; }

		public int RequestId  { get; set; }

		public DateTime CreatedOn { get; set; }

		public string CreatedBy { get; set; }

		public DateTime UpdatedOn { get; set; }

		public string UpdatedBy { get; set; }
	}
}
