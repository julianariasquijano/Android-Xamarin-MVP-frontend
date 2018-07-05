﻿
using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace miA
{
    [Activity(Label = "Mis datos")]
    public class EditarRegistro : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.registro);

            var nombre = FindViewById<EditText>(Resource.Id.registroNombre);
            var mail = FindViewById<EditText>(Resource.Id.registroEmail);
            var telefono = FindViewById<EditText>(Resource.Id.registroTelefono);

            FindViewById<TextView>(Resource.Id.mensajeRegistro).Text="Si No deseas cambiar tu contraseña, dejala en blanco.";

            JsonValue resultado = Datos.LlamarWsSync(Datos.sessionDataWebServiceUrl+"traerRegistro",Datos.idUsuario);

            if ((string)resultado["status"] == "OK")
            {
                nombre.Text = (string)resultado["nombre"];
                mail.Text = (string)resultado["correo"];
                telefono.Text = (string)resultado["telefono"];
            }
            else
            {

                Utilidades.showMessage(this, "Antención", (string)resultado["mensaje"], "OK");
                this.Finish();

            }


            var buttonContinue = FindViewById<Button>(Resource.Id.registroButtonContinue);

            buttonContinue.Click += (sender, e) =>
            {
                

                string resultadoValidacion = ValidarFormulario();
                if (resultadoValidacion == "")
                {
                     

                    buttonContinue.Enabled = false;
                    var password = FindViewById<EditText>(Resource.Id.registroPassword);

                    var tempPassword = password.Text;

                    if (tempPassword != "" && tempPassword != null)
                    {
                        tempPassword = Datos.Sha1Hash(password.Text);
                    }


                    var datos = new Dictionary<string, string>
                    {
                        ["nombre"] = nombre.Text,
                        ["correo"] = mail.Text,
                        ["telefono"] = telefono.Text,
                        ["password"] = tempPassword
                    };


                    resultado = Datos.editarRegistro(datos);

                    if ((string)resultado["status"] == "OK" && (string) resultado["mensaje"] == "" )
                    {
                        Datos.idUsuario = mail.Text.ToLower().Trim();

                        var preferencias = Application.Context.GetSharedPreferences("miax", FileCreationMode.Private);

                        var editorPreferencias = preferencias.Edit();
                        editorPreferencias.PutString("mail", mail.Text);
                        editorPreferencias.PutString("idUsuario", Datos.idUsuario);
                        editorPreferencias.Commit();

                        this.Finish();

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

            if (password.Text != null && password.Text != "")
            {

                if ( password.Text.Length < 5 )
                {
                    resutlado = "Digita una contraseña de al menos 5 caracteres.";
                }


                if (password.Text != passwordCheck.Text)
                {
                    resutlado = "Las contraseñas No coinciden. Intenta de nuevo.";
                    password.Text = "";
                    passwordCheck.Text = "";
                }


            }
                

            if (telefono.Text == null || telefono.Text == "")
                resutlado = "Digita un teléfono de contacto.";


            if (mail.Text == null || !Datos.EsCorreoElectronico(mail.Text) || mail.Text == "")
                resutlado = "Digita tu correo electrónico.";


            if (nombre.Text == null || nombre.Text == "")
                resutlado = "Digita tu nombre completo.";


            return resutlado;
        }

    }
}
