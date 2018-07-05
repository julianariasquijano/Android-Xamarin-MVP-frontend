using Android.App;
using Android.OS;

using Android.Content;
using Android.Widget;
using Android.Graphics;
using System.Json;

namespace miA
{
    [Activity(Label = "miA", MainLauncher = true)]
    public class MainActivity : Activity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var preferencias = Application.Context.GetSharedPreferences("miax", FileCreationMode.Private);
            var editorPreferencias = preferencias.Edit();

            var logged = preferencias.GetString("logged", null);
            if (logged == "" || logged == null)
            {

                SetContentView(Resource.Layout.login);

                var buttonContinue = FindViewById<Button>(Resource.Id.buttonContinue);
                var buttonRemember = FindViewById<Button>(Resource.Id.buttonRemember);
                var buttonRegister = FindViewById<Button>(Resource.Id.buttonRegister);


                var previousMail = preferencias.GetString("mail", null);
                if (previousMail != "" && previousMail != null)
                {
                    FindViewById<EditText>(Resource.Id.email).Text = previousMail;
                }

                buttonContinue.Click += (sender, e) => {


                    var mail = FindViewById<EditText>(Resource.Id.email);
                    var password = FindViewById<EditText>(Resource.Id.password);

                    InactiveLoginButtons();
                    if (mail.Text == null || mail.Text == "" || !Datos.EsCorreoElectronico(mail.Text))
                        
                        Utilidades.showMessage(this, "Antención", "Para continuar, digita el correo electrónico registrado.", "OK");

                    else if (password.Text == null || password.Text == "")
                        
                        Utilidades.showMessage(this, "Antención", "Escribe tu contraseña", "OK");
                    else
                    {


                        JsonValue resultado = Datos.verificarCredenciales(mail.Text, password.Text);

                        if ((string)resultado["status"] == "OK")
                        {
                            Datos.idUsuario = mail.Text.ToLower().Trim();
                            Datos.token = (string)resultado["token"];

                            if (Datos.token != "")
                            {

                                editorPreferencias.PutString("mail", Datos.idUsuario);
                                editorPreferencias.PutString("idUsuario", Datos.idUsuario);
                                editorPreferencias.PutString("token", Datos.token);
                                editorPreferencias.PutString("logged", "logged");
                                editorPreferencias.Commit();

                                password.Text = "";
                                var intent = new Intent(this, typeof(MainActivity));
                                StartActivity(intent);

                                this.Finish();
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
                                    
                                    Utilidades.showMessage(this, "Antención", "Usuario y contraseña inválidos", "OK");
                                }

                            }

                        }
                        else
                        {
                            
                            Utilidades.showMessage(this, "Antención", (string)resultado["mensaje"], "OK");

                        }


                    }
                    ActiveLoginButtons();
                };


                buttonRemember.Click += (sender, e) => {

                    var mail = FindViewById<EditText>(Resource.Id.email).Text.ToLower().Trim();

                    if (Datos.EsCorreoElectronico(mail))
                    {
                        var intent = new Intent(this, typeof(RememberPassword));
                        intent.PutExtra("mail", mail);
                        StartActivity(intent);
                    }
                    else Utilidades.showMessage(this, "Antención", "Para continuar, digita el correo electrónico registrado.", "OK");



                };

                buttonRegister.Click += (sender, e) => { 
                
                    var intent = new Intent(this, typeof(Registro));
                    StartActivity(intent);
                
                
                };



            }
            else {
                SetContentView(Resource.Layout.main);
                //LinearLayout mainLayout = FindViewById<LinearLayout>(Resource.Id.linearLayout);

                //var layout1 = new LinearLayout(this.BaseContext);layout1.Orientation = Orientation.Horizontal;
                //var button11 = new ImageButton(this.BaseContext);button11.SetBackgroundColor(Color.Transparent);
                //var button12 = new ImageButton(this.BaseContext);
                //button11.SetImageResource(Resource.Drawable.foreignAgendas);
                //button12.SetImageResource(Resource.Drawable.myGeneralAgenda);
                //layout1.AddView(button11);layout1.AddView(button12);

                //mainLayout.AddView(layout1);


                Datos.idUsuario = preferencias.GetString("idUsuario", null);
                Datos.token = preferencias.GetString("token", null);


                var userDataButton = FindViewById<Button>(Resource.Id.userDataButton);
                userDataButton.Click += (sender, e) => {

                    var intent = new Intent(this, typeof(EditarRegistro));
                    StartActivity(intent);


                };


                var closeSessionButton = FindViewById<Button>(Resource.Id.closeSessionButton);
                closeSessionButton.Click +=(sender, e) => {

                    editorPreferencias.PutString("idUsuario", "");
                    editorPreferencias.PutString("token", "");
                    editorPreferencias.PutString("logged", "");
                    editorPreferencias.Commit();

                    var intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);

                    this.Finish();

                };




            }

        }


        void InactiveLoginButtons(){

            FindViewById<Button>(Resource.Id.buttonContinue).Enabled = false;
            FindViewById<Button>(Resource.Id.buttonRemember).Enabled = false;
            FindViewById<Button>(Resource.Id.buttonRegister).Enabled = false;
            
        }

        void ActiveLoginButtons()
        {
            FindViewById<Button>(Resource.Id.buttonContinue).Enabled = true;
            FindViewById<Button>(Resource.Id.buttonRemember).Enabled = true;
            FindViewById<Button>(Resource.Id.buttonRegister).Enabled = true;

        }


    }

}