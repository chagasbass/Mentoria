using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace Mentoria
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();

            CreateLogger();

            try
            {
                Log.Information("Iniciando start de webHost");

                var host = CreateWebHostBuilder();
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal($"Deu ruim!! motivo: {ex.Message}, stacktrace: {ex.StackTrace}");
            }
            finally
            {
                Log.Information("Finalizando log");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseIISIntegration();
                    webBuilder.UseKestrel(options =>
                    {
                        options.ListenLocalhost(7070, options => options.UseHttps());
                        options.ListenLocalhost(7071, options => options.UseHttps());
                    });
                });

        /// <summary>
        /// Cria um WebHostBuilder add arquivos de configuração extra na aplicação
        /// </summary>
        /// <returns></returns>
        public static IWebHost CreateWebHostBuilder()
        {
            Log.Information("Efetuando a configuração do arquivo de hosting");

            var host =  new WebHostBuilder()
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseSerilog()
                   .ConfigureAppConfiguration((hostingContext, config) =>
                   {
                       config.AddJsonFile("hosting.json", optional: false, reloadOnChange: false);
                       config.AddJsonFile("logsettings.json", optional: false, reloadOnChange: false);
                   })
                    .UseStartup<Startup>()
                    .Build();


            Log.Information("Configuração Efetuada");

            return host;
        }

        /// <summary>
        /// Efetua a criação do serviço de logger
        /// </summary>
        public static void CreateLogger()
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

            Log.Information("Configuração de log criada");
        }
    }
}
