using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Android.App;

namespace miA
{
    public class Utilidades
    {

        static string directorioBase;
        public static string strangeFormElement = "jf74f92#$%&1hzawa2";

        static Utilidades()
        {
            directorioBase = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/proximus.miagenda";
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


        public static string TraerRutaBd()
        {
            string ruta = TraerRuta(Sha1Hash(Datos.idUsuario));
            return ruta;

        }

        public static string TraerRuta(string filename)
        {
            if (!System.IO.File.Exists(directorioBase))
            {
                System.IO.Directory.CreateDirectory(directorioBase);
            }
            return directorioBase + "/" + filename;
        }


    }



    public static class Crypto
    {
        private static readonly byte[] IVa = new byte[] { 0x0b, 0x0c, 0x0d, 0x0e, 0x0f, 0x11, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17 };


        public static string Encrypt(this string text, string salt)
        {
            try
            {
                using (Aes aes = new AesManaged())
                {
                    Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetString(IVa, 0, IVa.Length), Encoding.UTF8.GetBytes(salt));
                    aes.Key = deriveBytes.GetBytes(128 / 8);
                    aes.IV = aes.Key;
                    using (MemoryStream encryptionStream = new MemoryStream())
                    {
                        using (CryptoStream encrypt = new CryptoStream(encryptionStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            byte[] cleanText = Encoding.UTF8.GetBytes(text);

                            encrypt.Write(cleanText, 0, cleanText.Length);
                            encrypt.FlushFinalBlock();
                        }

                        byte[] encryptedData = encryptionStream.ToArray();
                        string encryptedText = Convert.ToBase64String(encryptedData);

                        return encryptedText;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return String.Empty;
            }
        }

        public static string Decrypt(this string text, string salt)
        {

            using (Aes aes = new AesManaged())
            {
                Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(Encoding.UTF8.GetString(IVa, 0, IVa.Length), Encoding.UTF8.GetBytes(salt));
                aes.Key = deriveBytes.GetBytes(128 / 8);
                aes.IV = aes.Key;

                using (MemoryStream decryptionStream = new MemoryStream())
                {
                    using (CryptoStream decrypt = new CryptoStream(decryptionStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        byte[] encryptedData = Convert.FromBase64String(text);

                        decrypt.Write(encryptedData, 0, encryptedData.Length);
                        decrypt.Flush();
                    }

                    byte[] decryptedData = decryptionStream.ToArray();
                    string decryptedText = Encoding.UTF8.GetString(decryptedData, 0, decryptedData.Length);

                    return decryptedText;
                }
            }

        }

    }

    public abstract class ConfirmedActivity : Activity
    {

        public abstract void PositiveConfirm();
        public abstract void NegativeConfirm();
    }

}
