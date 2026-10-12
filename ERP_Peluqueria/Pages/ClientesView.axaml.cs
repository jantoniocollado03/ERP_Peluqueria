using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MongoDB.Driver;
using ERP_Peluqueria.Datos;
using ERP_Peluqueria.Modelos;

namespace ERP_Peluqueria.Pages;

public partial class ClientesView : UserControl
{
    private readonly AppDbContexto _context;
    private Cliente? _clienteSeleccionado;

    public ClientesView(AppDbContexto context)
    {
        InitializeComponent();
        _context = context;
        CargarClientes();
    }

    private async void CargarClientes(string filtro = "")
    {
        var filterDef = string.IsNullOrWhiteSpace(filtro)
            ? Builders<Cliente>.Filter.Empty
            : Builders<Cliente>.Filter.Or(
                Builders<Cliente>.Filter.Regex(c => c.Nombre, new MongoDB.Bson.BsonRegularExpression(filtro, "i")),
                Builders<Cliente>.Filter.Regex(c => c.Telefono, new MongoDB.Bson.BsonRegularExpression(filtro, "i"))
              );

        var lista = await _context.Clientes.Find(filterDef).ToListAsync();
        GridClientes.ItemsSource = lista;
    }

    private void TxtBuscar_TextChanged(object? sender, TextChangedEventArgs e) => CargarClientes(TxtBuscar.Text ?? "");
    private void BtnRefrescar_Click(object? sender, RoutedEventArgs e) => CargarClientes();

    private void GridClientes_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (GridClientes.SelectedItem is Cliente c)
        {
            _clienteSeleccionado = c;
            TxtNombre.Text = c.Nombre;
            TxtTelefono.Text = c.Telefono;
            TxtEmail.Text = c.Email;
            TxtHistorial.Text = c.HistorialTecnico;
            TxtTituloForm.Text = "Editar Cliente";
            BtnEliminar.IsVisible = true;
        }
    }

    private async void BtnGuardar_Click(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtNombre.Text)) return;

        if (_clienteSeleccionado == null)
        {
            var nuevo = new Cliente
            {
                Nombre = TxtNombre.Text.Trim(),
                Telefono = TxtTelefono.Text?.Trim() ?? "",
                Email = TxtEmail.Text?.Trim() ?? "",
                HistorialTecnico = TxtHistorial.Text?.Trim() ?? ""
            };
            await _context.Clientes.InsertOneAsync(nuevo);
        }
        else
        {
            var update = Builders<Cliente>.Update
                .Set(c => c.Nombre, TxtNombre.Text.Trim())
                .Set(c => c.Telefono, TxtTelefono.Text?.Trim() ?? "")
                .Set(c => c.Email, TxtEmail.Text?.Trim() ?? "")
                .Set(c => c.HistorialTecnico, TxtHistorial.Text?.Trim() ?? "");

            await _context.Clientes.UpdateOneAsync(c => c.Id == _clienteSeleccionado.Id, update);
        }

        LimpiarCampos();
        CargarClientes();
    }

    private async void BtnEliminar_Click(object? sender, RoutedEventArgs e)
    {
        if (_clienteSeleccionado != null)
        {
            await _context.Clientes.DeleteOneAsync(c => c.Id == _clienteSeleccionado.Id);
            LimpiarCampos();
            CargarClientes();
        }
    }

    private void BtnLimpiar_Click(object? sender, RoutedEventArgs e) => LimpiarCampos();

    private void LimpiarCampos()
    {
        _clienteSeleccionado = null;
        TxtNombre.Text = "";
        TxtTelefono.Text = "";
        TxtEmail.Text = "";
        TxtHistorial.Text = "";
        TxtTituloForm.Text = "Nuevo Cliente";
        BtnEliminar.IsVisible = false;
        GridClientes.SelectedItem = null;
    }
}