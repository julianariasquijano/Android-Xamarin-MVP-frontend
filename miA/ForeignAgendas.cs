
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
                    intent.PutExtra("json", rdJson);
                    intent.PutExtra("start", "");
                    StartActivity(intent);
                    OverridePendingTransition(0, 0);


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
