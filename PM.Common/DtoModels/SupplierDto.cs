﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PM.Common.DtoModels
{
	public class SupplierDto
	{
		public int SupplierId { get; set; }

		public string SupplierName { get; set; }

		public string SupplierEmail { get; set; }

		public DateTime CreatedOn { get; set; }

		public string CreatedBy { get; set; }

		public DateTime UpdatedOn { get; set; }

		public string UpdatedBy { get; set; }
	}
}
