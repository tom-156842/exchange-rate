using ExchangeRates.Domain.Integrations;
using ExchangeRates.Domain.Services;
using ExchangeRates.Integration.ExchangeRateHost;

namespace ExchangeRates.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder);

            var app = builder.Build();
            ConfigureApp(app);

            app.Run();
        }

        private static void ConfigureApp(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapControllers();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient();

            builder.Services.AddScoped<IExchangeRateService, ExchangeRateService>();
            builder.Services.AddScoped<IExchangeRateProvider, ExchangeRateProvider>();
            builder.Services.AddScoped<IExchangeRateHostClient, ExchangeRateHostClient>();
            builder.Services.Configure<ExchangeRateHostClientOptions>(builder.Configuration.GetSection(ExchangeRateHostClientOptions.DefaultSectionName));
        }
    }
}