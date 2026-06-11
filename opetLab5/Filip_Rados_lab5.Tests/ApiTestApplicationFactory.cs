using Filip_Rados_lab5.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Filip_Rados_lab5.Tests;

public sealed class ApiTestApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName;

    public ApiTestApplicationFactory()
    {
        _databaseName = $"lab5_api_tests_{Guid.NewGuid():N}";
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:AppDbContext"] = "TestConnection",
                ["Authentication:Google:ClientId"] = "",
                ["Authentication:Google:ClientSecret"] = "",
                ["Authentication:Facebook:AppId"] = "",
                ["Authentication:Facebook:AppSecret"] = ""
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}
