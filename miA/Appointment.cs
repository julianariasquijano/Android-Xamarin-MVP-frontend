
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Json;
using Android.App;
using Android.Content;
using Android.OS;

using Android.Widget;

namespace miA
{
    [Activity(Label = "Appointment")]
    public class Appointment : ConfirmedActivity
    {
        Spinner minuteSpinner;

        string year ;
        string month ;
        string day ;
        string hour ;
        string selectedMinute = "00";
        string endTime;

        ResourceDefinition rd;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Appointment);

            var backButton = FindViewById<ImageButton>(Resource.Id.backButton);
            backButton.Click += (sender, e) => {
                this.Finish();
                OverridePendingTransition(0, 0);
            };

            string rdJson = Intent.GetStringExtra("rdJson");
            year = Intent.GetStringExtra("year");
            month = Intent.GetStringExtra("month");
            day = Intent.GetStringExtra("day");
            hour = Intent.GetStringExtra("hour");

            rd = ResourceDefinition.FromJson(rdJson);

            var resourceLabel = FindViewById<TextView>(Resource.Id.resourceLabel);
            var dateFrom = FindViewById<TextView>(Resource.Id.dateFrom);
            minuteSpinner = FindViewById<Spinner>(Resource.Id.minuteSpinner);
            var dateTo = FindViewById<TextView>(Resource.Id.dateTo);
            var comment = FindViewById<TextView>(Resource.Id.comment);

            resourceLabel.Text = rd.name;
            dateFrom.Text = Utilidades.getFormattedDate(year , month , day) + " - " + Utilidades.dateCheckCero(hour) + ":";

            setFinalDateText();

            minuteSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(minuteSpinnerSelected);
            var adapter = ArrayAdapter.CreateFromResource(
                this, Resource.Array.mintuesArray, Android.Resource.Layout.SimpleSpinnerItem);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            minuteSpinner.Adapter = adapter;

            //dateFrom.UpdateDate(Int32.Parse(year),Int32.Parse(month),Int32.Parse(day));
            //double doubleValue = Java.Lang.Double.ParseDouble((String)hour);
            //hourFrom.CurrentHour = (Java.Lang.Number) Java.Lang.Number. doubleValue;
            //hourFrom.CurrentHour = tempHour;
            //hourFrom.Hour = Int32.Parse(hour);
            //hourFrom.SetIs24HourView(Java.Lang.Boolean.False);

            var saveButton = FindViewById<Button>(Resource.Id.saveButton);
            saveButton.Click += (sender, e) =>
            {

                string resultadoValidacion = ValidarFormulario();
                if (resultadoValidacion == "")
                {

                    Utilidades.ConfirmDialog(this,"Atención","Confirma que desea registrar este agendamiento ?","SI","NO");

                }
                else Utilidades.showMessage(this, "Antención", resultadoValidacion, "OK");

                saveButton.Enabled = true;

            };

        }

        private void minuteSpinnerSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            selectedMinute = string.Format("{0}", spinner.GetItemAtPosition(e.Position));
            setFinalDateText();

        }

        void setFinalDateText(){

            var dateTo = FindViewById<TextView>(Resource.Id.dateTo);
            string baseDateText = Utilidades.getFormattedDate(year ,month , day);
            DateTime baseDate = DateTime.ParseExact(baseDateText, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var ts = new TimeSpan(Int32.Parse(hour), rd.minutes + Int32.Parse(selectedMinute), 0);
            DateTime newDate = baseDate.Add(ts);
            string newHour = newDate.Hour.ToString();
            string newMinute = newDate.Minute.ToString();


            dateTo.Text=Utilidades.getFormattedDate(newDate.Year.ToString(), newDate.Month.ToString(), newDate.Day.ToString()) + " - " + Utilidades.getFormattedHour(newHour, newMinute);
            endTime = Utilidades.getFormattedDate(newDate.Year.ToString(), newDate.Month.ToString(), newDate.Day.ToString()) + " " + Utilidades.getFormattedHour(newHour, newMinute);
        }

        private string ValidarFormulario()
        {

            //var comment = FindViewById<TextView>(Resource.Id.comment);

            string resutlado = "";
            /*
            if (dateFrom. == null || nombre.Text == "")
                resutlado = "Digita tu nombre completo.";
            */

            return resutlado;

        }


        public override void NegativeConfirm()
        {

        }

        public override void PositiveConfirm()
        {

            var data = new Dictionary<string, string>
            {
                ["resourceName"] = rd.name,
                ["comment"] = FindViewById<TextView>(Resource.Id.comment).Text,
                ["startTime"] = Utilidades.getFormattedDate(year, month, day) + " " + Utilidades.getFormattedHour(hour, selectedMinute),
                ["endTime"] = endTime,
                ["faPdb"] = Information.seletedForeignAgenda.pdb,
                ["faIdPdb"] = Information.seletedForeignAgenda.idPdb.ToString()
            };

            JsonValue resultado = Datos.saveForeginAppointment(data);

            if ((string)resultado["status"] == "OK" && (string)resultado["mensaje"] == "")
            {
                this.Finish();
                OverridePendingTransition(0, 0);
            }
            else
            {
                Utilidades.showMessage(this, "Antención", (string)resultado["mensaje"], "OK");
            }

        }


        public override void OnBackPressed()
        {
            Finish();
            OverridePendingTransition(0, 0);
        }

        protected override void OnResume()
        {
            base.OnResume();


        }

    }

}