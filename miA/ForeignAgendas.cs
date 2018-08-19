
using System.Collections.Generic;
using System.Json;


using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace miA
{
    [Activity(Label = "Agendas Externas")]
    public class ForeignAgendas : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ForeignAgendas);

            var backButton = FindViewById<ImageButton>(Resource.Id.backButton);
            backButton.Click += (sender, e) => {
                this.Finish();
                OverridePendingTransition(0, 0);
            };

            var foreignAgendasList = FindViewById<ListView>(Resource.Id.agendasList);
            foreignAgendasList.Adapter = new ArrayAdapter<ForeignAgenda>(this, Android.Resource.Layout.SimpleListItem1, Information.foreignAgendas);
            foreignAgendasList.ItemClick += OnItemClick;


        }

        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {

            var foreignAgenda = Information.foreignAgendas[e.Position];

            var datos = new Dictionary<string, string>
            {
                ["pdb"] = foreignAgenda.pdb,
                ["idPdb"] = foreignAgenda.idPdb.ToString()
            };

            JsonValue resultado = Datos.getForeignAgendaResources(datos);
            var foreignAgendaResources = new ResourceDefinition();

            if ((string)resultado["status"] == "OK")
            {
                string rdJson = resultado["resources"].ToString();
                if (rdJson != "" && rdJson != null)
                {
                    //foreignAgendaResources = ResourceDefinition.FromJson(rdJson);
                    Information.foreignRd = ResourceDefinition.FromJson(rdJson);

                    var intent = new Intent(this, typeof(ForeignResourceView));
                    intent.PutExtra("rdJson", rdJson);
                    intent.PutExtra("start", "");
                    Information.seletedForeignAgenda = foreignAgenda;

                    StartActivityForResult(intent, 0);
                    OverridePendingTransition(0, 0);


                }

            }
            else
            {

                Utilidades.showMessage(this, "Antención", "Error de conexión", "OK");

            }

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
                        OverridePendingTransition(0, 0);
                    }
                    if (data.Extras.ContainsKey("networkError"))
                    {
                        Utilidades.showMessage(this, "Atención", "Error de conexión", "OK");
                    }
                }

            }
        }


        public override void OnBackPressed()
        {
            Finish();
            OverridePendingTransition(0, 0);
        }

    }
}
