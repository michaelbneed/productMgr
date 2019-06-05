using System;
using System.Collections.Generic;
using System.Text;

namespace PM.Common.DtoModels
{
	class PackageDto
	{
		public int PackageId { get; set; }

		public decimal PackageSize { get; set; }

		public decimal PackagePrice { get; set; }

		public ProductDto Product { get; set; }

		public List<ProductDto> Products { get; set; }

		public int PackageTypeId { get; set; }

		public string OrderWeek { get; set; }
	}
}
