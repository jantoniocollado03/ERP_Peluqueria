using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MongoDB.Driver;
using ERP_Peluqueria.Datos;
using ERP_Peluqueria.Modelos;

namespace ERP_Peluqueria.Pages;

public partial class VentasView : UserControl
{
    private readonly AppDbContexto _context;
    private readonly ObservableCollection<LineaVenta> _lineas = new();

    public VentasView(AppDbContexto context)
    {
        InitializeComponent();
        _context = context;
        GridLineas.ItemsSource = _lineas;
        CargarDatos();
    }

    private async void CargarDatos()
    {
        ListItemsCatalogo.ItemsSource = await _context.Items.Find(Builders<Articulo>.Filter.Empty).ToListAsync();
        CmbClienteVenta.ItemsSource = await _context.Clientes.Find(Builders<Cliente>.Filter.Empty).ToListAsync();
    }

    private void ListItemsCatalogo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (ListItemsCatalogo.SelectedItem is Articulo art)
        {
            var existente = _lineas.FirstOrDefault(l => l.ItemId == art.Id);
            if (existente != null)
            {
                existente.Cantidad++;
                // Notificar refresco de la cuadrícula
                var idx = _lineas.IndexOf(existente);
                _lineas[idx] = existente;
            }
            else
            {
                _lineas.Add(new LineaVenta
                {
                    ItemId = art.Id,
                    Nombre = art.Nombre,
                    Cantidad = 1,
                    PrecioUnitario = art.Precio
                });
            }
            RecalcularTotales();
            ListItemsCatalogo.SelectedItem = null;
        }
    }

    private void RecalcularTotales()
    {
        decimal total = _lineas.Sum(l => l.Cantidad * l.PrecioUnitario);
        decimal baseImp = Math.Round(total / 1.21m, 2);
        decimal iva = total - baseImp;

        TxtBase.Text = $"{baseImp:C}";
        TxtIva.Text = $"{iva:C}";
        TxtTotal.Text = $"{total:C}";
    }

    private async void BtnCobrar_Click(object? sender, RoutedEventArgs e)
    {
        if (!_lineas.Any() || CmbClienteVenta.SelectedItem is not Cliente cli) return;

        decimal total = _lineas.Sum(l => l.Cantidad * l.PrecioUnitario);
        decimal baseImp = Math.Round(total / 1.21m, 2);
        decimal iva = total - baseImp;

        var venta = new Venta
        {
            ClienteId = cli.Id,
            ClienteNombre = cli.Nombre,
            Fecha = DateTime.Now,
            BaseImponible = baseImp,
            Iva = iva,
            Total = total,
            Lineas = _lineas.ToList()
        };

        await _context.Ventas.InsertOneAsync(venta);

        // Descontar stock para productos físicos
        foreach (var linea in _lineas)
        {
            await _context.Items.UpdateOneAsync(
                i => i.Id == linea.ItemId && i.Tipo == "Producto",
                Builders<Articulo>.Update.Inc(i => i.Stock, -linea.Cantidad)
            );
        }

        _lineas.Clear();
        RecalcularTotales();
    }
}