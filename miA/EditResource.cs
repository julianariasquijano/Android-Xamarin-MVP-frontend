using System.Collections.Generic;
using System.Json;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace miA
{
    [Activity(Label = "Elemento de Recursos")]
    public class EditResource : ConfirmedActivity
    {

        bool editing = true;
        string resourceId;
        ResourceDefinition rd;

        ResourceTypes elementType;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.EditResource);
            resourceId = Intent.GetStringExtra("resourceId");
            elementType = ResourceTypes.None;
            if (Intent.Extras.ContainsKey("creating"))
            {
                editing = false;
                FindViewById<ImageButton>(Resource.Id.deleteButton).Visibility = ViewStates.Invisible;
                rd = new ResourceDefinition();
            }
            else {
                rd = ResourceDefinition.getNode(Information.mainRd, resourceId);
                FindViewById<EditText>(Resource.Id.resourceName).Text = rd.name;
                FindViewById<RadioGroup>(Resource.Id.radioGroup).Visibility = ViewStates.Invisible;

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
                    rd.name = FindViewById<EditText>(Resource.Id.resourceName).Text;
                    if (!editing) {
                        
                        rd.type = elementType;
                        var parentNode = ResourceDefinition.getNode(Information.mainRd, resourceId);
                        parentNode.children.Add(rd);
                      
                    }

                    var datos = new Dictionary<string, string>
                    {
                        ["rdJson"] = ResourceDefinition.ToJson(Information.mainRd)
                    };
                    JsonValue resultado = Datos.saveUserResources(datos);


                    Finish();
                    OverridePendingTransition(0, 0);  

                }
                else Utilidades.showMessage(this, "Atención", validationResult, "OK");

            };

            var elementRadio = FindViewById<RadioButton>(Resource.Id.elementRadio);
            elementRadio.Click += (sender, e) => {

                elementType = ResourceTypes.Element;

            };

            var groupRadio = FindViewById<RadioButton>(Resource.Id.groupRadio);
            groupRadio.Click += (sender, e) => {

                elementType = ResourceTypes.Group;

            };

        }

        private string ValidateResource(){
            var result = "";
            var resourceName = FindViewById<EditText>(Resource.Id.resourceName);
            if (resourceName.Text == null ||  resourceName.Text == "")
            {
                result = "Digita el nombre del Elemento";
            }
            else if(elementType == ResourceTypes.None && !editing){
                result = "Escoge el tipo de elemento que estas creando.";
            }

            return result;
        }

        public override void PositiveConfirm(){

            string parentId = ResourceDefinition.getParentNodeId(Information.mainRd, resourceId);
            var parentRd = ResourceDefinition.getNode(Information.mainRd, parentId);

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
