using Android.App;
using Android.OS;

using Android.Content;
using Android.Widget;
using System.Json;
using System;
using System.Threading.Tasks;
using System.Threading;
using Android.Util;
using Android.Support.V4.App;
using Android.Media;

namespace miA
{
    [Activity(Label = "miA", MainLauncher = true, Icon = "@drawable/miaLogo" )]
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

                var ServiceIntent = new Intent(this, typeof(MiaNotificationService));
                if (Information.ServiceStopFlag)
                {
                    Information.ServiceStopFlag = false;
                    StartService(ServiceIntent);
                }

                Datos.idUsuario = preferencias.GetString("idUsuario", null);
                Datos.token = preferencias.GetString("token", null);
                Datos.pdb = preferencias.GetString("pdb", null);
                Datos.idPdb = preferencias.GetString("idPdb", null);


                var foreignAgendasButton = FindViewById<Button>(Resource.Id.foreignAgendasButton);
                foreignAgendasButton.Click += (sender, e) => {

                    if (!Information.PopulateForeignAgendas())
                    {
                        Utilidades.showMessage(this, "Antención", "Error de conexión", "OK");
                    }
                    else {
                        var intent = new Intent(this, typeof(ForeignAgendas));
                        StartActivity(intent);
                        OverridePendingTransition(0, 0);
   
                    }

                };


                var generalAgendaButton = FindViewById<Button>(Resource.Id.generalAgendaButton);
                generalAgendaButton.Click += (sender, e) => {


                    var intent = new Intent(this, typeof(PersonalAgendaActivity));
                    StartActivity(intent);
                    OverridePendingTransition(0, 0);


                };


                var statsButton = FindViewById<Button>(Resource.Id.statsButton);
                statsButton.Click += (sender, e) => {

                    var intent = new Intent(this, typeof(StatsView));
                    StartActivity(intent);
                    OverridePendingTransition(0, 0);

                };


                var myResourcesButton = FindViewById<Button>(Resource.Id.myResourcesButton);
                myResourcesButton.Click += (sender, e) => {

                    if (Information.mainRd == null)
                    {
                        if (!Information.PopulateResources())
                        {
                            Utilidades.showMessage(this, "Antención", "Error de conexión", "OK");
                        }
                        else {
                            var intent = new Intent(this, typeof(ResourceView));
                            intent.PutExtra("json", ResourceDefinition.ToJson(Information.mainRd));
                            intent.PutExtra("start", "");
                            StartActivity(intent);
                            OverridePendingTransition(0, 0);
                        }

                    }
                    else {
                        var intent = new Intent(this, typeof(ResourceView));
                        intent.PutExtra("json", ResourceDefinition.ToJson(Information.mainRd));
                        intent.PutExtra("start", "");
                        StartActivity(intent);
                        OverridePendingTransition(0, 0);
                        
                    }

                };

                var myClientsButton = FindViewById<Button>(Resource.Id.myClientsButton);
                myClientsButton.Click += (sender, e) => {

                    if (Information.mainCd == null)
                    {
                        if(!Information.PopulateClients()){
                            Utilidades.showMessage(this, "Antención", "Error de conexión", "OK");
                        }
                        else {
                            var intent = new Intent(this, typeof(ClientView));
                            intent.PutExtra("json", ClientDefinition.ToJson(Information.mainCd));
                            intent.PutExtra("start", "");
                            StartActivity(intent);
                            OverridePendingTransition(0, 0);  
                        }
                    }
                    else {
                        var intent = new Intent(this, typeof(ClientView));
                        intent.PutExtra("json", ClientDefinition.ToJson(Information.mainCd));
                        intent.PutExtra("start", "");
                        StartActivity(intent);
                        OverridePendingTransition(0, 0);  
                    }




                };

                var userDataButton = FindViewById<Button>(Resource.Id.userDataButton);
                userDataButton.Click += (sender, e) => {
                    
                    var intent = new Intent(this, typeof(EditarRegistro));
                    StartActivityForResult(intent, 0);
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

                    Information.ServiceStopFlag = true;
                    StopService(ServiceIntent);


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


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                if (data.Extras != null)
                {
                    if (data.Extras.ContainsKey("close"))
                    {
                        Finish();
                    }
                    if (data.Extras.ContainsKey("networkError"))
                    {
                        Utilidades.showMessage(this,"Atención","Error de conexión","OK");
                    }
                }

            }
        }


    }

    [Service (Label = "MiaNotificationService") ] 
    class MiaNotificationService : Service
    {
        const string tag = "MiaNotificationService";

        public override void OnCreate()
        {
            base.OnCreate();

        }

        public override IBinder OnBind(Intent intent)
        {

            throw new NotImplementedException();
        }


        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Task.Run(() => {
                while (true)
                {
                    if (Information.ServiceStopFlag)
                    {
                        break;
                    }

                    const int NOTIFICATION_ID = 9000;
                    Notification.Builder notificationBuilder = new Notification.Builder(this)
                        .SetSmallIcon(Resource.Drawable.miniMiaLogo)
                        .SetContentTitle("Mia Agenda")
                        .SetVibrate(new long[] { 1000, 1000, 1000, 1000, 1000 })
                        .SetStyle(new Notification.BigTextStyle().BigText("Fulano reservó tal cosa tal dia. y tales y pascuales"));


                    var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                    notificationManager.Notify(NOTIFICATION_ID, notificationBuilder.Build());


                    //Thread.Sleep(300000);
                    Thread.Sleep(10000);
                }
            });

            return StartCommandResult.StickyCompatibility;
        }

        public override void OnDestroy()
        {
        }
    }

}