using System;
using System.Collections.Generic;
using System.Json;
using Android.App;
using Android.Content;
using Android.Graphics;
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


        List<string> colorCollection;
        ScheduleAppointmentCollection Meetings;
        NonAccessibleBlocksCollection nonAccessibleBlocksCollection;

        List<TempClientAppointment> tempClientAppointments;


        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);

            Meetings = new ScheduleAppointmentCollection();
            nonAccessibleBlocksCollection = new NonAccessibleBlocksCollection();
            tempClientAppointments = new List<TempClientAppointment>();

            schedule = new SfSchedule(this); schedule.SetBackgroundColor(Color.Brown);
            schedule.Locale = new Locale("es", "ES");
            schedule.MonthViewSettings.ShowAppointmentsInline = true;

            var dayViewSettings = new DayViewSettings();
            dayViewSettings.WorkStartHour = 7;
            dayViewSettings.WorkEndHour = 19;

            schedule.DayViewSettings = dayViewSettings;

            string rdJson = Intent.GetStringExtra("rdJson");
            rd = ResourceDefinition.FromJson(rdJson);

            schedule.ScheduleView = ScheduleView.MonthView;


            schedule.CellTapped += (sender, e) =>
            {
                if (schedule.ScheduleView == ScheduleView.MonthView)
                {
                    schedule.ScheduleView = ScheduleView.DayView;
                }
                else if (schedule.ScheduleView == ScheduleView.DayView)
                {

                    bool makeNewAppointment = true;

                    int incomingYear = e.Calendar.Get(CalendarField.Year);
                    int incomingMonth = e.Calendar.Get(CalendarField.Month);
                    int incomingDay = e.Calendar.Get(CalendarField.DayOfMonth);
                    int incomingHour = e.Calendar.Get(CalendarField.HourOfDay);
                    int incomingMinute = e.Calendar.Get(CalendarField.Minute);


                    foreach (var actualAppointment in tempClientAppointments)
                    {
                        if (actualAppointment.year == incomingYear)
                        {
                            if (actualAppointment.month  == incomingMonth + 1)
                            {

                                if (actualAppointment.day == incomingDay)
                                {

                                    if (actualAppointment.hour == incomingHour)
                                    {

                                        if (actualAppointment.minute == incomingMinute)
                                        {
                                            makeNewAppointment = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (makeNewAppointment)
                    {
                        var intent = new Intent(this, typeof(Appointment));

                        intent.PutExtra("rdJson", rdJson);
                        intent.PutExtra("year", e.Calendar.Get(CalendarField.Year).ToString());
                        intent.PutExtra("month", (Int32.Parse(e.Calendar.Get(CalendarField.Month).ToString()) + 1).ToString());
                        intent.PutExtra("day", e.Calendar.Get(CalendarField.DayOfMonth).ToString());
                        intent.PutExtra("hour", e.Calendar.Get(CalendarField.HourOfDay).ToString());
                        StartActivityForResult(intent, 0);
                        OverridePendingTransition(0, 0);
                    }

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


            var data = new Dictionary<string, string>
            {
                ["faPdb"] = Information.seletedForeignAgenda.pdb,
                ["faIdPdb"] = Information.seletedForeignAgenda.idPdb.ToString(),
                ["resourceName"] = rd.name
            };


            JsonValue result = Datos.getResourceAgenda(data);
            schedule.Appointments.Clear();
            nonAccessibleBlocksCollection.Clear();
            tempClientAppointments.Clear();

            if ((string)result["status"] == "OK" && (string)result["mensaje"] == "")
            {

                foreach (JsonValue agendaRegister in result["agenda"])
                {

                    string startTime = agendaRegister["start_time"];
                    string endTime = agendaRegister["end_time"];

                    string[] stSections = startTime.Split(' ');
                    string stDate = stSections[0];
                    string stHour = stSections[1];

                    string[] stDateSections = stDate.Split('-');
                    string[] stHourSections = stHour.Split(':');

                    int startYear = Int32.Parse(stDateSections[0]);
                    int startMonth = Int32.Parse(stDateSections[1]);
                    int startDay = Int32.Parse(stDateSections[2]);
                    int startHour = Int32.Parse(stHourSections[0]);
                    int startMinute = Int32.Parse(stHourSections[1]);

                    string[] etSections = endTime.Split(' ');
                    string etDate = etSections[0];
                    string etHour = etSections[1];

                    string[] etDateSections = etDate.Split('-');
                    string[] etHourSections = etHour.Split(':');

                    int endYear = Int32.Parse(etDateSections[0]);
                    int endMonth = Int32.Parse(etDateSections[1]);
                    int endDay = Int32.Parse(etDateSections[2]);
                    int endHour = Int32.Parse(etHourSections[0]);
                    int endMinute = Int32.Parse(etHourSections[1]);

                    var startTimeData = new Dictionary<string, int>();
                    var endTimeData = new Dictionary<string, int>();

                    startTimeData["year"] = startYear;
                    startTimeData["month"] = startMonth;
                    startTimeData["day"] = startDay;
                    startTimeData["hour"] = startHour;
                    startTimeData["minute"] = startMinute;

                    endTimeData["year"] = endYear;
                    endTimeData["month"] = endMonth;
                    endTimeData["day"] = endDay;
                    endTimeData["hour"] = endHour;
                    endTimeData["minute"] = endMinute;

                    var thisAppointment = new TempClientAppointment();
                    thisAppointment.year = startYear;
                    thisAppointment.month = startMonth;
                    thisAppointment.day = startDay;
                    thisAppointment.hour = startHour;
                    thisAppointment.minute = startMinute;
                    thisAppointment.pdb = (string)agendaRegister["client_pdb"];
                    thisAppointment.idPdb = agendaRegister["client_id_pdb"];

                    tempClientAppointments.Add(thisAppointment);

                    if ((string)agendaRegister["client_pdb"] == Datos.pdb && agendaRegister["client_id_pdb"] == Int32.Parse(Datos.idPdb))
                    {
                        //DisableHourRange(startHour, endHour, "Tu Reserva", Color.DarkOrange);
                        CreateAppointment(startTimeData, endTimeData, (string)agendaRegister["client_name"], (string)agendaRegister["comment"], Color.DarkOrange);

                    }
                    else CreateAppointment(startTimeData, endTimeData, "No Disponible", "", Color.DarkBlue);


                }

                //DayViewSettings dayViewSettings = new DayViewSettings();
                //dayViewSettings.NonAccessibleBlocks = nonAccessibleBlocksCollection;
                //schedule.DayViewSettings = dayViewSettings;
                schedule.Appointments = Meetings;
            }
            else
            {

                Intent intent = new Intent(this, typeof(ForeignResourceView));
                intent.PutExtra("networkError", "");
                SetResult(Result.Ok, intent);
                Finish();
                OverridePendingTransition(0, 0);

            }



        }


        private void CreateAppointment(Dictionary<string,int> startTimeData,Dictionary<string, int> endTimeData,string subject,string notes,Color color)
        {
            Java.Util.Random randomTime = new Java.Util.Random();

            Calendar calendar = Calendar.Instance;
            Calendar DateFrom = Calendar.Instance;
            DateFrom.Add(CalendarField.Date, -10);
            Calendar DateTo = Calendar.Instance;
            DateTo.Add(CalendarField.Date, 10);
            Calendar dateRangeStart = Calendar.Instance;
            dateRangeStart.Add(CalendarField.Date, -3);
            Calendar dateRangeEnd = Calendar.Instance;
            dateRangeEnd.Add(CalendarField.Date, 3);

            ScheduleAppointment meeting = new ScheduleAppointment();

            Calendar startTimeCalendar = Calendar.Instance;
            Calendar endTimeCalendar = Calendar.Instance;

            startTimeCalendar.Set(startTimeData["year"], startTimeData["month"] - 1, startTimeData["day"],startTimeData["hour"], startTimeData["minute"]);
            meeting.StartTime = startTimeCalendar;

            endTimeCalendar.Set(endTimeData["year"], endTimeData["month"] - 1, endTimeData["day"], endTimeData["hour"], endTimeData["minute"]);
            meeting.EndTime = endTimeCalendar;

            meeting.Color = color;
            meeting.Subject = subject;
            meeting.Notes = notes;
            Meetings.Add(meeting);


        }


        private void DisableHourRange(int startTime,int endTime, string subject, Color color)
        {

            //Create new instance of NonAccessibleBlock
            NonAccessibleBlock nonAccessibleBlock = new NonAccessibleBlock();

            nonAccessibleBlock.StartTime = startTime;
            nonAccessibleBlock.EndTime = endTime;
            nonAccessibleBlock.Text = subject;
            nonAccessibleBlock.Color = color;
            nonAccessibleBlocksCollection.Add(nonAccessibleBlock);

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


    }


    public class TempClientAppointment{
        public int year;
        public int month;
        public int day;
        public int hour;
        public int minute;
        public string pdb;
        public int idPdb;

    }

}