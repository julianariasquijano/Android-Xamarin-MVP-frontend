using Android.App;
using Android.OS;

using Com.Syncfusion.Schedule;
using Com.Syncfusion.Schedule.Enums;
using System.Collections.Generic;
using Java.Util;
using Android.Graphics;

using Com.Syncfusion.Charts;
using System.Collections.ObjectModel;


namespace miA
{
    [Activity(Label = "miA", MainLauncher = true)]
    public class MainActivity : Android.Support.V4.App.FragmentActivity
    {
        SfSchedule schedule;

        List<string> subjectCollection;
        List<string> colorCollection;
        ScheduleAppointmentCollection Meetings;
        int workStartHour = 9;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);




            schedule = new SfSchedule(this);
            schedule.MonthViewSettings.ShowAppointmentsInline = true;

            schedule.ScheduleView = ScheduleView.MonthView;

            schedule.CellTapped += (sender, e) => {
                if (schedule.ScheduleView == ScheduleView.MonthView)
                {
                    schedule.ScheduleView = ScheduleView.WeekView;
                }
                else if (schedule.ScheduleView == ScheduleView.WeekView){
                    schedule.ScheduleView = ScheduleView.DayView;
                }
                else if (schedule.ScheduleView == ScheduleView.DayView)
                {
                    schedule.ScheduleView = ScheduleView.MonthView;
                }
            };

            CreateAppointments();
            schedule.Appointments = Meetings;

            //SetContentView(schedule);
            ///////////////////////////////////////
            SfChart chart = new SfChart(this);

            //Initializing Primary Axis
            CategoryAxis primaryAxis = new CategoryAxis();

            chart.PrimaryAxis = primaryAxis;

            //Initializing Secondary Axis
            NumericalAxis secondaryAxis = new NumericalAxis();

            chart.SecondaryAxis = secondaryAxis;


            //*************************



            primaryAxis.Title.Text = "Name";

            chart.PrimaryAxis = primaryAxis;



            secondaryAxis.Title.Text = "Height (in cm)";

            chart.SecondaryAxis = secondaryAxis;

            //Initializing column series
            ColumnSeries series = new ColumnSeries();

            ViewModel viewModel = new ViewModel();

            series.ItemsSource = viewModel.Data;

            series.XBindingPath = "Name";

            series.YBindingPath = "Height";

            chart.Series.Add(series);

            //***************
            chart.Title.Text = "Chart";

            series.DataMarker.ShowLabel = true;

            chart.Legend.Visibility = Visibility.Visible;

            series.TooltipEnabled = true;


            SetContentView(chart);




