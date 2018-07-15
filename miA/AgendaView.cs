﻿using System;
using Android.App;
using Android.Content;
using Android.OS;

using Android.Widget;
using Com.Syncfusion.Schedule;
using Com.Syncfusion.Schedule.Enums;
using Java.Util;


namespace miA
{
    [Activity(Label = "Agenda")]
    public class AgendaView : Activity
    {

        SfSchedule schedule;
        ResourceDefinition rd;

        bool showResourceName = true;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            schedule = new SfSchedule(this);
            schedule.Locale = new Locale("es", "ES");
            schedule.MonthViewSettings.ShowAppointmentsInline = true;

            string rdJson = Intent.GetStringExtra("rdJson");
            rd = ResourceDefinition.FromJson(rdJson);

            schedule.ScheduleView = ScheduleView.MonthView;

            schedule.CellTapped += (sender, e) =>
            {
                if (schedule.ScheduleView == ScheduleView.MonthView)
                {
                    schedule.ScheduleView = ScheduleView.DayView;
                }
                else if (schedule.ScheduleView == ScheduleView.WeekView)
                {
                    schedule.ScheduleView = ScheduleView.DayView;
                }
                else if (schedule.ScheduleView == ScheduleView.DayView)
                {
                    //string horaCita ="Año: "+ e.Calendar.Get(CalendarField.Year).ToString() +
                    //                           " Mes: " + (Int32.Parse(e.Calendar.Get(CalendarField.Month).ToString()) + 1).ToString() +
                    //                           " Dia: " + e.Calendar.Get(CalendarField.DayOfMonth).ToString() +
                    //                           " Hora: " + e.Calendar.Get(CalendarField.HourOfDay).ToString();
                    //Utilidades.showMessage(this,"",horaCita,"OK");

                    var intent = new Intent(this, typeof(Appointment));

                    intent.PutExtra("rdJson", rdJson);
                    intent.PutExtra("year", e.Calendar.Get(CalendarField.Year).ToString());
                    intent.PutExtra("month", (Int32.Parse(e.Calendar.Get(CalendarField.Month).ToString()) + 1).ToString());
                    //intent.PutExtra("month", e.Calendar.Get(CalendarField.Month).ToString());
                    intent.PutExtra("day", e.Calendar.Get(CalendarField.DayOfMonth).ToString());
                    intent.PutExtra("hour", e.Calendar.Get(CalendarField.HourOfDay).ToString());
                    StartActivityForResult(intent, 0);
                    OverridePendingTransition(0, 0);

                }

            };

            SetContentView(schedule);

        }

        public override void OnBackPressed()
        {
            Finish();
            OverridePendingTransition(0, 0);
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (showResourceName)
            {
                Toast.MakeText(this, "Recurso: " + rd.name, ToastLength.Long).Show();
                showResourceName = false;
            }

        }
    }
}