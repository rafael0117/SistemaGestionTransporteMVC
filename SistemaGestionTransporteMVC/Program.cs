using SistemaGestionTransporteMVC.Controllers;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole(); // Muestra los errores en la consola
builder.Logging.AddDebug();   // Muestra los errores en el depurador

// Agregar servicios para las sesiones **antes de construir la aplicación**
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiración de la sesión
    options.Cookie.HttpOnly = true; // Solo accesible por el servidor
    options.Cookie.IsEssential = true; // Asegura que la cookie sea esencial
});

builder.Services.AddHttpClient<ViajeController>();

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configurar el pipeline de solicitudes
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Usar las sesiones antes de enrutamiento
app.UseSession();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cliente}/{action=Index}/{id?}");

app.Run();
