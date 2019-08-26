using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using FFImageLoading.Forms.Platform;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.MediaManager;
using Plugin.MediaManager.ExoPlayer;
using Xamarin.Forms;

namespace RadioApp.Droid
{
    [Activity(Label = "RadioApp", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            AppCenter.Start("16514814-0822-4179-8cf9-0633b11cd8a3", typeof(Analytics), typeof(Crashes));
            AppCenter.Start("16514814-0822-4179-8cf9-0633b11cd8a3", typeof(Analytics), typeof(Crashes));

            CachedImageRenderer.Init(enableFastRenderer: true);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            CrossMediaManager.Current = new MediaManagerImplementation();

            AddAutoStartup();

            LoadApplication(new App());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StopService(new Intent(this, typeof(ExoPlayerAudioService)));
        }

        private void AddAutoStartup()
        {

            try
            {
                Intent intent = new Intent();
                String manufacturer = Android.OS.Build.Manufacturer;
                if ("xiaomi".Equals(manufacturer, StringComparison.OrdinalIgnoreCase))
                {
                    Intent.SetComponent(new ComponentName("com.miui.securitycenter", "com.miui.permcenter.autostart.AutoStartManagementActivity"));
                }
                else if ("oppo".Equals(manufacturer, StringComparison.OrdinalIgnoreCase))
                {
                    Intent.SetComponent(new ComponentName("com.coloros.safecenter", "com.coloros.safecenter.permission.startup.StartupAppListActivity"));
                }
                else if ("vivo".Equals(manufacturer, StringComparison.OrdinalIgnoreCase))
                {
                    Intent.SetComponent(new ComponentName("com.vivo.permissionmanager", "com.vivo.permissionmanager.activity.BgStartUpManagerActivity"));
                }
                else if ("Letv".Equals(manufacturer, StringComparison.OrdinalIgnoreCase))
                {
                    Intent.SetComponent(new ComponentName("com.letv.android.letvsafe", "com.letv.android.letvsafe.AutobootManageActivity"));
                }
                else if ("Honor".Equals(manufacturer, StringComparison.OrdinalIgnoreCase))
                {
                    Intent.SetComponent(new ComponentName("com.huawei.systemmanager", "com.huawei.systemmanager.optimize.process.ProtectActivity"));
                }

                var list = PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
                if (list.Count > 0)
                {
                    StartActivity(intent);
                }
            }
            catch (Exception e)
            {
                //Log.e("exc", String.valueOf(e));
            }
        }
    }
}