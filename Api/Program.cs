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
using Microsoft.EntityFrameworkCore;
using Nest;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Microsoft.Extensions.Logging;
using Api;

internal class Program
{

    public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureWebHostDefaults(webBuilder =>
               {
                   webBuilder.UseStartup<Startup>();
               });

    private static void Main(string[] args)
    {
       
        CreateHostBuilder(args).Build().Run();

    }
}