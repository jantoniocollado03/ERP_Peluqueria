using System;
using Avalonia;
using ERP_Peluqueria.Datos;

namespace ERP_Peluqueria;

internal class Program
{
    // El punto de entrada principal para una aplicación de escritorio
    [STAThread]
    public static void Main(string[] args)
    {
        // 1. Inicializar la base de datos MongoDB al arrancar
        try
        {
            var contexto = new AppDbContexto();
            DbInitializer.Initialize(contexto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al inicializar la base de datos: {ex.Message}");
        }

        // 2. Arrancar el ciclo de vida de la aplicación Avalonia
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}