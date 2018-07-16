using System.Collections.Generic;
using System.Json;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;

namespace miA
{
    [Activity(Label = "Recursos Externos")]
    public class ForeignResourceView : Activity
    {
        ResourceDefinition rd;
        string rdJson;
        string parentResourceId;
        string resourceId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ForeignResourceView);

            var backButton = FindViewById<ImageButton>(Resource.Id.backButton);
            backButton.Click += (sender, e) => {
                this.Finish();
                //OverridePendingTransition(Resource.Animation.abc_slide_in_top, Resource.Animation.abc_slide_out_top);
                OverridePendingTransition(0, 0);
            };

            var homeButton = FindViewById<ImageButton>(Resource.Id.homeButton);
            homeButton.Click += (sender, e) => {
                var intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop);
                StartActivity(intent);
                OverridePendingTransition(0, 0);

            };


            rdJson = Intent.GetStringExtra("rdJson");
            rd = ResourceDefinition.FromJson(rdJson);

        }

        protected override void OnResume()
        {
            base.OnResume();
            if (Intent.Extras.ContainsKey("start"))
            {
                resourceId = rd.name;
            }
            else
            {
                parentResourceId = Intent.GetStringExtra("parentResourceId");
                resourceId = parentResourceId + "--" + rd.name;
            }
            rd = ResourceDefinition.getNode(Information.foreignRd, resourceId);
            FindViewById<TextView>(Resource.Id.resourceLabel).Text = rd.name;
            PopulateButtons();

        }

        private void PopulateButtons()
        {
            var resourceViewLayout = FindViewById<LinearLayout>(Resource.Id.resourceViewLayout);
            resourceViewLayout.RemoveAllViews();
            foreach (var childRd in rd.children)
            {
                if (childRd.active)
                {
                    addLayoutButton(resourceViewLayout, ResourceDefinition.ToJson(childRd));
                }

            }
        }



        private void addLayoutButton(LinearLayout linearLayout, string json)
        {
            var rdForButton = ResourceDefinition.FromJson(json);

            var button = new Button(this.BaseContext);
            button.SetBackgroundColor(Color.Transparent);
            button.SetTextColor(Color.Black);

            button.Text = rdForButton.name;
            if (rdForButton.type == ResourceTypes.Group)
            {
                button.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.folder, 0, 0, 0);
            }
            else button.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.resource, 0, 0, 0);

            button.Click += (sender, e) => {

                if (rdForButton.type == ResourceTypes.Group)
                {
                    var intent = new Intent(this, typeof(ForeignResourceView));

                    intent.PutExtra("rdJson", json);
                    intent.PutExtra("parentResourceId", resourceId);
                    StartActivity(intent);
                    OverridePendingTransition(0, 0);
                }
                else
                {

                    var intent = new Intent(this, typeof(AgendaView));

                    intent.PutExtra("rdJson", json);
                    StartActivityForResult(intent, 0);
                    OverridePendingTransition(0, 0);

                }

            };

            linearLayout.AddView(button);

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