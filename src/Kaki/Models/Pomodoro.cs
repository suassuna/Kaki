using System;
using System.Collections.Generic;
using System.Linq;

namespace Kaki.Models
{
    public class Pomodoro
    {
        public Pomodoro(string name, DateTime startTime, int workingTimeSpan = 25, int tinyBreakTimeSpan = 5, int epicBreakTimeSpan = 10)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (startTime.Equals(DateTime.MinValue) || startTime.Equals(DateTime.MaxValue))
                throw new ArgumentOutOfRangeException(nameof(startTime));
            if (workingTimeSpan < 0)
                throw new ArgumentOutOfRangeException(nameof(workingTimeSpan));
            if (tinyBreakTimeSpan < 0)
                throw new ArgumentOutOfRangeException(nameof(tinyBreakTimeSpan));
            if (epicBreakTimeSpan < 0)
                throw new ArgumentOutOfRangeException(nameof(epicBreakTimeSpan));

            Name = name;
            StartTime = startTime;
            WorkingTimeSpan = workingTimeSpan == 0 ? 25 : workingTimeSpan;
            TinyBreakTimeSpan = tinyBreakTimeSpan == 0 ? 5 : tinyBreakTimeSpan;
            EpicBreakTimeSpan = epicBreakTimeSpan == 0 ? 10 : epicBreakTimeSpan;
        }

        /// <summary>
        /// Pomodoro's key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Pomodoro's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Pomodoro's start time
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Working time in minutes
        /// </summary>
        public int WorkingTimeSpan { get; set; }

        /// <summary>
        /// short break in minutes
        /// </summary>
        public int TinyBreakTimeSpan { get; set; }

        /// <summary>
        /// long break in minutes
        /// </summary>
        public int EpicBreakTimeSpan { get; set; }

        /// <summary>
        /// Current pomodoro cycle information
        /// </summary>
        public PomodoroCycle CurrentCycle
        {
            get { return _GetCurrentCycle(DateTime.UtcNow); }
        }

        private PomodoroCycle _GetCurrentCycle(DateTime referenceTime)
        {
            var currentCycleTimeSpan = _GetCurrentCycleTimeSpan(referenceTime);

            var currentCycleStart = referenceTime.Subtract(currentCycleTimeSpan);

            var cycles = _GetPomodoroCycles(currentCycleStart);

            return cycles.FirstOrDefault(c => referenceTime.CompareTo(c.EndTime) < 0);
        }

        private TimeSpan _GetCurrentCycleTimeSpan(DateTime referenceDate)
        {
            var totalTimeSpan = referenceDate - StartTime;
            var cycleTotalTimeSpan = _GetCycleTotalTimeSpan();

            return TimeSpan.FromMilliseconds(totalTimeSpan.TotalMilliseconds % cycleTotalTimeSpan.TotalMilliseconds);
        }

        private TimeSpan _GetCycleTotalTimeSpan()
        {
            return new TimeSpan(0, (4 * WorkingTimeSpan) + (3 * TinyBreakTimeSpan) + EpicBreakTimeSpan, 0);
        }

        private IEnumerable<PomodoroCycle> _GetPomodoroCycles(DateTime cycleStart)
        {
            var totalMinutesSpan = 0;

            var cycles = new List<PomodoroCycle>();

            for(int i = 0; i < 3; ++i)
            {
                totalMinutesSpan += WorkingTimeSpan;
                cycles.Add(new PomodoroCycle() { Status = EPomodoroStatus.WorkingTime, EndTime = cycleStart.AddMinutes(totalMinutesSpan) });
                totalMinutesSpan += TinyBreakTimeSpan;
                cycles.Add(new PomodoroCycle() { Status = EPomodoroStatus.TinyBreak, EndTime = cycleStart.AddMinutes(totalMinutesSpan) });
            }
            
            totalMinutesSpan += WorkingTimeSpan;
            cycles.Add(new PomodoroCycle() { Status = EPomodoroStatus.WorkingTime, EndTime = cycleStart.AddMinutes(totalMinutesSpan) });
            totalMinutesSpan += EpicBreakTimeSpan;
            cycles.Add(new PomodoroCycle() { Status = EPomodoroStatus.EpicBreak, EndTime = cycleStart.AddMinutes(totalMinutesSpan) });

            return cycles;
        }

        public class PomodoroCycle
        {
            public EPomodoroStatus Status;
            public DateTime EndTime;
        }
    }
}
