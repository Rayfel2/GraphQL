using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using api3.Models;
using api3.GraphQL;
using api3.Interface;
using api3.Repository;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<InterfaceEmployee, RepositoryEmployee>();
builder.Services.AddScoped<InterfaceStore, RepositoryStore>();
builder.Services.AddScoped<InterfaceInventory, RepositoryInventory>();
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<PgAdminContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddProjections()
    .AddSorting()
    .AddFiltering()
    .AddType<IcecreamType>()
   .AddInputObjectType<InventoryInput>() // Ya este trae consigo los otros inputs
    .AddMutationType<Mutation>()
    ;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthorization();

app.MapGraphQL();


app.Run();