            //SetContentView(Resource.Layout.Main);
        }

        private void CreateSubjectCollection()
        {
            subjectCollection = new List<string>();
            subjectCollection.Add("GoToMeeting");
            subjectCollection.Add("Business Meeting");
            subjectCollection.Add("Conference");
            subjectCollection.Add("Project Status Discussion");
            subjectCollection.Add("Auditing");
            subjectCollection.Add("Client Meeting");
            subjectCollection.Add("Generate Report");
            subjectCollection.Add("Target Meeting");
            subjectCollection.Add("General Meeting");
            subjectCollection.Add("Pay House Rent");
            subjectCollection.Add("Car Service");
            subjectCollection.Add("Medical Check Up");
            subjectCollection.Add("Wedding Anniversary");
            subjectCollection.Add("Sam's Birthday");
            subjectCollection.Add("Jenny's Birthday");
        }

        private void CreateColorCollection()
        {
            colorCollection = new List<string>();
            colorCollection.Add("#117EB4");
            colorCollection.Add("#B4112E");
            colorCollection.Add("#C44343");
            colorCollection.Add("#11B45E");
            colorCollection.Add("#43BEC4");
            colorCollection.Add("#B4112E");
            colorCollection.Add("#C44343");
            colorCollection.Add("#117EB4");
            colorCollection.Add("#C4435A");
            colorCollection.Add("#DF5348");
            colorCollection.Add("#43c484");
            colorCollection.Add("#11B49B");
            colorCollection.Add("#C44378");
            colorCollection.Add("#DF8D48");
            colorCollection.Add("#11B45E");
            colorCollection.Add("#43BEC4");
        }

        private void CreateAppointments()
        {
            Meetings = new ScheduleAppointmentCollection();
            Java.Util.Random randomTime = new Java.Util.Random();
            CreateSubjectCollection();
            CreateColorCollection();
            Calendar calendar = Calendar.Instance;
            Calendar DateFrom = Calendar.Instance;
            DateFrom.Add(CalendarField.Date, -10);
            Calendar DateTo = Calendar.Instance;
            DateTo.Add(CalendarField.Date, 10);
            Calendar dateRangeStart = Calendar.Instance;
            dateRangeStart.Add(CalendarField.Date, -3);
            Calendar dateRangeEnd = Calendar.Instance;
            dateRangeEnd.Add(CalendarField.Date, 3);
            for (calendar = DateFrom; calendar.Before(DateTo); calendar.Add(CalendarField.Date, 1))
            {
                if (calendar.After(dateRangeStart) && calendar.Before(dateRangeEnd))
                {
                    for (int AdditionalAppointmentIndex = 0; AdditionalAppointmentIndex < 3; AdditionalAppointmentIndex++)
                    {
                        ScheduleAppointment meeting = new ScheduleAppointment();
                        int hour = workStartHour + randomTime.NextInt(9);
                        Calendar startTimeCalendar = Calendar.Instance;
                        startTimeCalendar.Set(calendar.Get(CalendarField.Year),
                                              calendar.Get(CalendarField.Month),
                                              calendar.Get(CalendarField.Date),
                                              hour, 0);
                        meeting.StartTime = startTimeCalendar;
                        Calendar endTimeCalendar = Calendar.Instance;
                        endTimeCalendar.Set(calendar.Get(CalendarField.Year),
                                            calendar.Get(CalendarField.Month),
                                            calendar.Get(CalendarField.Date),
                                            hour + 1, 0);
                        meeting.EndTime = endTimeCalendar;
                        meeting.Color = Color.ParseColor(colorCollection[randomTime.NextInt(9)]);
                        meeting.Subject = subjectCollection[randomTime.NextInt(9)];
                        Meetings.Add(meeting);
                    }
                }
                else
                {
                    ScheduleAppointment meeting = new ScheduleAppointment();
                    int hour = workStartHour + randomTime.NextInt(9);
                    Calendar startTimeCalendar = Calendar.Instance;
                    startTimeCalendar.Set(calendar.Get(CalendarField.Year),
                                          calendar.Get(CalendarField.Month),
                                          calendar.Get(CalendarField.Date),
                                          hour, 0);
                    meeting.StartTime = startTimeCalendar;
                    Calendar endTimeCalendar = Calendar.Instance;
                    endTimeCalendar.Set(calendar.Get(CalendarField.Year),
                                        calendar.Get(CalendarField.Month),
                                        calendar.Get(CalendarField.Date),
                                        hour + 1, 0);
                    meeting.EndTime = endTimeCalendar;
                    meeting.Color = Color.ParseColor(colorCollection[randomTime.NextInt(9)]);
                    meeting.Subject = subjectCollection[randomTime.NextInt(9)];
                    Meetings.Add(meeting);
                }
            }
        }



    }


    public class Person
    {
        public string Name { get; set; }

        public double Height { get; set; }
    }

    public class ViewModel
    {
        public ObservableCollection<Person> Data { get; set; }

        public ViewModel()
        {
            Data = new ObservableCollection<Person>()
            {
                new Person { Name = "David", Height = 180 },
                new Person { Name = "Michael", Height = 170 },
                new Person { Name = "Steve", Height = 160 },
                new Person { Name = "Joel", Height = 182 }
            };
        }
    }

}