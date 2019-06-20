using Microsoft.Extensions.Configuration;

namespace PM.Business.Email
{
    public class Request
    {
        private readonly IConfiguration _configuration;

        public Request(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
