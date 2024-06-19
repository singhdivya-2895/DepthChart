using Microsoft.AspNetCore.Mvc.Testing;

namespace Api.IntegrationTests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
    }
}
