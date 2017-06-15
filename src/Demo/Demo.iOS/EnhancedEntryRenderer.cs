using System;
using Demo.Controls;
using Demo.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(EnhancedEntry), typeof(EnhancedEntryRenderer))]
namespace Demo.iOS
{
    public class EnhancedEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            var baseEntry = (EnhancedEntry)this.Element;
            base.OnElementChanged(e);

            Control.ShouldReturn += field =>
            {
                baseEntry.EntryActionFired();
                return true;
            };
        }
    }
}
