namespace PM.DatabaseOperations.DtoModels
{
	public class ProductDto
	{
		public int ProductId { get; set; }

		public string UpcCode { get; set; }

		public string ProductName { get; set; }

		public string ProductDesc { get; set; }

		public string ProductLocation { get; set; }

		public decimal ProductCost { get; set; }

		public decimal ProductPrice { get; set; }
	}
}
