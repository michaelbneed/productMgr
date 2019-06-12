namespace PM.AzureB2C.Models.User
{
    public class SignInName
    {
        public string Type => "emailAddress";

        public string Value { get; set; }
    }
}
