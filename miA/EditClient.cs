using System.Collections.Generic;
using System.Json;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace miA
{
    [Activity(Label = "Elemento de Clientes")]
    public class EditClient : ConfirmedActivity
    {

        bool editing = true;
        string clientId;
        ClientDefinition cd;

        ClientTypes elementType;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.EditClient);
            clientId = Intent.GetStringExtra("clientId");
            elementType = ClientTypes.None;
            if (Intent.Extras.ContainsKey("creating"))
            {
                editing = false;
                FindViewById<ImageButton>(Resource.Id.deleteButton).Visibility = ViewStates.Invisible;
                cd = new ClientDefinition();
            }
            else
            {
                cd = ClientDefinition.getNode(Information.mainCd, clientId);
                FindViewById<EditText>(Resource.Id.clientName).Text = cd.name;
                FindViewById<RadioGroup>(Resource.Id.radioGroup).Visibility = ViewStates.Invisible;

            }


            var backButton = FindViewById<ImageButton>(Resource.Id.backButton);
            backButton.Click += (sender, e) => {
                this.Finish();
                OverridePendingTransition(0, 0);
            };


            var deleteButton = FindViewById<ImageButton>(Resource.Id.deleteButton);
            deleteButton.Click += (sender, e) =>
            {
                Utilidades.ConfirmDialog(this, "Atención", "Eliminar este elemento ?", "SI", "NO");

            };

            var saveButton = FindViewById<Button>(Resource.Id.saveButton);

            saveButton.Click += (sender, e) => {
                string validationResult = Validate();
                if (validationResult == "")
                {
                    cd.name = FindViewById<EditText>(Resource.Id.clientName).Text;

                    if (!editing)
                    {
                        cd.mail = FindViewById<EditText>(Resource.Id.clientMail).Text;
                        cd.type = elementType;
                        var parentNode = ClientDefinition.getNode(Information.mainCd, clientId);
                        parentNode.children.Add(cd);

                    }

                    var datos = new Dictionary<string, string>
                    {
                        ["cdJson"] = ClientDefinition.ToJson(Information.mainCd)
                    };

                    if (!editing)
                    {
                        datos["mail"] = cd.mail;
                        datos["operation"] = "add";

                    }

                    JsonValue resultado = Datos.saveUserClients(datos);


                    Finish();
                    OverridePendingTransition(0, 0);

                }
                else Utilidades.showMessage(this, "Atención", validationResult, "OK");

            };

            var elementRadio = FindViewById<RadioButton>(Resource.Id.elementRadio);
            elementRadio.Click += (sender, e) => {

                elementType = ClientTypes.Element;
                FindViewById<EditText>(Resource.Id.clientMail).Visibility = ViewStates.Visible;

            };

            var groupRadio = FindViewById<RadioButton>(Resource.Id.groupRadio);
            groupRadio.Click += (sender, e) => {

                elementType = ClientTypes.Group;
                FindViewById<EditText>(Resource.Id.clientMail).Visibility = ViewStates.Invisible;

            };

        }

        private string Validate()
        {
            var result = "";
            var name = FindViewById<EditText>(Resource.Id.clientName);
            var mail = FindViewById<EditText>(Resource.Id.clientMail);
            if (name.Text == null || name.Text == "")
            {
                result = "Digita el nombre del Elemento";
            }
            else if (!Utilidades.EsCorreoElectronico(mail.Text) && elementType == ClientTypes.Element)
            {
                result = "Digita el correo electrónico del Usuario";
            }
            else if (elementType == ClientTypes.None && !editing)
            {
                result = "Escoge el tipo de elemento que estas creando.";
            }

            return result;
        }

        public override void PositiveConfirm()
        {
            if (cd.type == ClientTypes.Group)
            {
                if (cd.children.Count > 0)
                {
                    Utilidades.showMessage(this,"Antención","Un grupo de Usuarios primero debe estar vacío antes de eliminarlo.","OK");
                    return;
                }
            }

            string parentId = ClientDefinition.getParentNodeId(Information.mainCd, clientId);
            var parentRd = ClientDefinition.getNode(Information.mainCd, parentId);

            foreach (var child in parentRd.children)
            {
                if (child.name == cd.name)
                {
                    parentRd.children.Remove(child);
                    break;
                }
            }

            var datos = new Dictionary<string, string>
            {
                ["cdJson"] = ClientDefinition.ToJson(Information.mainCd),
                ["mail"] = cd.mail,
                ["operation"] = "remove"
            };

            JsonValue resultado = Datos.saveUserClients(datos);


            Intent intent = new Intent(this, typeof(ResourceView));
            intent.PutExtra("close", "");
            SetResult(Result.Ok, intent);
            Finish();
            OverridePendingTransition(0, 0);
        }

        public override void NegativeConfirm()
        {

        }

        public override void OnBackPressed()
        {
            Finish();
            OverridePendingTransition(0, 0);
        }
    }
}
