using Microsoft.EntityFrameworkCore;
using Vofity.API.Components;
using Votify.Core.Interfaces;
using Votify.Core.Services;
using Votify.Infrastructure.Data;
using Votify.Infrastructure.Repositorios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<IEvento, EventoService>();
builder.Services.AddScoped<IEventoRepository,  EventoRepository>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped<IRegister, RegisterService>();
builder.Services.AddScoped<IProyectoService, ProyectoService>();
builder.Services.AddScoped<IProyectoRepository, ProyectoRepository>();
builder.Services.AddHttpClient<ILogin, LoginService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7053"); // usar 7053 no el 5129 , uno es para https y el otro para http
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.Run();
