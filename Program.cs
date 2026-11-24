using ApiAgenteConversacional.AgenteConversacional.Aplicacion;
using ApiAgenteConversacional.AgenteConversacional.Services;
using CommunityToolkit.VectorData.InMemory;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(options =>
{
    options.RootDirectory = "/AgenteConversacional/Pages";
}); 
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<VectorStore, InMemoryVectorStore>();
builder.Services.AddSingleton<Kernel>(sp=>{
    var builderKernel = Kernel.CreateBuilder();
    builderKernel.AddOllamaChatCompletion(builder.Configuration["NombreAgente"], new Uri(builder.Configuration["UrlAgente"]));
    builderKernel.AddOllamaEmbeddingGenerator(builder.Configuration["EmbeddingGenerador"], new Uri(builder.Configuration["UrlAgente"]));
    return builderKernel.Build();
});

builder.Services.AddScoped<IAgenteService, ServicioAgente>();
builder.Services.AddScoped<IAgenteAplicacion,AgenteAplicacion>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app. UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllers();

app.MapRazorPages();

app.Run();
