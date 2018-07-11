using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace miA
{
    [Activity(Label = "Cliente")]
    public class ClientView: Activity
    {
        ClientDefinition cd;
        string json;
        string parentClientId;
        string clientId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.resourceView);

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


            json = Intent.GetStringExtra("json");
            cd = ClientDefinition.FromJson(json);

            if (cd.type == ClientTypes.Element)
            {
                FindViewById<ImageButton>(Resource.Id.addButton).Visibility = ViewStates.Invisible;
            }

            var editButton = FindViewById<ImageButton>(Resource.Id.editButton);
            editButton.Click += (sender, e) => {
                var intent = new Intent(this, typeof(EditClient));
                intent.PutExtra("clientId", clientId);
                //StartActivity(intent);
                StartActivityForResult(intent, 0);
                OverridePendingTransition(0, 0);
            };

            var addButton = FindViewById<ImageButton>(Resource.Id.addButton);
            addButton.Click += (sender, e) => {

                var intent = new Intent(this, typeof(EditClient));
                intent.PutExtra("clientId", clientId);
                intent.PutExtra("creating", "");
                StartActivity(intent);
                OverridePendingTransition(0, 0);

            };


        }

        protected override void OnResume()
        {
            base.OnResume();
            if (Intent.Extras.ContainsKey("start"))
            {
                var editButton = FindViewById<ImageButton>(Resource.Id.editButton);
                editButton.Visibility = ViewStates.Invisible;
                clientId = cd.name;
            }
            else
            {
                parentClientId = Intent.GetStringExtra("parentClientId");
                clientId = parentClientId + "--" + cd.name;
            }
            cd = ClientDefinition.getNode(Information.mainCd, clientId);
            FindViewById<TextView>(Resource.Id.resourceLabel).Text = cd.name;
            PopulateButtons();

        }

        private void PopulateButtons()
        {
            var resourceViewLayout = FindViewById<LinearLayout>(Resource.Id.resourceViewLayout);
            resourceViewLayout.RemoveAllViews();
            foreach (var childRd in cd.children)
            {

                addLayoutButton(resourceViewLayout, ClientDefinition.ToJson(childRd));

            }
        }



        private void addLayoutButton(LinearLayout linearLayout, string json)
        {
            var cdForButton = ClientDefinition.FromJson(json);

            var button = new Button(this.BaseContext);
            button.SetBackgroundColor(Color.Transparent);
            button.SetTextColor(Color.Black);

            button.Text = cdForButton.name;
            if (cdForButton.type == ClientTypes.Group)
            {
                button.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.folder, 0, 0, 0);
            }
            else button.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.resource, 0, 0, 0);

            button.Click += (sender, e) => {

                var intent = new Intent(this, typeof(ClientView));
                intent.PutExtra("json", json);
                intent.PutExtra("parentClientId", clientId);
                StartActivity(intent);
                //OverridePendingTransition(Resource.Animation.abc_slide_in_top, Resource.Animation.abc_slide_out_top);
                OverridePendingTransition(0, 0);

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
                    }
                }

            }
        }

        public override void OnBackPressed()
        {
            return;
        }
    }
}
