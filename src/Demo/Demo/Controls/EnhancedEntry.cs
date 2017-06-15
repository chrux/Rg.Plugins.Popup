using System;
using Xamarin.Forms;

namespace Demo.Controls
{
    public class EnhancedEntry: Entry
    {
        public event EventHandler EventTriggered;
        public Entry Next { get; set; }

        public EnhancedEntry()
        {
            EventTriggered += Goto;
        }

        private static void Goto(object sender, EventArgs e)
        {
	        var next = ((EnhancedEntry)sender)?.Next;

	        if (next != null)
	        {
		        next.Focus();
	        }
        }

        public void EntryActionFired()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                EventTriggered?.Invoke(this, null);
            });
        }
    }
}
