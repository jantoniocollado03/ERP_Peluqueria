using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MongoDB.Driver;
using ERP_Peluqueria.Datos;
using ERP_Peluqueria.Modelos;

namespace ERP_Peluqueria.Pages;

public partial class ArticulosView : UserControl
{
    private readonly AppDbContexto _context;
    private Articulo? _articuloSeleccionado;

    public ArticulosView(AppDbContexto context)
    {
        InitializeComponent();
        _context = context;
        CargarArticulos();
    }

    private async void CargarArticulos(string filtro = "")
    {
        var filterDef = string.IsNullOrWhiteSpace(filtro)
            ? Builders<Articulo>.Filter.Empty
            : Builders<Articulo>.Filter.Regex(a => a.Nombre, new MongoDB.Bson.BsonRegularExpression(filtro, "i"));

        var lista = await _context.Items.Find(filterDef).ToListAsync();
        GridArticulos.ItemsSource = lista;
    }

    private void TxtBuscar_TextChanged(object? sender, TextChangedEventArgs e) => CargarArticulos(TxtBuscar.Text ?? "");
    private void BtnRefrescar_Click(object? sender, RoutedEventArgs e) => CargarArticulos();

    private void CmbTipo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (PanelStock == null || PanelDuracion == null) return;

        bool esProducto = (CmbTipo.SelectedItem as ComboBoxItem)?.Content?.ToString() == "Producto";
        PanelStock.IsVisible = esProducto;
        PanelDuracion.IsVisible = !esProducto;
    }

    private void GridArticulos_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (GridArticulos.SelectedItem is Articulo art)
        {
            _articuloSeleccionado = art;
            TxtNombre.Text = art.Nombre;
            TxtPrecio.Text = art.Precio.ToString("0.00", CultureInfo.InvariantCulture);
            TxtStock.Text = art.Stock.ToString();
            TxtDuracion.Text = art.DuracionMinutos.ToString();
            CmbTipo.SelectedIndex = art.Tipo == "Producto" ? 1 : 0;

            TxtTituloForm.Text = "Editar Artículo";
            BtnEliminar.IsVisible = true;
        }
    }

    private async void BtnGuardar_Click(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TxtNombre.Text)) return;

        decimal.TryParse(TxtPrecio.Text?.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precio);
        int.TryParse(TxtStock.Text, out int stock);
        int.TryParse(TxtDuracion.Text, out int duracion);
        string tipo = (CmbTipo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Servicio";

        if (_articuloSeleccionado == null)
        {
            var nuevo = new Articulo
            {
                Nombre = TxtNombre.Text.Trim(),
                Tipo = tipo,
                Precio = precio,
                Stock = tipo == "Producto" ? stock : 0,
                DuracionMinutos = tipo == "Servicio" ? duracion : 0
            };
            await _context.Items.InsertOneAsync(nuevo);
        }
        else
        {
            var update = Builders<Articulo>.Update
                .Set(a => a.Nombre, TxtNombre.Text.Trim())
                .Set(a => a.Tipo, tipo)
                .Set(a => a.Precio, precio)
                .Set(a => a.Stock, tipo == "Producto" ? stock : 0)
                .Set(a => a.DuracionMinutos, tipo == "Servicio" ? duracion : 0);

            await _context.Items.UpdateOneAsync(a => a.Id == _articuloSeleccionado.Id, update);
        }

        LimpiarCampos();
        CargarArticulos();
    }

    private async void BtnEliminar_Click(object? sender, RoutedEventArgs e)
    {
        if (_articuloSeleccionado != null)
        {
            await _context.Items.DeleteOneAsync(a => a.Id == _articuloSeleccionado.Id);
            LimpiarCampos();
            CargarArticulos();
        }
    }

    private void BtnLimpiar_Click(object? sender, RoutedEventArgs e) => LimpiarCampos();

    private void LimpiarCampos()
    {
        _articuloSeleccionado = null;
        TxtNombre.Text = "";
        TxtPrecio.Text = "";
        TxtStock.Text = "";
        TxtDuracion.Text = "30";
        CmbTipo.SelectedIndex = 0;
        TxtTituloForm.Text = "Nuevo Artículo / Servicio";
        BtnEliminar.IsVisible = false;
        GridArticulos.SelectedItem = null;
    }
}