using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using KafkaFlow;
using KafkaFlow.Serializer;


using MyBancoApi.Tarifas.Domain.Interfaces;
using MyBancoApi.Tarifas.Infrastructure.Repositories;
using MyBancoApi.Tarifas.Infrastructure.Database;
using MyBancoApi.Tarifas.Application.Handlers;
using MyBancoApi.Tarifas.Application.Interfaces;
using MyBancoApi.Tarifas.Infrastructure.Producers;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        // 1. Inicializa o Banco de Dados
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var db = services.GetRequiredService<SqliteConnection>();
            db.Open();
            new DbInitializer(db).Initialize();
        }

        // 2. Inicializa o KafkaFlow Bus
        var bus = host.Services.CreateKafkaBus();
        await bus.StartAsync();

        // 3. Executa a aplicação
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .ConfigureServices((hostContext, services) =>
            {
                // 1. Banco de Dados
                var connectionString = "Data Source=/dbdata/tarifas.db";
                services.AddSingleton(new SqliteConnection(connectionString));
                services.AddSingleton<DbInitializer>();

                // 2. Repositórios e Handlers
                services.AddScoped<ITarifaRepository, TarifaRepository>();
                services.AddScoped<ITarifaProducer, TarifaProducer>();
                services.AddScoped<TransferenciaConsumerHandler>();

                // 3. KafkaFlow
                services.AddKafka(
                    kafka => kafka
                        .AddCluster(
                            cluster => cluster
                                .WithBrokers(
                                    hostContext.Configuration
                                        .GetSection("Kafka:Brokers")
                                        .Get<string[]>()
                                )
                                .AddConsumer(
                                    consumer => consumer
                                        .Topic("transferencias_realizadas")
                                        .WithGroupId("tarifas-processor-group")
                                        .WithWorkersCount(3)
                                        .WithBufferSize(100)
                                        .AddMiddlewares(m => m
                                        .AddDeserializer<JsonCoreDeserializer>() // ✅ use este no consumer
                                        .AddTypedHandlers(h => h.AddHandler<TransferenciaConsumerHandler>())
                                    )
                                )
                                .AddProducer(
                                    "tarifas-producer",
                                    producer => producer
                                        .DefaultTopic("tarifas_realizadas")
                                        .AddMiddlewares(m => m.AddSerializer<JsonCoreSerializer>())
                                )
                        )
                );
            });
}
