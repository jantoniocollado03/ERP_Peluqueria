using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using MongoDB.Driver;
using ERP_Peluqueria.Datos;

namespace ERP_Peluqueria.Pages;

public partial class LoginView : UserControl
{
    private readonly AppDbContexto _context;
    public event Action<string, string>? LoginExitoso; // Notifica al contenedor principal

    public LoginView(AppDbContexto context)
    {
        InitializeComponent();
        _context = context;
    }

    private async void BtnLogin_Click(object? sender, RoutedEventArgs e)
    {
        string username = UsernameBox.Text?.Trim() ?? string.Empty;
        string password = PasswordBox.Text ?? string.Empty;

        var user = await _context.Usuarios
            .Find(u => u.Username == username && u.PasswordHash == password)
            .FirstOrDefaultAsync();

        if (user == null)
        {
            ErrorMessageText.Text = "Usuario o contraseña inválidos.";
            ErrorBorder.IsVisible = true;
            return;
        }

        ErrorBorder.IsVisible = false;
        LoginExitoso?.Invoke(user.Nombre, user.Rol);
    }
}