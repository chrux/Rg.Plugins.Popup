using System;
using CoreGraphics;
using Demo.Controls;
using Demo.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(EnhancedEntry), typeof(EnhancedEntryRenderer))]
namespace Demo.iOS.Renderers
{
    public class EnhancedEntryRenderer : EntryRenderer
    {
		UIToolbar toolbar;

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            var baseEntry = (EnhancedEntry)this.Element;
            base.OnElementChanged(e);

            Control.ShouldReturn += field =>
            {
                baseEntry.EntryActionFired();
                return true;
            };

			toolbar = new UIToolbar(new CGRect(0.0f, 0.0f, Control.Frame.Size.Width, 44.0f));
            var barButtonSystemItem = UIBarButtonSystemItem.Done;

            toolbar.Items = new[]
            {
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                new UIBarButtonItem(barButtonSystemItem, delegate { Control.ResignFirstResponder(); })
            };

			this.Control.InputAccessoryView = toolbar;
        }
    }
}
