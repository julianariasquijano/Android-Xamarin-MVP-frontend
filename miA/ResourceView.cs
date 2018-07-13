using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace miA
{
    [Activity(Label = "Recurso")]
    public class ResourceView : Activity
    {
        ResourceDefinition rd;
        string json;
        string parentResourceId;
        string resourceId;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.resourceView);

            var backButton = FindViewById<ImageButton>(Resource.Id.backButton);
            backButton.Click +=(sender, e) => {
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
            rd = ResourceDefinition.FromJson(json);

            if (rd.type == ResourceTypes.Element)
            {
                FindViewById<ImageButton>(Resource.Id.addButton).Visibility = ViewStates.Invisible;
            }

            var editButton = FindViewById<ImageButton>(Resource.Id.editButton);
            editButton.Click += (sender, e) => {
                var intent = new Intent(this, typeof(EditResource));
                intent.PutExtra("resourceId", resourceId);
                StartActivityForResult(intent, 0);
                OverridePendingTransition(0, 0);
            };

            var addButton = FindViewById<ImageButton>(Resource.Id.addButton);
            addButton.Click += (sender, e) => {

                var intent = new Intent(this, typeof(EditResource));
                intent.PutExtra("resourceId", resourceId);
                intent.PutExtra("creating", "");
                StartActivity(intent);
                OverridePendingTransition(0, 0);
                
            };


        }

        protected override void OnResume(){
            base.OnResume();
            if (Intent.Extras.ContainsKey("start"))
            {
                var editButton = FindViewById<ImageButton>(Resource.Id.editButton);
                editButton.Visibility = ViewStates.Invisible;
                resourceId = rd.name;
            }
            else
            {
                parentResourceId = Intent.GetStringExtra("parentResourceId");
                resourceId = parentResourceId + "--" + rd.name;
            }
            rd = ResourceDefinition.getNode(Information.mainRd, resourceId);
            FindViewById<TextView>(Resource.Id.resourceLabel).Text = rd.name;
            PopulateButtons();
            
        }

        private void PopulateButtons(){
            var resourceViewLayout = FindViewById<LinearLayout>(Resource.Id.resourceViewLayout);
            resourceViewLayout.RemoveAllViews();
            foreach (var childRd in rd.children)
            {

                addLayoutButton(resourceViewLayout, ResourceDefinition.ToJson(childRd));

            }
        }



        private void addLayoutButton(LinearLayout linearLayout,string json){
            var rdForButton = ResourceDefinition.FromJson(json);

            var button = new Button(this.BaseContext);
            button.SetBackgroundColor(Color.Transparent);
            button.SetTextColor(Color.Black);

            button.Text = rdForButton.name;
            if (rdForButton.type == ResourceTypes.Group)
            {
                button.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.folder, 0, 0, 0);
            }
            else button.SetCompoundDrawablesWithIntrinsicBounds(Resource.Drawable.resource, 0,0,0);

            button.Click += (sender, e) => {

                var intent = new Intent(this, typeof(ResourceView));
                intent.PutExtra("json", json);
                intent.PutExtra("parentResourceId", resourceId);
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
            Finish();
            OverridePendingTransition(0, 0);
        }


    }
}
