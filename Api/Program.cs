using Application.Permissions.Commands;
using Application.Permissions.Querys;
using Domain.Interface.Base;
using Domain.Interface.Permissions;
using Domain.Interface.UnitOfWorks;
using Domain.Permissions;
using Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Permissions;
using Infrastructure.UnitOfWork;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql;

var builder = WebApplication.CreateBuilder(args);




var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<IAppDbContext,AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllers();

builder.Services.AddMediatR(typeof(Program).Assembly);

builder.Services.AddScoped(typeof(IRepository<Permission>), typeof(Repository<Permission>));
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPermissionTypeRepository, PermissionTypeRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IRequestHandler<RequestPermissionCommand, Permission>, RequestPermissionCommandHandler>();
builder.Services.AddScoped<IRequestHandler<ModifyPermissionCommand, Permission>, ModifyPermissionCommandHandler>();
builder.Services.AddScoped<IRequestHandler<GetPermissionsQuery, IEnumerable<Permission>>, GetPermissionsQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetPermissionsFilterQuery, Permission?>, GetPermissionsFilterQueryHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Migrate the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();


