using System;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using Rg.Plugins.Popup.IOS.Renderers;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Size = Xamarin.Forms.Size;

[assembly: ExportRenderer(typeof(PopupPage), typeof(PopupPageRenderer))]
namespace Rg.Plugins.Popup.IOS.Renderers
{
	[Preserve(AllMembers = true)]
	public class PopupPageRenderer : PageRenderer
	{
        private CGRect _keyboardBounds;
        private bool shouldHideKeyboard = true;
		private PopupPage _element
		{
			get { return (PopupPage)Element; }
		}

		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);

			var tapGesture = new UITapGestureRecognizer(OnTap)
			{
				CancelsTouchesInView = false
			};

			if (e.NewElement != null)
			{
				ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
				ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;

				View.AddGestureRecognizer(tapGesture);
			}
			if (e.OldElement != null)
			{
				View.RemoveGestureRecognizer(tapGesture);
			}
		}

		private void OnTap(UITapGestureRecognizer e)
		{
			var view = e.View;
			var location = e.LocationInView(view);
			var subview = view.HitTest(location, null);
			if (subview == view)
			{
				_element.SendBackgroundClick();
			}
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();
			SetElementSize(new Size(View.Bounds.Width, View.Bounds.Height));
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillChangeFrameNotification, KeyBoardUpNotification);
			NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, KeyBoardDownNotification);
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			NSNotificationCenter.DefaultCenter.RemoveObserver(this);

			var isNotRemoved = PopupNavigation.PopupStack.Any(e => e == _element);

			// Close all open pages the Popup, if the main page, on which opened PresentViewControllerAsync, destroyed.
			if (isNotRemoved)
			{
				RemoveThisPageFromStack();
			}
		}

		private void KeyBoardUpNotification(NSNotification notifi)
        {
			if (shouldHideKeyboard)
			{
				shouldHideKeyboard = false;
			}

            _keyboardBounds = UIKeyboard.BoundsFromNotification(notifi);
			// With this piece of code we make sure if user uses a external
			// keyboard the space is not left blank
			//// get the frame end user info key
			var kbEndFrame = (notifi.UserInfo.ObjectForKey(UIKeyboard.FrameEndUserInfoKey) as NSValue).CGRectValue;
            //// calculate the visible portion of the keyboard on the screen
            System.Diagnostics.Debug.WriteLine(_keyboardBounds.Height);
            var newKbHeight = UIScreen.MainScreen.Bounds.Height - kbEndFrame.Y;
            if (newKbHeight != 0) {
                _keyboardBounds.Height = newKbHeight;
            }
            //_keyboardBounds.Height = UIScreen.MainScreen.Bounds.Height - kbEndFrame.Y;
            System.Diagnostics.Debug.WriteLine("--" + _keyboardBounds.Height);

			UpdateElementSize();
		}

		private void KeyBoardDownNotification(NSNotification notifi)
		{
            System.Diagnostics.Debug.WriteLine("KeyBoardDownNotification");
            shouldHideKeyboard = true;
			Task.Run(async () =>
			{
				await Task.Delay(TimeSpan.FromMilliseconds(62.5));
				if (!shouldHideKeyboard)
				{
					return;
				}
				_keyboardBounds = CGRect.Empty;

				Device.BeginInvokeOnMainThread(() =>
				{
					UpdateElementSize();
				});
			});
		}

		private void UpdateElementSize()
		{
			if (View?.Superview == null)
				return;

			var bound = View.Superview.Bounds;

			SetElementSize(new Size(bound.Width, bound.Height - _keyboardBounds.Height));
		}

		private async void RemoveThisPageFromStack()
		{
			await PopupNavigation.RemovePageAsync(_element, false);
		}
	}
}