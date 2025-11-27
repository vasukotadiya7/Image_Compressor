

using System.Threading;

namespace CompressionSerive
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private IHost? _webHost;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        public static void ConfigureService(ConfigurationManager configuration, IServiceCollection services, string serviceName, int referenceId)
        {
            services.AddSingleton<ImageCompressorDemo.Compression>();
        }
        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            
            _ = Task.Run(() => StartWebHostAsync(stoppingToken), stoppingToken);
            await base.StartAsync(stoppingToken);

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_webHost is not null)
            {
                try
                {
                    _logger.LogInformation("Stopping web host...");
                    await _webHost.StopAsync(cancellationToken);
                    _logger.LogInformation("Web host stopped.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while stopping web host.");
                }
            }

            await base.StopAsync(cancellationToken);
        }
        private async Task StartWebHostAsync(CancellationToken stoppingToken)
        {
            int portNumber = 21922;
            string URL = $"http://localhost:{portNumber}";
            WebApplicationBuilder builder = ConfigureWebHost(URL);
            var host = builder.Build();
            var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
            lifetime.ApplicationStarted.Register(() =>
            {
                _logger.LogInformation("Service started at: {time}", DateTimeOffset.Now);
            });
            host.MapControllers();

            _webHost = host;
            await host.RunAsync(stoppingToken);
        }

        private WebApplicationBuilder ConfigureWebHost(string url)
        {
            var builder = WebApplication.CreateBuilder();
            builder.WebHost.UseUrls(url);
            builder.WebHost.ConfigureServices((hostContext, services) =>
            {
                services.AddControllers();
                services.AddHttpContextAccessor();
                services.AddSingleton<ImageCompressorDemo.Compression>();
            });
            return builder;
        }
    }
}
