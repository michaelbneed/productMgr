using System;
using System.Collections.Generic;
using System.Text;

namespace PM.Common.DtoModels
{
	public class VendorDto
	{
		public int SupplierId { get; set; }

		public int UserId { get; set; }

		public string SupplierName { get; set; }

		public string SupplierAddress { get; set; }

		public string SupplierCity { get; set; }

		public string SupplierState { get; set; }

		public int SupplierZip { get; set; }

		public string Notes { get; set; }
	}
}
