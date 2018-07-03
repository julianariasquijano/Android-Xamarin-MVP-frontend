using System;
using Android.App;

namespace miA
{
    public class Utilidades
    {
        public Utilidades()
        {
        }

        public static void showMessage(Activity activity,string title,string message, string type){

            var dialog = new AlertDialog.Builder(activity);
            dialog.SetMessage(message);
            if (type.ToLower()=="ok")
            {
                dialog.SetNeutralButton("OK", delegate { });
            }

            dialog.Show();

        }

    }
}
