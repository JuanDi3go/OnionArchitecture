using Application;
using Application.Features.Clientes.Commands.CreateClienteCommand;
using Identity;
using Identity.Models;
using Identity.Seeds;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Persistence;
using Shared;
using System.Reflection;
using WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);
//var host = WebApplication.CreateBuilder(args).Build();




    // Add services to the container.

    builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationLayer();// esta conecta a services de aplication para poder usar los que matricule en ese lugar
builder.Services.AddIdentityInfraestructure(builder.Configuration);
builder.Services.AddPersistenceInfraestructure(builder.Configuration);
builder.Services.AddSharedInfraEstructure(builder.Configuration);
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(MediatrEntryPoint).Assembly);
});
builder.Services.AddApiVersioningExtension();



var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await DefaultRoles.SeedAsync(userManager, roleManager);
        await DefaultAdminUser.SeedAsync(userManager, roleManager);
        await DefaultBasicUser.SeedAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {

        throw;
    }

}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseErrorHandlingMiddleware();
app.MapControllers();

app.Run();
