namespace PM.AzureB2C.Models.User
{
    public class PasswordProfile
    {
        public bool ForceChangePasswordNextLogin => false;

        public string Password { get; set; }
    }
}
