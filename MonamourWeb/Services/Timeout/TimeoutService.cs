using System;
using System.Timers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MonamourWeb.Services.Timeout
{
    public class TimeoutService
    {
        public static TimeoutService Instance=> new TimeoutService();

        private Timer _timer;

        private TimeoutService()
        {
            _timer = new Timer(TimeSpan.FromMinutes(3).TotalMilliseconds);
            _timer.Enabled = true;
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
           // RedirectToActionResult()
        }

        public void Start()
        {

        }
    }
}