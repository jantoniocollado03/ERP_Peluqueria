using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ERP_Peluqueria.Modelos;

public class Cita
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string ClienteId { get; set; } = string.Empty;
    public string ClienteNombre { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string EmpleadoId { get; set; } = string.Empty;
    public string EmpleadoNombre { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string ItemId { get; set; } = string.Empty;
    public string ServicioNombre { get; set; } = string.Empty;
    public int DuracionMinutos { get; set; } = 30;

    public DateTime FechaHora { get; set; }

    [BsonRepresentation(BsonType.String)]
    public EstadoCita Estado { get; set; } = EstadoCita.Pendiente;
}