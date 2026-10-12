using MongoDB.Driver;
using ERP_Peluqueria.Modelos;

namespace ERP_Peluqueria.Datos;

public static class DbInitializer
{
    public static void Initialize(AppDbContexto context)
    {
        // 1. Usuarios (si ya existen, no sembrar datos iniciales)
        if (context.Usuarios.Find(_ => true).Any()) return;

        var admin = new Usuario
        {
            Nombre = "Gerente Principal",
            Username = "admin",
            PasswordHash = "admin123",
            Rol = "Admin"
        };
        var estilista = new Usuario
        {
            Nombre = "Lucía Estilista",
            Username = "lucia",
            PasswordHash = "lucia123",
            Rol = "Empleado"
        };
        context.Usuarios.InsertMany(new[] { admin, estilista });

        // 2. Clientes con Ficha Técnica Capilar
        var cliente1 = new Cliente
        {
            Nombre = "Marta Gómez",
            Telefono = "611223344",
            Email = "marta@email.com",
            HistorialTecnico = "Alergia a amoníaco. Tinte habitual: Base 6 + Ceniza (20 vol)."
        };
        var cliente2 = new Cliente
        {
            Nombre = "Carlos Pérez",
            Telefono = "655998877",
            Email = "carlos@email.com",
            HistorialTecnico = "Cuero cabelludo sensible. Degradado al 0.5 a tijera en zona superior."
        };
        context.Clientes.InsertMany(new[] { cliente1, cliente2 });

        // 3. Catálogo de Artículos
        var corte = new Articulo
        {
            Nombre = "Corte y Peinado",
            Tipo = "Servicio",
            Precio = 18.00m,
            DuracionMinutos = 30
        };
        var tinte = new Articulo
        {
            Nombre = "Tinte Completo + Hidratación",
            Tipo = "Servicio",
            Precio = 45.00m,
            DuracionMinutos = 90
        };
        var champu = new Articulo
        {
            Nombre = "Champú Reparador Argán 500ml",
            Tipo = "Producto",
            Precio = 16.50m,
            Stock = 20
        };
        var cera = new Articulo
        {
            Nombre = "Cera Mate Fijación Fuerte",
            Tipo = "Producto",
            Precio = 12.00m,
            Stock = 15
        };
        context.Items.InsertMany(new[] { corte, tinte, champu, cera });

        // 4. Proveedor
        var prov = new Proveedor
        {
            Nombre = "Cosméticos Profesionales L'Oréal",
            Contacto = "Javier Cano",
            Telefono = "913002211"
        };
        context.Proveedores.InsertOne(prov);

        // 5. Cita inicial usando Enum
        var citaEjemplo = new Cita
        {
            ClienteId = cliente1.Id,
            ClienteNombre = cliente1.Nombre,
            EmpleadoId = estilista.Id,
            EmpleadoNombre = estilista.Nombre,
            ItemId = corte.Id,
            ServicioNombre = corte.Nombre,
            DuracionMinutos = corte.DuracionMinutos,
            FechaHora = DateTime.Now.AddHours(2),
            Estado = EstadoCita.Pendiente
        };
        context.Citas.InsertOne(citaEjemplo);
    }
}