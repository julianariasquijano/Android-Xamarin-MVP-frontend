using System.Collections.Generic;
using System.Json;


using Android.App;
using Android.Content;
using Android.OS;

using Android.Widget;

namespace miA
{
    [Activity(Label = "Registro")]
    public class Registro : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.registro);


            var backButton = FindViewById<ImageButton>(Resource.Id.backButton);
            backButton.Click += (sender, e) => {
                this.Finish();
                OverridePendingTransition(0, 0);
            };


            var buttonContinue = FindViewById<Button>(Resource.Id.registroButtonContinue);

            buttonContinue.Click += (sender, e) =>
            {

                string resultadoValidacion = ValidarFormulario();
                if (resultadoValidacion == "")
                {

                    buttonContinue.Enabled = false;
                    var nombre = FindViewById<EditText>(Resource.Id.registroNombre);
                    var telefono = FindViewById<EditText>(Resource.Id.registroTelefono);
                    var mail = FindViewById<EditText>(Resource.Id.registroEmail);
                    var password = FindViewById<EditText>(Resource.Id.registroPassword);

                    var datos = new Dictionary<string, string>
                    {
                        ["nombre"] = nombre.Text,
                        ["correo"] = mail.Text,
                        ["telefono"] = telefono.Text,
                        ["password"] = Utilidades.Sha1Hash(password.Text)
                    };


                    JsonValue resultado = Datos.nuevoRegistro(datos);

                    if ((string)resultado["status"] == "OK")
                    {
                        Datos.idUsuario = mail.Text.ToLower().Trim();;
                        Datos.token = (string)resultado["token"];
                        Datos.pdb = (string)resultado["pdb"];
                        Datos.idPdb = (string)resultado["idPdb"];

                        if (Datos.token != "")
                        {
                            var preferencias = Application.Context.GetSharedPreferences("miax", FileCreationMode.Private);

                            var editorPreferencias = preferencias.Edit();
                            editorPreferencias.PutString("mail", Datos.idUsuario);
                            editorPreferencias.PutString("idUsuario", Datos.idUsuario);
                            editorPreferencias.PutString("token", Datos.token);
                            editorPreferencias.PutString("pdb", Datos.pdb);
                            editorPreferencias.PutString("idPdb", Datos.idPdb);

                            editorPreferencias.PutString("logged", "logged");
                            editorPreferencias.Commit();

                            password.Text = "";
                            var intent = new Intent(this, typeof(MainActivity));
                            intent.AddFlags(ActivityFlags.ClearTop);
                            StartActivity(intent);

                        }
                        else
                        {

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
                                Utilidades.showMessage(this, "Antención", (string)resultado["mensaje"], "OK");
                            }

                        }

                    }
                    else
                    {

                        Utilidades.showMessage(this, "Antención", (string)resultado["mensaje"], "OK");

                    }

                }
                else Utilidades.showMessage(this, "Antención", resultadoValidacion, "OK");

                buttonContinue.Enabled = true;

            };

        }


        private string ValidarFormulario()
        {

            var nombre = FindViewById<EditText>(Resource.Id.registroNombre);
            var telefono = FindViewById<EditText>(Resource.Id.registroTelefono);
            var mail = FindViewById<EditText>(Resource.Id.registroEmail);
            var password = FindViewById<EditText>(Resource.Id.registroPassword);
            var passwordCheck = FindViewById<EditText>(Resource.Id.registroPasswordCheck);

            string resutlado = "";

            if (password.Text == null || password.Text.Length < 5 || password.Text == "")
                resutlado = "Digita una contraseña de al menos 5 caracteres.";

            if (password.Text != passwordCheck.Text)
            {
                resutlado = "Las contraseñas No coinciden. Intenta de nuevo.";
                password.Text = "";
                passwordCheck.Text = "";
            }

            if (telefono.Text == null || telefono.Text == "")
                resutlado = "Digita un teléfono de contacto.";


            if (mail.Text == null || !Utilidades.EsCorreoElectronico(mail.Text) || mail.Text == "")
                resutlado = "Digita tu correo electrónico.";


            if (nombre.Text == null || nombre.Text == "")
                resutlado = "Digita tu nombre completo.";


            return resutlado;
        }

        public override void OnBackPressed()
        {
            Finish();
            OverridePendingTransition(0, 0);
        }

    }

}