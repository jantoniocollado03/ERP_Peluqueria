using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ERP_Peluqueria.Modelos;

public class Articulo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = "Servicio"; // "Servicio" o "Producto"
    public decimal Precio { get; set; }
    public int Stock { get; set; } = 0;
    public int DuracionMinutos { get; set; } = 30;
}