using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Kaki.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Kaki.Controllers
{
    [Route("api/[controller]")]
    public class PomodoroController : Controller
    {
        public PomodoroController(IPomodoroProvider pomodoroProvider)
        {
            if (pomodoroProvider == null)
                throw new ArgumentNullException(nameof(pomodoroProvider));

            _PomodoroProvider = pomodoroProvider;
        }

        private readonly IPomodoroProvider _PomodoroProvider;

        [HttpGet]
        public IEnumerable<Pomodoro> GetAllPomodoros()
        {
            return _PomodoroProvider.GetAllPomodoros();
        }

        [HttpGet("{id}", Name = "GetPomodoro")]
        public IActionResult GetPomodoro(string id)
        {
            var pomodoro = _PomodoroProvider.GetPomodoro(id);

            if (pomodoro == null)
                return NotFound();

            return new ObjectResult(pomodoro);
        }

        [HttpPost]
        public IActionResult AddPomodoro([FromBody] Pomodoro pomodoro)
        {
            if (pomodoro == null)
                return BadRequest();

            _PomodoroProvider.AddPomodoro(pomodoro);

            return CreatedAtRoute("GetPomodoro", new { id = pomodoro.Key }, pomodoro);
        }

        [HttpPost("{id}/reset")]
        public IActionResult Reset(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var pomodoro = _PomodoroProvider.GetPomodoro(id);

            if (pomodoro == null)
                return NotFound();

            _PomodoroProvider.ResetPomodoro(id);

            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var pomodoro = _PomodoroProvider.GetPomodoro(id);

            if (pomodoro == null)
                return NotFound();

            _PomodoroProvider.RemovePomodoro(id);
            return new NoContentResult();
        }
    }
}
