using System.Collections.Generic;
using System.Json;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace miA
{
    [Activity(Label = "Recordar Contraseña")]
    public class RememberPassword : Activity
    {

        string mail;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mail = Intent.GetStringExtra("mail").ToLower().Trim();
            SetContentView(Resource.Layout.RememberPassword);


            var backButton = FindViewById<ImageButton>(Resource.Id.backButton);
            backButton.Click += (sender, e) => {
                this.Finish();
                OverridePendingTransition(0, 0);
            };

            var sendCodeButton = FindViewById<Button>(Resource.Id.sendCodeButton);

            sendCodeButton.Click += (sender, e) => {

                JsonValue resultado = Datos.LlamarWsSync(Datos.registerAndAuthenticationWebServiceUrl + "resetPassword1", mail);

                if ((string)resultado["status"] == "OK" && (string)resultado["mensaje"] == "")
                {
                    Utilidades.showMessage(this, "Antención", "Hemos enviado un código a tu correo electrónico.", "OK");
                }
                else
                {
                    Utilidades.showMessage(this, "Antención", (string)resultado["mensaje"], "OK");

                }

            };

            var rememberContinueButton = FindViewById<Button>(Resource.Id.rememberContinueButton);

            rememberContinueButton.Click += (sender, e) => {

                string resultadoValidacion = ValidarFormulario();

                if ( resultadoValidacion == "")
                {
                    var receivedCode = FindViewById<EditText>(Resource.Id.receivedCode);
                    var password = FindViewById<EditText>(Resource.Id.rememberPassword);
                    var passwordCheck = FindViewById<EditText>(Resource.Id.rememberPasswordCheck);

                    var data = new Dictionary<string, string>
                    {
                        ["receivedCode"] = receivedCode.Text,
                        ["password"] = Utilidades.Sha1Hash(password.Text),
                        ["mail"] = mail
                    };

                    JsonValue resultado = Datos.ResetPassword2(data);

                    if ((string)resultado["status"] == "OK")
                    {

                        if ((string)resultado["token"] != "")
                        {
                            Datos.idUsuario = mail;
                            Datos.token = (string)resultado["token"];
                            var preferencias = Application.Context.GetSharedPreferences("miax", FileCreationMode.Private);

                            var editorPreferencias = preferencias.Edit();

                            editorPreferencias.PutString("mail", Datos.idUsuario);
                            editorPreferencias.PutString("idUsuario", Datos.idUsuario);
                            editorPreferencias.PutString("token", Datos.token);
                            editorPreferencias.PutString("logged", "logged");

                            editorPreferencias.Commit();

                            var intent = new Intent(this, typeof(MainActivity));
                            intent.AddFlags(ActivityFlags.ClearTop);
                            StartActivity(intent);

                        }
                        else {

                            if ((string)resultado["versionActual"] != Datos.versionApp)
                            {

                                Utilidades.showMessage(this, "Antención", Datos.mensajeVersion, "OK");
                            }
                            else if ((string)resultado["activa"] == "0")
                            {

                                Utilidades.showMessage(this, "Antención", Datos.mensajeMantenimiento, "OK");

                            }
                            else
                            {

                                Utilidades.showMessage(this, "Antención", "Código no encontrado", "OK");
                            }


                        }


                    }
                    else
                    {
                        Utilidades.showMessage(this, "Antención", (string)resultado["mensaje"], "OK");

                    }
                }

                else Utilidades.showMessage(this, "Antención", resultadoValidacion, "OK");

            };



        }

        private string ValidarFormulario()
        {

            var receivedCode = FindViewById<EditText>(Resource.Id.receivedCode);
            var password = FindViewById<EditText>(Resource.Id.rememberPassword);
            var passwordCheck = FindViewById<EditText>(Resource.Id.rememberPasswordCheck);

            string resultado = "";

            if (password.Text != null && password.Text != "")
            {

                if (password.Text.Length < 5)
                {
                    resultado = "Digita una contraseña de al menos 5 caracteres.";
                }


                if (password.Text != passwordCheck.Text)
                {
                    resultado = "Las contraseñas No coinciden. Intenta de nuevo.";
                    password.Text = "";
                    passwordCheck.Text = "";
                }


            } else resultado = "Digita una contraseña de al menos 5 caracteres.";

            if (receivedCode.Text == "" || receivedCode == null)
            {
                resultado = "Digita el código recibido";
            }

            return resultado;
        }

        public override void OnBackPressed()
        {
            Finish();
            OverridePendingTransition(0, 0);
        }

    }
}
