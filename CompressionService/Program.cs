namespace CompressionSerive
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddWindowsService();
            Worker.ConfigureService(builder.Configuration, builder.Services, args.Length > 0 ? args[0] : "", 0);
            builder.Services.AddHostedService<Worker>();

            var host = builder.Build();
            host.Run();
        }
    }
}