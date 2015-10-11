using System;
using System.Collections.Generic;
using System.Linq;

namespace Enrampage.Models
{
    public class RantResponse
    {
        public string Created;
        public string Text;
        public List<string> Tags = new List<string>();

        public static RantResponse FromRant(Rant Rant)
        {
            var rant = new RantResponse()
            {
                Text = Rant.Text,
            };

            var timeElapsed = DateTime.Now - Rant.Created;

            if (timeElapsed.Days >= 365)
            {
                rant.Created = string.Format("{0} years ago", Math.Floor(timeElapsed.Days / 365.0));
            }
            else if (timeElapsed.Days >= 1)
            {
                rant.Created = string.Format("{0} days ago", timeElapsed.Days);
            }
            else if (timeElapsed.Hours >= 1)
            {
                rant.Created = string.Format("{0} hours ago", timeElapsed.Hours);
            }
            else if (timeElapsed.Minutes >= 1)
            {
                rant.Created = string.Format("{0} minutes ago", timeElapsed.Minutes);
            }
            else
            {
                rant.Created = string.Format("{0} seconds ago", timeElapsed.Seconds);
            }

            rant.Tags.AddRange(Rant.Tags.Select(x => x.Text));

            return rant;
        }

        private RantResponse() { }
    }
}