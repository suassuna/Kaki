using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Kaki.Models
{
    public class PomodoroInMemoryProvider : IPomodoroProvider
    {
        public PomodoroInMemoryProvider()
        {
            _Pomodoros = new ConcurrentDictionary<string, Pomodoro>();
            AddPomodoro(new Pomodoro("kaki", DateTime.UtcNow));
        }

        private ConcurrentDictionary<string, Pomodoro> _Pomodoros;

        public IEnumerable<Pomodoro> GetAllPomodoros()
        {
            return _Pomodoros.Values;
        }

        public Pomodoro GetPomodoro(string key)
        {
            Pomodoro pomodoro;

            _Pomodoros.TryGetValue(key, out pomodoro);

            return pomodoro;
        }

        public void AddPomodoro(Pomodoro pomodoro)
        {
            if (pomodoro == null)
                throw new ArgumentNullException(nameof(pomodoro));

            pomodoro.Key = Guid.NewGuid().ToString();

            _Pomodoros[pomodoro.Key] = pomodoro;
        }

        public void ResetPomodoro(string key)
        {
            _Pomodoros[key].StartTime = DateTime.UtcNow;
        }

        public void RemovePomodoro(string key)
        {
            Pomodoro pomodoro;

            _Pomodoros.TryRemove(key, out pomodoro);
        }
    }
}
