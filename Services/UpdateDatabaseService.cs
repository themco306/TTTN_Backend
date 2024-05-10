// using backend.Services;
// using Cronos;

// public class UpdateDatabaseService : BackgroundService
// {
//     private readonly ILogger<UpdateDatabaseService> _logger;
//     private readonly CronExpression _cronExpression;
//     private readonly IServiceProvider _services;

//     public UpdateDatabaseService(ILogger<UpdateDatabaseService> logger, IServiceProvider services)
//     {
//         _logger = logger;
//         _cronExpression = CronExpression.Parse("*/59 * * * *"); // Mỗi 5 phút
//         _services = services;
//     }

//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         while (!stoppingToken.IsCancellationRequested)
//         {
//             var next = _cronExpression.GetNextOccurrence(DateTime.UtcNow);

//             if (next.HasValue)
//             {
//                 var delay = next.Value - DateTime.UtcNow;

//                 if (delay.TotalMilliseconds > 0)
//                 {
//                     await Task.Delay(delay, stoppingToken);
//                 }

//                 _logger.LogInformation("Updating database at: {time}", DateTime.UtcNow);

//                 // using (var scope = _services.CreateScope())
//                 // {
//                 //     var productService = scope.ServiceProvider.GetRequiredService<ProductService>();
//                 //     await productService.UpdateTagProductAsync();
//                 // }
//             }
//         }
//     }
// }
