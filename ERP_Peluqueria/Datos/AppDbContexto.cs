using MongoDB.Driver;
using ERP_Peluqueria.Modelos;

namespace ERP_Peluqueria.Datos;

public class AppDbContexto
{
    private readonly IMongoDatabase _database;

    public AppDbContexto(string connectionString = "mongodb://localhost:27017")
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase("PeluqueriaERP");
    }
    public IMongoCollection<Usuario> Usuarios => _database.GetCollection<Usuario>("Usuarios");
    public IMongoCollection<Cliente> Clientes => _database.GetCollection<Cliente>("Clientes");
    public IMongoCollection<Articulo> Items => _database.GetCollection<Articulo>("Items");
    public IMongoCollection<Cita> Citas => _database.GetCollection<Cita>("Citas");
    public IMongoCollection<Proveedor> Proveedores => _database.GetCollection<Proveedor>("Proveedores");
    public IMongoCollection<Venta> Ventas => _database.GetCollection<Venta>("Ventas");
}