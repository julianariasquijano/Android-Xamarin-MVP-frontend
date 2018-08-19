using System;
using System.Collections.Generic;
using System.Json;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace miA
{
    [Activity(Label = "Elemento de Recursos", ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class EditResource : ConfirmedActivity
    {

        bool editing = true;
        string resourceId;
        string parentResourceId;
        ResourceDefinition rd;

        ResourceTypes elementType;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.EditResource);
            resourceId = Intent.GetStringExtra("resourceId");
            parentResourceId = Intent.GetStringExtra("parentResourceId");

            elementType = ResourceTypes.None;
            if (Intent.Extras.ContainsKey("creating"))
            {
                editing = false;
                FindViewById<ImageButton>(Resource.Id.deleteButton).Visibility = ViewStates.Invisible;
                FindViewById<LinearLayout>(Resource.Id.rdHoursLayout).Visibility = ViewStates.Invisible;
                FindViewById<Switch>(Resource.Id.active).Checked = true;
                rd = new ResourceDefinition();
            }
            else {
                rd = ResourceDefinition.getNode(Information.mainRd, resourceId);
                FindViewById<EditText>(Resource.Id.resourceName).Text = rd.name;
                FindViewById<RadioGroup>(Resource.Id.radioGroup).Visibility = ViewStates.Invisible;
                FindViewById<EditText>(Resource.Id.rdMinutes).Text= rd.minutes.ToString();

                if (rd.active)
                {
                    FindViewById<Switch>(Resource.Id.active).Checked = true;
                } else FindViewById<Switch>(Resource.Id.active).Checked = false;

                if (rd.type==ResourceTypes.Element)
                {
                    FindViewById<LinearLayout>(Resource.Id.rdHoursLayout).Visibility = ViewStates.Visible;
                } else FindViewById<LinearLayout>(Resource.Id.rdHoursLayout).Visibility = ViewStates.Invisible;


            }


            var backButton = FindViewById<ImageButton>(Resource.Id.backButton);
            backButton.Click += (sender, e) => {
                this.Finish();
                OverridePendingTransition(0, 0);
            };


            var deleteButton = FindViewById<ImageButton>(Resource.Id.deleteButton);
            deleteButton.Click +=  (sender, e) =>
            {
                Utilidades.ConfirmDialog(this, "Atención", "Eliminar este elemento y su contenido ?", "SI", "NO");

            };


            var resourceSaveButton = FindViewById<Button>(Resource.Id.resourceSaveButton);

            resourceSaveButton.Click += (sender, e) => {
                string validationResult = ValidateResource();
                if (validationResult == "")
                {
                    string newName = FindViewById<EditText>(Resource.Id.resourceName).Text;

                    //avoid name repetitions at this level

                    var parentRd = new ResourceDefinition();
                    if (parentResourceId == "") parentRd = Information.mainRd;
                    else parentRd = ResourceDefinition.getNode(Information.mainRd, parentResourceId);

                    bool nameAlreadyExists = false;

                    foreach (var child in parentRd.children)
                    {
                        if (child.name == newName)
                        {
                            nameAlreadyExists = true;
                            break;
                        }
                    }

                    if (!nameAlreadyExists || editing)
                    {

                        rd.name = FindViewById<EditText>(Resource.Id.resourceName).Text;

                        if (FindViewById<Switch>(Resource.Id.active).Checked)
                        {
                            rd.active = true;
                        }
                        else rd.active = false;

                        try
                        {
                            rd.minutes = Int32.Parse(FindViewById<EditText>(Resource.Id.rdMinutes).Text);
                        }
                        catch { }


                        if (!editing)
                        {

                            rd.type = elementType;
                            var parentNode = ResourceDefinition.getNode(Information.mainRd, parentResourceId);
                            parentNode.children.Add(rd);

                        }

                        var datos = new Dictionary<string, string>
                        {
                            ["rdJson"] = ResourceDefinition.ToJson(Information.mainRd)
                        };
                        JsonValue resultado = Datos.saveUserResources(datos);


                        if ((string)resultado["status"] == "OK")
                        {
                            Finish();
                            OverridePendingTransition(0, 0);
                        }
                        else
                        {
                            Utilidades.showMessage(this, "Antención", "Error de conexión", "OK");
                        }
                    }

                    else Utilidades.showMessage(this, "Error", "Ya existe un elemento de recurso con ese nombre.", "OK");


                }
                else Utilidades.showMessage(this, "Atención", validationResult, "OK");

            };

            var elementRadio = FindViewById<RadioButton>(Resource.Id.elementRadio);
            elementRadio.Click += (sender, e) => {

                elementType = ResourceTypes.Element;
                FindViewById<LinearLayout>(Resource.Id.rdHoursLayout).Visibility = ViewStates.Visible;

            };

            var groupRadio = FindViewById<RadioButton>(Resource.Id.groupRadio);
            groupRadio.Click += (sender, e) => {

                elementType = ResourceTypes.Group;
                FindViewById<LinearLayout>(Resource.Id.rdHoursLayout).Visibility = ViewStates.Invisible;

            };

        }

        private string ValidateResource(){
            var result = "";
            var resourceName = FindViewById<EditText>(Resource.Id.resourceName);
            var minutes = FindViewById<EditText>(Resource.Id.rdMinutes);


            if (resourceName.Text == null ||  resourceName.Text == "")
            {
                result = "Digita el nombre del Elemento";
            }
            else if(elementType == ResourceTypes.None && !editing){
                result = "Escoge el tipo de elemento que estas creando.";
            }
            else if((minutes.Text == "" || minutes.Text =="0")&& elementType == ResourceTypes.Element){
                result = "Debes definir los minutos que dura la utilización de este recurso.(Mayor a Cero)";
            }

            return result;
        }

        public override void PositiveConfirm(){

            //string parentId = ResourceDefinition.getParentNodeId(Information.mainRd, resourceId);
            var parentRd = ResourceDefinition.getNode(Information.mainRd, parentResourceId);

            foreach (var child in parentRd.children)
            {
                if (child.name == rd.name)
                {
                    parentRd.children.Remove(child);
                    break;
                }
            }

            var datos = new Dictionary<string, string>
            {
                ["rdJson"] = ResourceDefinition.ToJson(Information.mainRd)
            };

            JsonValue resultado = Datos.saveUserResources(datos);


            if ((string)resultado["status"] == "OK" )
            {

                Intent intent = new Intent(this, typeof(ResourceView));
                intent.PutExtra("close", "");
                SetResult(Result.Ok, intent);
                Finish();
                OverridePendingTransition(0, 0);

            }
            else
            {
                Utilidades.showMessage(this, "Antención", "Error de conexión", "OK");
            }


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
