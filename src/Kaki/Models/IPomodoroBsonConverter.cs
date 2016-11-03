using MongoDB.Bson;

namespace Kaki.Models
{
    public interface IPomodoroBsonConverter
    {
        Pomodoro ConvertToPomodoro(BsonDocument document);
        BsonDocument ConvertToBsonDocument(Pomodoro pomodoro);
    }
}