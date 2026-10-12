using Avalonia.Controls;
using Avalonia.Interactivity;
using ERP_Peluqueria.Datos;
using ERP_Peluqueria.Pages;

namespace ERP_Peluqueria.Views;

public partial class MainWindow : Window
{
    private readonly AppDbContexto _context;
    private string _rolActual = string.Empty;
    private string _nombreUsuario = string.Empty;

    public MainWindow()
    {
        InitializeComponent();
        _context = new AppDbContexto();
        MostrarLogin();
    }

    private void MostrarLogin()
    {
        NavBar.IsVisible = false;
        var loginView = new LoginView(_context);
        loginView.LoginExitoso += (nombre, rol) =>
        {
            _nombreUsuario = nombre;
            _rolActual = rol;
            TxtUsuarioSesion.Text = $"{nombre} [{rol}]";
            NavBar.IsVisible = true;

            // Restricción por rol: Informes y Proveedores solo para Administrador
            bool esAdmin = rol.Equals("Administrador", System.StringComparison.OrdinalIgnoreCase) || 
                           rol.Equals("Admin", System.StringComparison.OrdinalIgnoreCase);
            BtnNavInformes.IsVisible = esAdmin;
            BtnNavProveedores.IsVisible = esAdmin;

            // Vista por defecto tras el login
            MainContent.Content = new AgendaView(_context, _nombreUsuario);
        };
        MainContent.Content = loginView;
    }

    private void NavAgenda_Click(object? sender, RoutedEventArgs e) => MainContent.Content = new AgendaView(_context, _nombreUsuario);
    private void NavClientes_Click(object? sender, RoutedEventArgs e) => MainContent.Content = new ClientesView(_context);
    private void NavArticulos_Click(object? sender, RoutedEventArgs e) => MainContent.Content = new ArticulosView(_context);
    private void NavVentas_Click(object? sender, RoutedEventArgs e) => MainContent.Content = new VentasView(_context);
    private void NavProveedores_Click(object? sender, RoutedEventArgs e) => MainContent.Content = new ProveedoresView(_context);
    private void NavInformes_Click(object? sender, RoutedEventArgs e) => MainContent.Content = new InformesView(_context);
    private void NavLogout_Click(object? sender, RoutedEventArgs e) => MostrarLogin();
}