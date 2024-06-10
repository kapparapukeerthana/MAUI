#if ANDROID
using Android.App;
using Android.Content;
using CommunityToolkit.Mvvm.Messaging;
#endif

namespace CargoMobile.Platforms;


[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter(new[] { "com.ndzl.DW" })]
public class DWIntentReceiver : BroadcastReceiver
{
    private static string SOURCE_TAG = "com.motorolasolutions.emdk.datawedge.source";
    private static string LABEL_TYPE_TAG = "com.motorolasolutions.emdk.datawedge.label_type";
    private static string DATA_STRING_TAG = "com.motorolasolutions.emdk.datawedge.data_string";
    public static string IntentAction = "barcodescanner.RECVR";
    public static string IntentCategory = "android.intent.category.DEFAULT";

    public event EventHandler<String> scanDataReceived;
    public override void OnReceive(Context context, Intent intent)
    {
        //System.Console.WriteLine("Here is DW on MAUI");
        if (intent.Extras != null)
        {
            String bc_type = intent.Extras.GetString("com.symbol.datawedge.label_type");
            String bc_data = intent.Extras.GetString("com.symbol.datawedge.data_string");

            WeakReferenceMessenger.Default.Send(bc_type + " " + bc_data);
        }
        var Out = "Scanned Text";
        if (scanDataReceived != null)
        {
            scanDataReceived(this, Out);
        }
    }
}
