using Android.Content;
using Android.OS;
using Plugin.MediaManager.Audio;

namespace Plugin.MediaManager
{
    internal class MediaPlayerServiceConnection : Java.Lang.Object, IServiceConnection
    {
        private AudioPlayerImplementation instance;

        public MediaPlayerServiceConnection(AudioPlayerImplementation mediaPlayer)
        {
            this.instance = mediaPlayer;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            if (service is MediaServiceBinder mediaServiceBinder)
            {
               instance.OnServiceConnected(mediaServiceBinder);
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            instance.OnServiceDisconnected();
        }
    }
}