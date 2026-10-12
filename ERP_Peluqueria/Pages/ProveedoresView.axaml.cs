using Avalonia.Controls;
using Avalonia.Interactivity;
using MongoDB.Driver;
using ERP_Peluqueria.Datos;
using ERP_Peluqueria.Modelos;

namespace ERP_Peluqueria.Pages;

public partial class ProveedoresView : UserControl
{
    private readonly AppDbContexto _context;
    private Proveedor? _proveedorSeleccionado;

    public ProveedoresView(AppDbContexto context)
    {
        InitializeComponent();
        _context = context;
        CargarProveedores();
    }

    private async void CargarProveedores(string filtro = "")
    {
        var filterDef = string.IsNullOrWhiteSpace(filtro)
            ? Builders<Proveedor>.Filter.Empty
            : Builders<Proveedor>.Filter.Or(
                Builders<Proveedor>.Filter.Regex(p => p.Nombre, new MongoDB.Bson.BsonRegularExpression(filtro, "i")),
                Builders<Proveedor>.Filter.Regex(p => p.Contacto, new MongoDB.Bson.BsonRegularExpression(filtro, "i"))
              );

        var lista = await _context.Proveedores.Find(filterDef).ToListAsync();
        GridProveedores.ItemsSource = lista;
    }

    private void TxtBuscar_TextChanged(object? sender, TextChangedEventArgs e) => CargarProveedores(TxtBuscar.Text ?? "");
    private void BtnRefrescar_Click(object? sender, RoutedEventArgs e) => CargarProveedores();

    private void GridProveedores_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (GridProveedores.SelectedItem is Proveedor prov)
        {
            _proveedorSeleccionado = prov;
            TxtNombre.Text = prov.Nombre;
            TxtContacto.Text = prov.Contacto;
            TxtTelefono.Text = prov.Telefono;

            TxtTituloForm.Text = "Editar Proveedor";
            BtnEliminar.IsVisible = true;
        }
    }

    private async void BtnGuardar_Click(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtNombre.Text)) return;

        if (_proveedorSeleccionado == null)
        {
            var nuevo = new Proveedor
            {
                Nombre = TxtNombre.Text.Trim(),
                Contacto = TxtContacto.Text?.Trim() ?? "",
                Telefono = TxtTelefono.Text?.Trim() ?? ""
            };
            await _context.Proveedores.InsertOneAsync(nuevo);
        }
        else
        {
            var update = Builders<Proveedor>.Update
                .Set(p => p.Nombre, TxtNombre.Text.Trim())
                .Set(p => p.Contacto, TxtContacto.Text?.Trim() ?? "")
                .Set(p => p.Telefono, TxtTelefono.Text?.Trim() ?? "");

            await _context.Proveedores.UpdateOneAsync(p => p.Id == _proveedorSeleccionado.Id, update);
        }

        LimpiarCampos();
        CargarProveedores();
    }

    private async void BtnEliminar_Click(object? sender, RoutedEventArgs e)
    {
        if (_proveedorSeleccionado != null)
        {
            await _context.Proveedores.DeleteOneAsync(p => p.Id == _proveedorSeleccionado.Id);
            LimpiarCampos();
            CargarProveedores();
        }
    }

    private void BtnLimpiar_Click(object? sender, RoutedEventArgs e) => LimpiarCampos();

    private void LimpiarCampos()
    {
        _proveedorSeleccionado = null;
        TxtNombre.Text = "";
        TxtContacto.Text = "";
        TxtTelefono.Text = "";
        TxtTituloForm.Text = "Nuevo Proveedor";
        BtnEliminar.IsVisible = false;
        GridProveedores.SelectedItem = null;
    }
}