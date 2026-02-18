using DomainLogic.Repositories;
using DomainLogic.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using Implementations.RepositoriesMongoDB;
using MongoDB.Driver;
using Implementations.RepositoriesEF;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using Implementations.MassTransitMq;
using Microsoft.OpenApi;

namespace RestApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var postgresConnectionString = Configuration.GetValue<string>("checkerGameConnectionString");
            var mongoDbConnectionString = Configuration.GetValue<string>("checkersMongoConnectionString");
            var rabbitMqConnectionString = Configuration.GetValue<string>("checkerGameRabbitMqConnectionString");
            var mongoClientSettings = MongoClientSettings.FromConnectionString(mongoDbConnectionString);

            services
                .AddTransient<GameService>()
                .AddTransient<IGameRepository, GameRepository>()
                .AddTransient<IGameHistoryRepository, MongoGameHistoryRepository>()
                .AddTransient<MoveManager>()
                .AddScoped<IBotNotifier, MassTransitBotNotifier>()
                .AddSingleton(mongoClientSettings)
                .AddScoped<IMongoClient, MongoClient>()
                .AddTransient<GameBoardStateMongoDBContext>()
                .AddDbContext<GameDbContext>(options =>
                    options.UseNpgsql(
                        postgresConnectionString,
                        a => a.MigrationsAssembly(typeof(GameDbContext).Assembly.FullName)));

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq( (_, config) =>
                {
                    config.Host(rabbitMqConnectionString);
                });
            });

            services.AddAutoMapper(expr => expr.AddProfile<MappingProfile>());

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
            
            using var scope = app.ApplicationServices.CreateScope();
            using var dbContext = scope.ServiceProvider.GetService<GameDbContext>();
            dbContext.Database.MigrateAsync().ConfigureAwait(false).GetAwaiter().GetResult();

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
