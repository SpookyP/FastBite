using MenuCatalog.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MenuCatalog.Infrastructure.Services
{
    public class DailyStockResetService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public DailyStockResetService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Este método é executado em segundo plano e é responsável por redefinir a quantidade vendida de cada item no final do dia. Ele calcula o tempo restante até a meia-noite e aguarda esse período antes de executar a redefinição.
        /// </summary>
        /// <param name="stoppingToken"></param>

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) // Loop infinito que continua enquanto o serviço não for cancelado
            {
                var agora = DateTime.Now;
                var amanha = agora.Date.AddDays(1);
                var tempoAteMeiaNoite = amanha - agora;

                await Task.Delay(tempoAteMeiaNoite, stoppingToken);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var repo = scope.ServiceProvider.GetRequiredService<IItemRepository>(); //
                    var todosItens = await repo.GetAllAsync();

                    foreach (var item in todosItens)
                    {
                        item.QuantidadeVendidaHoje = 0;
                        await repo.UpdateItemAsync(item);
                    }
                }
            }
        }

    }
}
