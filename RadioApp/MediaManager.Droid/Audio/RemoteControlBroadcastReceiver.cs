using Android.Content;
using Android.Support.V4.Content;
using Android.Views;

namespace Plugin.MediaManager
{
	[BroadcastReceiver]
	[Android.App.IntentFilter(new []{Intent.ActionMediaButton})]
	public class RemoteControlBroadcastReceiver : BroadcastReceiver
	{

		/// <summary>
		/// gets the class name for the component
		/// </summary>
		/// <value>The name of the component.</value>
		public string ComponentName => Class.Name;

	    /// <Docs>The Context in which the receiver is running.</Docs>
		/// <summary>
		/// When we receive the action media button intent
		/// parse the key event and tell our service what to do.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="intent">Intent.</param>
		public override void OnReceive (Context context, Intent intent)
		{
			if (intent.Action != Intent.ActionMediaButton)
				return;

			//The event will fire twice, up and down.
			// we only want to handle the down event though.
			var key = (KeyEvent) intent.GetParcelableExtra(Intent.ExtraKeyEvent);
			if (key.Action != KeyEventActions.Down)
				return;
	
            var action = MediaServiceBase.ActionPlay;

			switch (key.KeyCode) {
				case Keycode.Headsethook:
				case Keycode.MediaPlayPause:
                action = MediaServiceBase.ActionTogglePlayback;
					break;
				case Keycode.MediaPlay:
                action = MediaServiceBase.ActionPlay;
					break;
				case Keycode.MediaPause:
                action = MediaServiceBase.ActionPause;
					break;
				case Keycode.MediaStop:
                action = MediaServiceBase.ActionStop;
					break;
				case Keycode.MediaNext:
                action = MediaServiceBase.ActionNext;
					break;
				case Keycode.MediaPrevious:
                action = MediaServiceBase.ActionPrevious;
					break;
				default:
					return;
			}

			var remoteIntent = new Intent(action);
            ContextCompat.StartForegroundService(context, remoteIntent);
		}
	}
}

