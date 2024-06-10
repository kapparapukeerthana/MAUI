using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using CargoMobile.Platforms;
using CommunityToolkit.Mvvm.Messaging;

namespace Webview
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode =LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        //protected override void OnCreate(Bundle savedInstanceState) {
        private DWIntentReceiver _broadcastReceiver = null;
        private static string ACTION_DATAWEDGE_FROM_6_2 = "com.symbol.datawedge.api.ACTION";
        private static string EXTRA_CREATE_PROFILE = "com.symbol.datawedge.api.CREATE_PROFILE";
        private static string EXTRA_SET_CONFIG = "com.symbol.datawedge.api.SET_CONFIG";
        private static string EXTRA_PROFILE_NAME = "Inventory DEMO";
        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            RegisterReceivers();
            CreateProfile();
            _broadcastReceiver = new DWIntentReceiver();
            _broadcastReceiver.scanDataReceived += (s, scanData) =>
            {
                WeakReferenceMessenger.Default.Send("22");
                //MessagingCenter.Send<App, string>(app, "ScanBarcode", scanData);
            };

            WeakReferenceMessenger.Default.Register<string>(this, (r, li) =>
            {
                MainThread.BeginInvokeOnMainThread(() => {
                    if (li == "11")
                    {
                        Intent i = new Intent();
                        i.SetAction("com.symbol.datawedge.api.ACTION");
                        i.PutExtra("com.symbol.datawedge.api.SCANNER_INPUT_PLUGIN", "DISABLE_PLUGIN");
                        i.PutExtra("SEND_RESULT", "true");
                        i.PutExtra("COMMAND_IDENTIFIER", "MY_DISABLE_SCANNER");  //Unique identifier
                        this.SendBroadcast(i);
                    }
                    else if (li == "22")
                    {
                        Intent i = new Intent();
                        i.SetAction("com.symbol.datawedge.api.ACTION");
                        i.PutExtra("com.symbol.datawedge.api.SCANNER_INPUT_PLUGIN", "ENABLE_PLUGIN");
                        i.PutExtra("SEND_RESULT", "true");
                        i.PutExtra("COMMAND_IDENTIFIER", "MY_ENABLE_SCANNER");  //Unique identifier
                        this.SendBroadcast(i);
                    }

                });
            });
            //showing saved states 
            try
            {
                string savedDatetime = savedInstanceState.GetString("time");
                if (savedDatetime is not null)
                    WeakReferenceMessenger.Default.Send("Saved DateTime=" + savedDatetime);

            }
            catch (System.Exception ex)
            {
                WeakReferenceMessenger.Default.Send("No previously saved instance available");
            }
        }


        protected override void OnSaveInstanceState(Bundle outState)
        {
            string currentDatetime = DateTime.Now.ToString();
            outState.PutString("time", currentDatetime);
            base.OnSaveInstanceState(outState);
        }

        void RegisterReceivers()
        {
            IntentFilter filter = new IntentFilter();
            filter.AddCategory("android.intent.category.DEFAULT");
            filter.AddAction("com.ndzl.DW");
            filter.AddAction("com.zebra.sensors");

            Intent regres = RegisterReceiver(new DWIntentReceiver(), filter);
        }
        private void CreateProfile()
        {
            String profileName = EXTRA_PROFILE_NAME;
            SendDataWedgeIntentWithExtra(ACTION_DATAWEDGE_FROM_6_2, EXTRA_CREATE_PROFILE, profileName);

            //  Now configure that created profile to apply to our application
            Bundle profileConfig = new Bundle();
            profileConfig.PutString("PROFILE_NAME", EXTRA_PROFILE_NAME);
            profileConfig.PutString("PROFILE_ENABLED", "true"); //  Seems these are all strings
            profileConfig.PutString("CONFIG_MODE", "UPDATE");
            Bundle barcodeConfig = new Bundle();
            barcodeConfig.PutString("PLUGIN_NAME", "BARCODE");
            barcodeConfig.PutString("RESET_CONFIG", "true"); //  This is the default but never hurts to specify
            Bundle barcodeProps = new Bundle();
            barcodeConfig.PutBundle("PARAM_LIST", barcodeProps);
            profileConfig.PutBundle("PLUGIN_CONFIG", barcodeConfig);
            Bundle appConfig = new Bundle();
            appConfig.PutString("PACKAGE_NAME", this.PackageName);      //  Associate the profile with this app
            appConfig.PutStringArray("ACTIVITY_LIST", new String[] { "*" });
            profileConfig.PutParcelableArray("APP_LIST", new Bundle[] { appConfig });
            SendDataWedgeIntentWithExtra(ACTION_DATAWEDGE_FROM_6_2, EXTRA_SET_CONFIG, profileConfig);
            //  You can only configure one plugin at a time, we have done the barcode input, now do the intent output
            profileConfig.Remove("PLUGIN_CONFIG");
            Bundle intentConfig = new Bundle();
            intentConfig.PutString("PLUGIN_NAME", "INTENT");
            intentConfig.PutString("RESET_CONFIG", "true");
            Bundle intentProps = new Bundle();
            intentProps.PutString("intent_output_enabled", "true");
            intentProps.PutString("intent_action", DWIntentReceiver.IntentAction);
            intentProps.PutString("intent_delivery", "2");
            intentConfig.PutBundle("PARAM_LIST", intentProps);
            profileConfig.PutBundle("PLUGIN_CONFIG", intentConfig);
            SendDataWedgeIntentWithExtra(ACTION_DATAWEDGE_FROM_6_2, EXTRA_SET_CONFIG, profileConfig);
        }
        private void SendDataWedgeIntentWithExtra(String action, String extraKey, Bundle extras)
        {
            Intent dwIntent = new Intent();
            dwIntent.SetAction(action);
            dwIntent.PutExtra(extraKey, extras);
            SendBroadcast(dwIntent);
        }

        private void SendDataWedgeIntentWithExtra(String action, String extraKey, String extraValue)
        {
            Intent dwIntent = new Intent();
            dwIntent.SetAction(action);
            dwIntent.PutExtra(extraKey, extraValue);
            SendBroadcast(dwIntent);
        }
    }
}
