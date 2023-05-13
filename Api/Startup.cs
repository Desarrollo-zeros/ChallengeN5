using Application.ElasticsearchServices;
using Application.KafkaServices;
using Application.Logs.Querys;
using Application.Permissions.Commands;
using Application.Permissions.Querys;
using AutoMapper;
using Confluent.Kafka;
using Domain.Base;
using Domain.Interface.Base;
using Domain.Interface.ElasticsearchServices;
using Domain.Interface.KafkaServices;
using Domain.Interface.Permissions;
using Domain.Interface.UnitOfWorks;
using Domain.Permissions;
using Infrastructure;
using Infrastructure.Audilog;
using Infrastructure.Mapping;
using Infrastructure.Middleware;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Permissions;
using Infrastructure.Seeders;
using Infrastructure.UnitOfWork;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            var enviroment = Environment.GetEnvironmentVariable("SEQURL");
            if(enviroment == null)
            {
                DotNetEnv.Env.Load();
                Environment.SetEnvironmentVariable("SQL_SERVER_CONNECTION", Configuration["EnvironmentVariables:SQL_SERVER_CONNECTION"]);
                Environment.SetEnvironmentVariable("ELASTICSEARCH__URI", Configuration["EnvironmentVariables:Elasticsearch__Uri"]);
                Environment.SetEnvironmentVariable("SEQURL", Configuration["EnvironmentVariables:SEQURL"]);
                Environment.SetEnvironmentVariable("KAFKA__BOOTSTRAPSERVERS", Configuration["EnvironmentVariables:Kafka__BootstrapServers"]);
                
            }

            Log.Logger = CreateSerilogLogger();
            var connectionString = Environment.GetEnvironmentVariable("SQL_SERVER_CONNECTION");
            services.AddDbContext<IAppDbContext, AppDbContext>(options =>
                options.UseSqlServer(connectionString), ServiceLifetime.Scoped);

            services.AddControllers();
            services.AddScoped<ExceptionMiddleware>();
            services.AddTransient(x => x.GetRequiredService<RequestDelegate>());

            services.AddMediatR(typeof(Program).Assembly);
            services.AddScoped(typeof(Domain.Interface.Permissions.IRepository<>), typeof(Repository<>));
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IPermissionTypeRepository, PermissionTypeRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IElasticSearchApplication<>), typeof(ElasticSearchApplication<>));
            services.AddScoped<IRequestHandler<RequestPermissionCommand, PermissionResponse>, RequestPermissionCommandHandler>();
            services.AddScoped<IRequestHandler<ModifyPermissionCommand, PermissionResponse>, ModifyPermissionCommandHandler>();
            services.AddScoped<IRequestHandler<GetPermissionsQuery, IEnumerable<PermissionResponse>>, GetPermissionsQueryHandler>();
            services.AddScoped<IRequestHandler<GetPermissionsFilterQuery, PermissionResponse>, GetPermissionsFilterQueryHandler>();
            services.AddScoped<IRequestHandler<GetLogsQuery, IEnumerable<LogDto>>, GetLogsQueryHandler>();

            services.AddSingleton<IElasticClient>(provider =>
            {
                string elasticsearchUri = Environment.GetEnvironmentVariable("Elasticsearch__Uri") ?? "http://elasticsearch:9200";
                var settings = new ConnectionSettings(new Uri(elasticsearchUri));
                var client = new ElasticClient(settings);
                new Mapping()
                    .AddDefaultMappings<PermissionElasticsearch>(settings, "permissions")
                    .CreateIndex<PermissionElasticsearch>(client, "permissions");
                return client;
            });

            MapperConfiguration mappingConfig = new MapperConfiguration(config =>
            {
                config.AddMaps("Domain");
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            var producerConfig = new ProducerConfig
            {
                BootstrapServers = Configuration.GetValue<string>("Kafka:BootstrapServers")
            };
            services.AddSingleton(Configuration.GetValue<string>("Kafka:BootstrapServers"));
            services.AddSingleton(producerConfig);
            services.AddScoped<IKafkaProducer, KafkaService>();
            services.AddScoped<IKafkaConsumer, KafkaService>();
            services.AddSingleton(_ => new ProducerBuilder<Null, string>(producerConfig).Build());

            services.AddHttpContextAccessor();

        }


        static Serilog.ILogger CreateSerilogLogger()
        {
            var enviroment = Environment.GetEnvironmentVariable("SEQURL");
            return new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.WithProperty("ApplicationContext", "Api")
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Seq(enviroment)
                .CreateLogger();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Log.Information("Configuring web host ({ApplicationContext})...", "Api");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            Log.Information("Starting web host ({ApplicationContext})...", "Api");
        }
    }
}