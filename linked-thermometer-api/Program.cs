
namespace linked_thermometer_api
{
    using linked_thermometer_api.Interfaces;
    using linked_thermometer_api.Services;
    using Microsoft.Azure.Cosmos;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string endpoint = builder.Configuration["Cosmos:Endpoint"] ?? string.Empty;
            string database = builder.Configuration["Cosmos:Database"] ?? string.Empty;
            string container = builder.Configuration["Cosmos:Container"] ?? string.Empty;            

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR();

            builder.Services.AddSingleton(s =>
            {
                return new CosmosClient(endpoint);
            });

            builder.Services.AddSingleton<IReadingService, ReadingService>(s =>
            {
                var client = s.GetRequiredService<CosmosClient>();
                var c = client.GetContainer(database, container);
                return new ReadingService(c, builder.Configuration);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
