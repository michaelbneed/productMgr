using System.Collections.Generic;
using PM.DatabaseOperations.DtoModels;

namespace PM.DatabaseOperations.ViewModels
{
	class ProductViewModel
	{
		public List<ProductDto> ProductList { get; set; }
		public ProductDto Product { get; set; }

		// TODO - specific data for UI
	}
}
