using WebApiPeliculas;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
#pragma warning disable CS8600 // Se va a convertir un literal nulo o un posible valor nulo en un tipo que no acepta valores NULL
var servicioLogger = (ILogger<Startup>)app.Services.GetService(typeof(ILogger<Startup>));
#pragma warning restore CS8600 // Se va a convertir un literal nulo o un posible valor nulo en un tipo que no acepta valores NULL
#pragma warning disable CS8604 // Posible argumento de referencia nulo
startup.configure(app, app.Environment, servicioLogger);
#pragma warning restore CS8604 // Posible argumento de referencia nulo



// Configure the HTTP request pipeline.

app.Run();
