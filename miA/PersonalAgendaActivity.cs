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
    [Activity(Label = "Agenda Personal")]
    public class PersonalAgendaActivity : Activity
    {

        SfSchedule schedule;


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

            schedule.ScheduleView = ScheduleView.MonthView;

            schedule.CellTapped += (sender, e) =>
            {
                if (schedule.ScheduleView == ScheduleView.MonthView)
                {
                    schedule.ScheduleView = ScheduleView.DayView;
                }
                else if (schedule.ScheduleView == ScheduleView.DayView)
                {
                    schedule.ScheduleView = ScheduleView.MonthView;

                }

            };

            SetContentView(schedule);

        }

        public override void OnBackPressed()
        {

            if (schedule.ScheduleView == ScheduleView.MonthView)
            {
                Finish();
                OverridePendingTransition(0, 0);
            }
            else schedule.ScheduleView = ScheduleView.MonthView;


        }

        protected override void OnResume()
        {
            base.OnResume();


            JsonValue result = Datos.LlamarWsSync(Datos.sessionDataWebServiceUrl+"getPersonalAgenda", "-");
            schedule.Appointments.Clear();
            nonAccessibleBlocksCollection.Clear();
            tempClientAppointments.Clear();
            Meetings.Clear();


            if ((string)result["status"] == "OK" && (string)result["mensaje"] == "")
            {
                Datos.TruncateLocalPdbAgenda();

                foreach (JsonValue agendaRegister in result["agenda"])
                {

                    //DisableHourRange(startHour, endHour, "Tu Reserva", Color.DarkOrange);
                    //AddAppointment(startTimeData, endTimeData, (string)agendaRegister["resource_name"] + " : " + (string)agendaRegister["client_name"], (string)agendaRegister["comment"], Color.DarkOrange);
                    CreateAppointment((string)agendaRegister["start_time"], (string)agendaRegister["end_time"], (string)agendaRegister["resource_name"] , (string)agendaRegister["client_name"], (string)agendaRegister["comment"], Color.DarkOrange);

                    var pdbAgendaAppointment = new pdb_agenda{
                        startTime = Crypto.Encrypt((string) agendaRegister["start_time"],Utilidades.strangeFormElement+Datos.idUsuario),
                        endTime = Crypto.Encrypt((string)agendaRegister["end_time"], Utilidades.strangeFormElement+Datos.idUsuario),
                        client_name = Crypto.Encrypt((string)agendaRegister["client_name"], Utilidades.strangeFormElement+Datos.idUsuario),
                        fa_name = "",
                        resource_name = Crypto.Encrypt((string)agendaRegister["resource_name"], Utilidades.strangeFormElement+Datos.idUsuario),
                        comment = Crypto.Encrypt((string)agendaRegister["comment"], Utilidades.strangeFormElement+Datos.idUsuario)

                        };
                    Datos.InsertLocalAppointment(pdbAgendaAppointment);

                }

                //DayViewSettings dayViewSettings = new DayViewSettings();
                //dayViewSettings.NonAccessibleBlocks = nonAccessibleBlocksCollection;
                //schedule.DayViewSettings = dayViewSettings;

            }
            else
            {

                foreach (pdb_agenda agendaRegister in Datos.GetLocalAppointments())
                {
                    CreateAppointment(Crypto.Decrypt(agendaRegister.startTime.ToString(), Utilidades.strangeFormElement+Datos.idUsuario), 
                                      Crypto.Decrypt(agendaRegister.endTime.ToString(), Utilidades.strangeFormElement+Datos.idUsuario), 
                                      Crypto.Decrypt(agendaRegister.resource_name, Utilidades.strangeFormElement+Datos.idUsuario), 
                                      Crypto.Decrypt(agendaRegister.client_name, Utilidades.strangeFormElement+Datos.idUsuario), 
                                      Crypto.Decrypt(agendaRegister.comment, Utilidades.strangeFormElement+Datos.idUsuario), 
                                      Color.DarkOrange);
                }

                Toast.MakeText(this, "Error de conexión a la red." , ToastLength.Long).Show();

            }

            schedule.Appointments = Meetings;


        }

        private void CreateAppointment(string startTime, string endTime, string resourceName, string clientName, string comment,Color color)
        {

            string[] stSections = startTime.Split(' ');
            string stDate = stSections[0];
            string stHour = stSections[1];

            int startYear ;
            int startMonth ;
            int startDay  ;
            int startHour ;
            int startMinute ;

            int endYear ;
            int endMonth ;
            int endDay ;
            int endHour ;
            int endMinute ;   

            try
            {

                string[] stDateSections = stDate.Split('-');
                string[] stHourSections = stHour.Split(':');

                startYear = Int32.Parse(stDateSections[0]);
                startMonth = Int32.Parse(stDateSections[1]);
                startDay = Int32.Parse(stDateSections[2]);
                startHour = Int32.Parse(stHourSections[0]);
                startMinute = Int32.Parse(stHourSections[1]);

                string[] etSections = endTime.Split(' ');
                string etDate = etSections[0];
                string etHour = etSections[1];

                string[] etDateSections = etDate.Split('-');
                string[] etHourSections = etHour.Split(':');

                endYear = Int32.Parse(etDateSections[0]);
                endMonth = Int32.Parse(etDateSections[1]);
                endDay = Int32.Parse(etDateSections[2]);
                endHour = Int32.Parse(etHourSections[0]);
                endMinute = Int32.Parse(etHourSections[1]);                

            }
            catch (Exception ex)
            {
                string[] stDateSections = stDate.Split('/');
                string[] stHourSections = stHour.Split(':');

                startYear = Int32.Parse(stDateSections[2]);
                startMonth = Int32.Parse(stDateSections[1]);
                startDay = Int32.Parse(stDateSections[0]);
                startHour = Int32.Parse(stHourSections[0]);
                startMinute = Int32.Parse(stHourSections[1]);

                string[] etSections = endTime.Split(' ');
                string etDate = etSections[0];
                string etHour = etSections[1];

                string[] etDateSections = etDate.Split('/');
                string[] etHourSections = etHour.Split(':');

                endYear = Int32.Parse(etDateSections[2]);
                endMonth = Int32.Parse(etDateSections[1]);
                endDay = Int32.Parse(etDateSections[0]);
                endHour = Int32.Parse(etHourSections[0]);
                endMinute = Int32.Parse(etHourSections[1]);  
            }

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

            AddAppointment(startTimeData, endTimeData, resourceName + " : " + clientName, comment, color);


        }


        private void AddAppointment(Dictionary<string, int> startTimeData, Dictionary<string, int> endTimeData, string subject, string notes, Color color)
        {


            ScheduleAppointment meeting = new ScheduleAppointment();

            Calendar startTimeCalendar = Calendar.Instance;
            Calendar endTimeCalendar = Calendar.Instance;

            startTimeCalendar.Set(startTimeData["year"], startTimeData["month"] - 1, startTimeData["day"], startTimeData["hour"], startTimeData["minute"]);
            meeting.StartTime = startTimeCalendar;

            endTimeCalendar.Set(endTimeData["year"], endTimeData["month"] - 1, endTimeData["day"], endTimeData["hour"], endTimeData["minute"]);
            meeting.EndTime = endTimeCalendar;

            meeting.Color = color;
            meeting.Subject = subject;
            meeting.Notes = notes;
            Meetings.Add(meeting);


        }


        private void DisableHourRange(int startTime, int endTime, string subject, Color color)
        {

            //Create new instance of NonAccessibleBlock
            NonAccessibleBlock nonAccessibleBlock = new NonAccessibleBlock();

            nonAccessibleBlock.StartTime = startTime;
            nonAccessibleBlock.EndTime = endTime;
            nonAccessibleBlock.Text = subject;
            nonAccessibleBlock.Color = color;
            nonAccessibleBlocksCollection.Add(nonAccessibleBlock);

        }


    }


}