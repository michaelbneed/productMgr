namespace PM.Auth.GraphApi.User
{
    public class PasswordProfile
    {
        public bool ForceChangePasswordNextLogin => true;

        public string Password { get; set; }
    }
}
