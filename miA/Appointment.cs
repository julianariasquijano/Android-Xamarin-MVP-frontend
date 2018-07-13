
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
    [Activity(Label = "Appointment")]
    public class Appointment : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Appointment);

            string year = Intent.GetStringExtra("year");
            string month = Intent.GetStringExtra("month");
            string day = Intent.GetStringExtra("day");
            string hour = Intent.GetStringExtra("hour");

            var dateFrom = FindViewById<DatePicker>(Resource.Id.dateFrom);
            var dateTo = FindViewById<DatePicker>(Resource.Id.dateTo);
            var hourFrom = FindViewById<TimePicker>(Resource.Id.hourFrom);
            var hourTo = FindViewById<TimePicker>(Resource.Id.hourTo);
            var comment = FindViewById<EditText>(Resource.Id.comment);

            dateFrom.UpdateDate(Int32.Parse(year),Int32.Parse(month),Int32.Parse(day));
            //double doubleValue = Java.Lang.Double.ParseDouble((String)hour);
            //hourFrom.CurrentHour = (Java.Lang.Number) Java.Lang.Number. doubleValue;
            //hourFrom.CurrentHour = tempHour;
            //hourFrom.Hour = Int32.Parse(hour);
            hourFrom.SetIs24HourView(Java.Lang.Boolean.False);

            var saveButton = FindViewById<Button>(Resource.Id.saveButton);
            saveButton.Click += (sender, e) =>
            {

                string resultadoValidacion = ValidarFormulario();
                if (resultadoValidacion == "")
                {




                    saveButton.Enabled = false;
                    /*
                    var datos = new Dictionary<string, string>
                    {
                        ["dateFrom"] = nombre.Text,
                        ["dateTo"] = mail.Text,
                        ["hourFrom"] = telefono.Text,
                        ["hourTo"] = tempPassword
                    };


                    JsonValue resultado = Datos.editarRegistro(datos);

                    if ((string)resultado["status"] == "OK" && (string)resultado["mensaje"] == "")
                    {
                        this.Finish();
                        OverridePendingTransition(0, 0);

                    }
                    else
                    {

                        Utilidades.showMessage(this, "Antención", (string)resultado["mensaje"], "OK");

                    }
                    */

                }
                else Utilidades.showMessage(this, "Antención", resultadoValidacion, "OK");

                saveButton.Enabled = true;

            };

        }

        private string ValidarFormulario()
        {

            var dateFrom = FindViewById<DatePicker>(Resource.Id.dateFrom);
            var dateTo = FindViewById<DatePicker>(Resource.Id.dateTo);

            string resutlado = "";
            /*
            if (dateFrom. == null || nombre.Text == "")
                resutlado = "Digita tu nombre completo.";
            */

            return resutlado;

        }

    }

}
