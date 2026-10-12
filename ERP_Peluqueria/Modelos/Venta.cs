using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ERP_Peluqueria.Modelos;

public class Venta
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string ClienteId { get; set; } = string.Empty;
    public string ClienteNombre { get; set; } = string.Empty;

    public DateTime Fecha { get; set; } = DateTime.Now;
    public decimal BaseImponible { get; set; }
    public decimal Iva { get; set; }
    public decimal Total { get; set; }
    public List<LineaVenta> Lineas { get; set; } = new();
}