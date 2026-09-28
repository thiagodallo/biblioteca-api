using BibliotecaApi.Controllers;
using BibliotecaApi.Grpc;
using BibliotecaApi.Repositories;
using BibliotecaApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BibliotecaContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Biblioteca")));

builder.Services.AddScoped<IAutorRepository, AutorRepository>();
builder.Services.AddScoped<ILivroRepository, LivroRepository>();
builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<ILivroService, LivroService>();

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<TratadorExcecoesHttp>();

builder.Services.AddGrpc(options => options.Interceptors.Add<InterceptorExcecoesGrpc>());
builder.Services.AddGrpcReflection();

var app = builder.Build();

await SeedInicial.PrepararBancoAsync(app.Services);

app.UseExceptionHandler();

app.MapControllers();
app.MapGrpcService<AutoresGrpcService>();
app.MapGrpcService<LivrosGrpcService>();
app.MapGrpcReflectionService();

app.Run();
