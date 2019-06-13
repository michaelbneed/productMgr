namespace PM.Auth.GraphApi.User
{
    public class PasswordProfile
    {
        public bool ForceChangePasswordNextLogin => false;

        public string Password { get; set; }
    }
}
