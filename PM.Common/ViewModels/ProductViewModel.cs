using System;
using System.Collections.Generic;
using System.Text;
using PM.Common.DtoModels;

namespace PM.Common.ViewModels
{
	class ProductViewModel
	{
		public List<ProductDto> ProductList { get; set; }
		public ProductDto Product { get; set; }

		// TODO - specific data for UI
	}
}
