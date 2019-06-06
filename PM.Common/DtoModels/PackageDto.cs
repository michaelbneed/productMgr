using System;
using System.Collections.Generic;
using System.Text;

namespace PM.Common.DtoModels
{
	class PackageDto
	{
		public int PackageId { get; set; }

		public decimal PackageName { get; set; }

		public decimal PackagePrice { get; set; }

		public decimal PackageSize { get; set; }

		public int ProductId { get; set; }

		public DateTime CreatedOn { get; set; }

		public string CreatedBy { get; set; }

		public DateTime UpdatedOn { get; set; }

		public string UpdatedBy { get; set; }
	}
}
