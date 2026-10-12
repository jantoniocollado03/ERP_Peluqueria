using System.Linq;
using Avalonia.Controls;
using MongoDB.Driver;
using ERP_Peluqueria.Datos;
using ERP_Peluqueria.Modelos;

namespace ERP_Peluqueria.Pages;

public partial class InformesView : UserControl
{
    private readonly AppDbContexto _context;

    public InformesView(AppDbContexto context)
    {
        InitializeComponent();
        _context = context;
        GenerarInformes();
    }

    private async void GenerarInformes()
    {
        var ventas = await _context.Ventas.Find(Builders<Venta>.Filter.Empty).ToListAsync();

        // 1. Facturación por Mes
        var informeMes = ventas
            .GroupBy(v => v.Fecha.ToString("MM/yyyy"))
            .Select(g => new { Periodo = g.Key, Total = g.Sum(x => x.Total) })
            .ToList();
        GridInformeMeses.ItemsSource = informeMes;

        // 2. Artículos / Servicios Estrella
        var informeEstrella = ventas
            .SelectMany(v => v.Lineas)
            .GroupBy(l => l.Nombre)
            .Select(g => new { Nombre = g.Key, Unidades = g.Sum(x => x.Cantidad) })
            .OrderByDescending(x => x.Unidades)
            .ToList();
        GridInformeEstrella.ItemsSource = informeEstrella;

        // 3. Mejores Clientes
        var informeClientes = ventas
            .GroupBy(v => v.ClienteNombre)
            .Select(g => new { ClienteNombre = g.Key, TotalGastado = g.Sum(x => x.Total) })
            .OrderByDescending(x => x.TotalGastado)
            .ToList();
        GridInformeClientes.ItemsSource = informeClientes;
    }
}