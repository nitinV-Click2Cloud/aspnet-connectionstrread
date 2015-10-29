using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Runtime;
using System.Data.SqlClient;
using System.Data;
namespace UsingOptions
{
    public class Startup
    {
        static string connectionString = string.Empty;
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }
        public IConfiguration Configuration { get; set; }
        public Startup(IApplicationEnvironment appEnv)
        {
            var configBuilder = new Microsoft.Framework.Configuration.ConfigurationBuilder(appEnv.ApplicationBasePath)
                .AddJsonFile("config.json");
            Configuration = configBuilder.Build();
            connectionString = Configuration.Get("Data:DefaultConnection:ConnectionString");

        }
        public void Configure(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("<h1>Hello World!</h1><br/>Connection String For This " + Configuration.Get("Data:DefaultConnection:ConnectionString"));
                var products = await GetProductsAsync();
                await context.Response.WriteAsync("<br/>Total Products: " + products.Count);
              
            });
        }
        private async Task<IList<Product>> GetProductsAsync()
        {
            var sqlCommandWrapper = new SqlCommandWrapper(connectionString, 900);// connection string and timeout
            var parameters = new SqlParameter[] { };
            return (await sqlCommandWrapper.ExecuteReaderAsync(CommandType.Text, // For stored-procedures no need to pass  CommandType param
                "Select * From test1",
                r =>
                new Product
                {
                    Id = (int)r["Id"],
                   FirstName  = r["Name"].ToString(),
                }, parameters)).ToList();
        }

        private class Product
        {
            public int Id { get; set; }
            public string FirstName { get; set; }

        }
    }
}
