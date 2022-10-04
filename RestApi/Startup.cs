using DomainLogic.Repositories;
using DomainLogic.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Implementations.RepositoriesMongoDB;
using MongoDB.Driver;
using Implementations.RepositoriesEF;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using System.Text.Json;
using Implementations.MassTransitMq;

namespace RestApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var rabbitMqConfigJson = Configuration.GetValue<string>("checkerGameRabbitMqConfig");
            RabbitMqConfig = JsonSerializer.Deserialize<RabbitMqConfig>(rabbitMqConfigJson);
        }

        public IConfiguration Configuration { get; }

        public RabbitMqConfig RabbitMqConfig { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var mongoClientSettings = MongoClientSettings.FromConnectionString(Configuration.GetValue<string>("checkersMongoConnectionString"));

            services
                .AddTransient<GameService>()
                .AddTransient<IGameRepository, GameRepository>()
                .AddTransient<IGameHistoryRepository, MongoGameHistoryRepository>()
                .AddTransient<MoveManager>()
                .AddScoped<IBotNotifier, MassTransitBotNotifier>()
                .AddSingleton(mongoClientSettings)
                .AddTransient<GameBoardStateMongoDBContext>()
                .AddDbContext<GameDbContext>(options =>
                    options.UseNpgsql(
                        Configuration.GetValue<string>("checkerGameConnectionString"),
                        a => a.MigrationsAssembly(typeof(GameDbContext).Assembly.FullName)));

            services.AddMassTransit(x =>
            {
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
                {
                    config.Host(RabbitMqConfig.Host, RabbitMqConfig.VirtualHost, h =>
                    {
                        h.Username(RabbitMqConfig.Username);
                        h.Password(RabbitMqConfig.Password);

                    });
                }));
            });

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddControllers()
                .AddJsonOptions(opts =>
                {
                    var enumConverter = new JsonStringEnumConverter();
                    opts.JsonSerializerOptions.Converters.Add(enumConverter);
                });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RestApi", Version = "v1" });
            });

            var context = services.BuildServiceProvider().GetService<GameBoardStateMongoDBContext>();
            context.ConfigureIndexes();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RestApi v1"));
            }

            app.UseCors(builder => builder
                          .AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .WithExposedHeaders());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
