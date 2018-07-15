using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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


        public static Task<string> DisplayCustomDialog1(Android.Content.Context context,string dialogTitle, string dialogMessage, string dialogPositiveBtnLabel, string dialogNegativeBtnLabel)
        {
            var tcs = new TaskCompletionSource<string>();

            AlertDialog.Builder alert = new AlertDialog.Builder(context);
            alert.SetTitle(dialogTitle);
            alert.SetMessage(dialogMessage);
            alert.SetPositiveButton(dialogPositiveBtnLabel, (senderAlert, args) => {
                tcs.SetResult(dialogPositiveBtnLabel);
            });

            alert.SetNegativeButton(dialogNegativeBtnLabel, (senderAlert, args) => {
                tcs.SetResult(dialogNegativeBtnLabel);
            });

            Dialog dialog = alert.Create();
            dialog.Show();

            return tcs.Task;
        }


        public static void ConfirmDialog(ConfirmedActivity activity, string dialogTitle, string dialogMessage, string dialogPositiveBtnLabel, string dialogNegativeBtnLabel)
        {


            AlertDialog.Builder alert = new AlertDialog.Builder((Android.Content.Context)activity);
            alert.SetTitle(dialogTitle);
            alert.SetMessage(dialogMessage);
            alert.SetPositiveButton(dialogPositiveBtnLabel, (senderAlert, args) => {
                activity.PositiveConfirm();
            });

            alert.SetNegativeButton(dialogNegativeBtnLabel, (senderAlert, args) => {
                activity.NegativeConfirm();
            });

            Dialog dialog = alert.Create();
            dialog.Show();


        }



        public static bool EsCorreoElectronico(string correo)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(correo);
                return addr.Address == correo;
            }
            catch
            {
                return false;
            }
        }

        public static string Sha1Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }

        public static string getFormattedHour(string hour, string minute)
        {
            if (hour.Length == 1) hour = "0" + hour;
            if (minute.Length == 1) minute = "0" + minute;
            return  hour + ":" + minute;
        }

        public static string getFormattedDate(string year,string month, string day)
        {
            if (month.Length==1)month = "0" + month;
            if (day.Length == 1) day = "0" + day;
            return year + "-" + month + "-" + day;
        }

        public static string dateCheckCero(string number)
        {
            if (number.Length == 1) return number = "0" + number; else return number;
        }

    }


    public abstract class ConfirmedActivity : Activity
    {

        public abstract void PositiveConfirm();
        public abstract void NegativeConfirm();
    }

}
