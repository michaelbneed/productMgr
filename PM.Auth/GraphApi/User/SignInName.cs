namespace PM.Auth.GraphApi.User
{
    public class SignInName
    {
        public string Type => "emailAddress";

        public string Value { get; set; }
    }
}
