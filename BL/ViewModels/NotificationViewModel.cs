using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace BL.ViewModels
{
    public class NotificationViewModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; }

        [BsonElement("idevento")]
        public long IdEvento { get; set; }

        [BsonElement("idusuariosuscrito")]
        public long IdUsuarioSuscrito { get; set; }

        [BsonElement("tituloevento")]
        public string TituloEvento { get; set; } = null!;

        [BsonElement("inicioevento")]
        public string InicioEvento { get; set; } = null!;

        [BsonElement("estado")]
        public int Estado { get; set; }
    }
    public class MessageViewModel
    {
        public string Grupo { get; set; } = null!;
        public string Evento { get; set; } = null!;
        public string Inicio { get; set; } = null!;
    }
}
