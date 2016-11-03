using System.Collections.Generic;

namespace Kaki.Models
{
    public interface IPomodoroProvider
    {
        void AddPomodoro(Pomodoro pomodoro);
        Pomodoro GetPomodoro(string key);
        IEnumerable<Pomodoro> GetAllPomodoros();
        void ResetPomodoro(string key);
        void RemovePomodoro(string key);
    }
}
