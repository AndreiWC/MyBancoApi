using Microsoft.Data.Sqlite;

using MyBancoApi.Tarifas.Domain.Interfaces;
using MyBancoApi.Tarifas.Infrastructure.Repositories;
using MyBancoApi.Tarifas.Infrastructure.Database;
using MyBancoApi.Tarifas.Application.Handlers;

using KafkaFlow; // <-- Este using é o que define o 'CreateKafkaBus'
using MyBancoApi.Tarifas.Application.Interfaces;
using MyBancoApi.Tarifas.Infrastructure.Producers;
using Microsoft.Extensions.Configuration;




public class Program
{
    public static void Main(string[] args)
    {
        // 1. Constrói o host
        var host = CreateHostBuilder(args).Build();

        // --- LÓGICA DE INICIALIZAÇÃO MOVIDA PARA CÁ ---

        // 2. Cria um 'scope' para resolver os serviços de inicialização
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            // Inicializa o DB
            var db = services.GetService<SqliteConnection>();
            db.Open();
            new DbInitializer(db).Initialize();
        }

        // 3. Inicializa o Kafka Bus (agora no IServiceProvider correto)
        // O método 'CreateKafkaBus' será encontrado aqui.
        var bus = host.Services.CreateKafkaBus();
        bus.StartAsync().GetAwaiter().GetResult(); // Inicia o Kafka

        // 4. Roda a aplicação
        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                // 1. Configuração do Banco de Dados
                var connectionString = "Data Source=/dbdata/tarifas.db";
                services.AddSingleton(new SqliteConnection(connectionString));
                services.AddSingleton<DbInitializer>();

                // 2. Injeção de Dependência
                services.AddScoped<ITarifaRepository, TarifaRepository>();
                services.AddScoped<ITarifaProducer, TarifaProducer>();
                services.AddScoped<TransferenciaConsumerHandler>();

                // 3. Configuração do KafkaFlow
                services.AddKafkaFlow(kafka => kafka
                    .UseMicrosoftLog()
                    .AddCluster(cluster => cluster
                        .WithBrokers(hostContext.Configuration.GetSection("Kafka:Brokers").Get<string[]>())
                        .AddConsumer(consumer => consumer
                            .Topic("transferencias_realizadas")
                            .WithGroupId("tarifas-processor-group")
                            .WithWorkersCount(3)
                            .WithBufferSize(100)
                            .AddJsonSerializer()
                            .AddHandler<TransferenciaConsumerHandler>()
                        )
                        .AddProducer(
                            "tarifas-producer",
                            producer => producer
                                .DefaultTopic("tarifas_realizadas")
                                .AddJsonSerializer()
                        )
                    )
                );

                // 4. A LÓGICA DE INICIALIZAÇÃO FOI REMOVIDA DAQUI
            });
}