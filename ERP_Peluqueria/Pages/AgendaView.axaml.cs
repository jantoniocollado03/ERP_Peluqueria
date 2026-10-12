using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MongoDB.Driver;
using ERP_Peluqueria.Datos;
using ERP_Peluqueria.Modelos;

namespace ERP_Peluqueria.Pages;

public partial class AgendaView : UserControl
{
    private readonly AppDbContexto _context;
    private readonly string _empleadoActual;

    public AgendaView(AppDbContexto context, string empleadoActual)
    {
        InitializeComponent();
        _context = context;
        _empleadoActual = empleadoActual;
        TxtFechaHora.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        CargarDatos();
    }

    private async void CargarDatos()
    {
        var citas = await _context.Citas.Find(Builders<Cita>.Filter.Empty).SortBy(c => c.FechaHora).ToListAsync();
        GridCitas.ItemsSource = citas;

        var clientes = await _context.Clientes.Find(Builders<Cliente>.Filter.Empty).ToListAsync();
        CmbClientes.ItemsSource = clientes;

        // Solo articulos de tipo Servicio
        var servicios = await _context.Items.Find(a => a.Tipo == "Servicio").ToListAsync();
        CmbServicios.ItemsSource = servicios;
    }

    private void BtnRecargar_Click(object? sender, RoutedEventArgs e) => CargarDatos();

    private async void BtnGuardarCita_Click(object? sender, RoutedEventArgs e)
    {
        if (CmbClientes.SelectedItem is not Cliente cli || CmbServicios.SelectedItem is not Articulo srv)
            return;

        if (!DateTime.TryParse(TxtFechaHora.Text, out DateTime fechaHora))
            return;

        var cita = new Cita
        {
            ClienteId = cli.Id,
            ClienteNombre = cli.Nombre,
            EmpleadoNombre = _empleadoActual,
            ItemId = srv.Id,
            ServicioNombre = srv.Nombre,
            DuracionMinutos = srv.DuracionMinutos,
            FechaHora = fechaHora,
            Estado = EstadoCita.Pendiente
        };

        await _context.Citas.InsertOneAsync(cita);
        CargarDatos();
    }
}