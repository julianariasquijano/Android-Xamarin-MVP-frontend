using System.Collections.ObjectModel;
using System.Json;
using Android.App;
using Android.OS;
using Com.Syncfusion.Charts;


namespace miA
{
    [Activity(Label = "Estadísticas")]
    public class StatsView : Activity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            ///////////////////////////////////////
            SfChart chart = new SfChart(this);

            //Initializing Primary Axis
            CategoryAxis primaryAxis = new CategoryAxis();

            chart.PrimaryAxis = primaryAxis;

            //Initializing Secondary Axis
            NumericalAxis secondaryAxis = new NumericalAxis();

            chart.SecondaryAxis = secondaryAxis;

            //*************************

            primaryAxis.Title.Text = "Nombre";

            chart.PrimaryAxis = primaryAxis;

            secondaryAxis.Title.Text = "Cantidad de Reservas";

            chart.SecondaryAxis = secondaryAxis;

            //Initializing column series
            ColumnSeries series = new ColumnSeries();

            StatsViewModel viewModel = new StatsViewModel();

            series.ItemsSource = viewModel.Data;

            series.XBindingPath = "name";

            series.YBindingPath = "quantity";

            chart.Series.Add(series);

            //***************
            chart.Title.Text = "Uso de Recursos";

            series.DataMarker.ShowLabel = true;

            chart.Legend.Visibility = Visibility.Visible;

            series.TooltipEnabled = true;

            SetContentView(chart);

        }

        public override void OnBackPressed()
        {
            Finish();
            OverridePendingTransition(0, 0);
        }

    }


    public class Stat
    {
        public string name { get; set; }
        public double quantity { get; set; }
    }

    public class StatsViewModel
    {
        public ObservableCollection<Stat> Data { get; set; }

        public StatsViewModel()
        {
            Data = new ObservableCollection<Stat>();


            JsonValue result = Datos.LlamarWsSync(Datos.sessionDataWebServiceUrl + "getResourcesStats", Datos.idUsuario);


            if ((string)result["status"] == "OK")
            {
                foreach (JsonValue stat in result["stats"])
                {
                    var statItem = new Stat();
                    statItem.name = stat["name"];
                    statItem.quantity = stat["quantity"];

                    Data.Add(statItem);
                }

            }

        }
    }

}