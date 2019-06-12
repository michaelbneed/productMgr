namespace PM.Auth.GraphApi.User
{
    public class Create
    {
        public Create()
        {
            
        }

        public bool AccountEnabled { get; set; }

        public string DisplayName => $"{GivenName} {Surname}";

        public string MailNickname => $"{GivenName}{Surname}";

        public string CreationType => "LocalAccount";

        public string PasswordPolicies => "DisablePasswordExpiration";

        public string GivenName { get; set; }

        public string Surname { get; set; }

        public string SignInName { get; set; }
    }
}
