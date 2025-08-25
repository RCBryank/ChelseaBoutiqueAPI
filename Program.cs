using Chelsea_Boutique.Models;
using Chelsea_Boutique.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var origins = builder.Configuration.GetValue<string>("AllowedOrigins")!.Split(",");
        policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors();
app.MapControllers();

string[] passwords = { "p", "passw", "qwe#qw2!" };
foreach (string pass in passwords)
{
    Debug.WriteLine("Salt generada: " + HashPasswordService.getSalt());
}

app.MapGet("/minimaltabletest", async () =>
{
    var url = builder.Configuration.GetValue<string>("SupabaseAPI:URL");
    var key = builder.Configuration.GetValue<string>("SupabaseAPI:Key");

    var supabase = new Supabase.Client(url, key, new Supabase.SupabaseOptions { AutoConnectRealtime = true });
    await supabase.InitializeAsync();
});

app.Run();
