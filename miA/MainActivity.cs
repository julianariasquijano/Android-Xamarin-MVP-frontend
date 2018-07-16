using Android.App;
using Android.OS;

using Android.Content;
using Android.Widget;
using Android.Graphics;
using System.Json;
using Android.Content.PM;

namespace miA
{
    [Activity(Label = "miA", MainLauncher = true, ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait )]
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
                    if (mail.Text == null || mail.Text == "" || !Utilidades.EsCorreoElectronico(mail.Text))
                        
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
                            Datos.pdb = (string)resultado["pdb"];
                            Datos.idPdb = (string)resultado["idPdb"];

                            if (Datos.token != "")
                            {

                                editorPreferencias.PutString("mail", Datos.idUsuario);
                                editorPreferencias.PutString("idUsuario", Datos.idUsuario);
                                editorPreferencias.PutString("token", Datos.token);
                                editorPreferencias.PutString("pdb", Datos.pdb);
                                editorPreferencias.PutString("idPdb", Datos.idPdb);

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

                FindViewById<EditText>(Resource.Id.password).EditorAction += (sender, e) => { buttonContinue.PerformClick(); };

                buttonRemember.Click += (sender, e) => {

                    var mail = FindViewById<EditText>(Resource.Id.email).Text.ToLower().Trim();

                    if (Utilidades.EsCorreoElectronico(mail))
                    {
                        var intent = new Intent(this, typeof(RememberPassword));
                        intent.PutExtra("mail", mail);
                        StartActivity(intent);
                        OverridePendingTransition(0, 0);
                    }
                    else Utilidades.showMessage(this, "Antención", "Para continuar, digita el correo electrónico registrado.", "OK");


                };

                buttonRegister.Click += (sender, e) => { 
                
                    var intent = new Intent(this, typeof(Registro));
                    StartActivity(intent);
                    OverridePendingTransition(0, 0);
                
                };



            }
            else {
                SetContentView(Resource.Layout.main);

                Datos.idUsuario = preferencias.GetString("idUsuario", null);
                Datos.token = preferencias.GetString("token", null);
                Datos.pdb = preferencias.GetString("pdb", null);
                Datos.idPdb = preferencias.GetString("idPdb", null);


                var foreignAgendasButton = FindViewById<Button>(Resource.Id.foreignAgendasButton);
                foreignAgendasButton.Click += (sender, e) => {

                    Information.PopulateForeignAgendas();

                    var intent = new Intent(this, typeof(ForeignAgendas));
                    StartActivity(intent);
                    OverridePendingTransition(0, 0);

                };


                var myResourcesButton = FindViewById<Button>(Resource.Id.myResourcesButton);
                myResourcesButton.Click += (sender, e) => {

                    if (Information.mainRd == null)
                    {
                        Information.PopulateResources();
                    }

                    var intent = new Intent(this, typeof(ResourceView));
                    intent.PutExtra("json", ResourceDefinition.ToJson(Information.mainRd));
                    intent.PutExtra("start", "");
                    StartActivity(intent);
                    OverridePendingTransition(0, 0);


                };

                var myClientsButton = FindViewById<Button>(Resource.Id.myClientsButton);
                myClientsButton.Click += (sender, e) => {

                    if (Information.mainCd == null)
                    {
                        Information.PopulateClients();
                    }


                    var intent = new Intent(this, typeof(ClientView));
                    intent.PutExtra("json", ClientDefinition.ToJson(Information.mainCd));
                    intent.PutExtra("start", "");
                    StartActivity(intent);
                    OverridePendingTransition(0, 0);

                };

                var userDataButton = FindViewById<Button>(Resource.Id.userDataButton);
                userDataButton.Click += (sender, e) => {

                    var intent = new Intent(this, typeof(EditarRegistro));
                    StartActivity(intent);
                    OverridePendingTransition(0, 0);


                };


                var closeSessionButton = FindViewById<Button>(Resource.Id.closeSessionButton);
                closeSessionButton.Click +=(sender, e) => {

                    editorPreferencias.PutString("idUsuario", "");
                    editorPreferencias.PutString("token", "");
                    editorPreferencias.PutString("logged", "");
                    editorPreferencias.Commit();

                    Information.mainCd = null;
                    Information.mainRd = null;

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