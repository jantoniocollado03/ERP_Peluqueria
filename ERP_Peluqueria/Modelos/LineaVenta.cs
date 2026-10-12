using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ERP_Peluqueria.Modelos;

public class LineaVenta
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string ItemId { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}