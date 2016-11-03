using MongoDB.Bson;

namespace Kaki.Models
{
    public class PomodoroBsonConverter : IPomodoroBsonConverter
    {
        private const string _NAME = "name";
        private const string _START_TIME = "startTime";
        private const string _WORKING_TIMESPAN = "workingTimeSpan";
        private const string _TINY_BREAK_TIMESPAN = "tinyBreakTimeSpan";
        private const string _EPIC_BREAK_TIMESPAN = "epicBreakTimeSpan";
        private const string _ID = "_id";

        public BsonDocument ConvertToBsonDocument(Pomodoro pomodoro)
        {
            return new BsonDocument
            {
                { _NAME , pomodoro.Name },
                { _START_TIME, pomodoro.StartTime },
                { _WORKING_TIMESPAN, pomodoro.WorkingTimeSpan },
                { _TINY_BREAK_TIMESPAN, pomodoro.TinyBreakTimeSpan },
                { _EPIC_BREAK_TIMESPAN, pomodoro.EpicBreakTimeSpan }
            };
        }

        public Pomodoro ConvertToPomodoro(BsonDocument document)
        {
            var pomodoro = new Pomodoro(document[_NAME].AsString,
                document[_START_TIME].ToUniversalTime(),
                document[_WORKING_TIMESPAN].AsInt32,
                document[_TINY_BREAK_TIMESPAN].AsInt32,
                document[_EPIC_BREAK_TIMESPAN].AsInt32);

            pomodoro.Key = document[_ID].AsObjectId.ToString();

            return pomodoro;
        }
    }
}
