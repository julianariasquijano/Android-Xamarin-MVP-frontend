
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Syncfusion.Schedule;
using Com.Syncfusion.Schedule.Enums;
using Java.Util;
using Android.Graphics;

using System.Collections.ObjectModel;

namespace miA
{
    [Activity(Label = "Agenda")]
    public class AgendaView : Activity
    {

        SfSchedule schedule;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            //SetContentView(Resource.Layout.AgendaView);

            //var backButton = FindViewById<ImageButton>(Resource.Id.backButton);
            //backButton.Click += (sender, e) => {
            //    this.Finish();
            //    OverridePendingTransition(0, 0);
            //};

            //var homeButton = FindViewById<ImageButton>(Resource.Id.homeButton);
            //homeButton.Click += (sender, e) => {
            //    var intent = new Intent(this, typeof(MainActivity));
            //    intent.AddFlags(ActivityFlags.ClearTop);
            //    StartActivity(intent);
            //    OverridePendingTransition(0, 0);

            //};


            schedule = new SfSchedule(this);
            schedule.Locale = new Locale("es", "ES");
            schedule.MonthViewSettings.ShowAppointmentsInline = true;

            schedule.ScheduleView = ScheduleView.MonthView;

            schedule.CellTapped += (sender, e) =>
            {
                if (schedule.ScheduleView == ScheduleView.MonthView)
                {
                    schedule.ScheduleView = ScheduleView.WeekView;
                }
                else if (schedule.ScheduleView == ScheduleView.WeekView)
                {
                    schedule.ScheduleView = ScheduleView.DayView;
                }
                else if (schedule.ScheduleView == ScheduleView.DayView)
                {
                    schedule.ScheduleView = ScheduleView.MonthView;
                }

            };
            schedule.AddView(new Button(this));
            //var layout = FindViewById<LinearLayout>(Resource.Id.scheduleViewLayout);
            //layout.AddView(schedule);

            SetContentView(schedule);


        }

        public override void OnBackPressed()
        {
            Finish();
            OverridePendingTransition(0, 0);
        }

    }
}
