// Author: Mia Ramos
// Date: December 1, 2024
// Email: miarramos29@gmail.com
// #####################################################################################################################################################
using CommunityToolkit.Maui.Views;
using RXTrackByMiaRamosCo.Models;
using System.Collections.ObjectModel;
using System.Linq;
using Plugin.LocalNotification;

namespace RXTrackByMiaRamosCo.Views
{
    public partial class MedicationsView : ContentPage
    {
        private readonly MedicationDatabase _medicationDatabase;
        ObservableCollection<MedicationModel> Medications;

        // Constructor with Dependency Injection
        public MedicationsView()
        {
            InitializeComponent(); // Initializer
            // Initializes database using filepath 
            _medicationDatabase = new MedicationDatabase(Path.Combine(FileSystem.AppDataDirectory, "medication.db3"));
            Medications = new ObservableCollection<MedicationModel>();
            MedicationsList.ItemsSource = Medications;

            LoadMedicationsForToday(); // Based on Database, medications will load (allows to save medicationlist even if app is closed out and update list after deleting a medication)
        }
        //Reloads and checks medications that might be due
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Reload medications whenever the page appears
            LoadMedicationsForToday();
            CheckForDueMedications();
        }
        // This will take data from database and load medications accordingly
        private async void LoadMedicationsForToday()
        {
            // Chronologically orders the list of medications based on time
            var medications = await _medicationDatabase.GetMedicationsForDateAsync(DateTime.Today);

            // Reload -- Clears medications only to reload ones from database (forloop is used so that it does all in database and ends untill there are no more)
            Medications.Clear();
            foreach (var medication in medications)
            {
                Medications.Add(medication);
            }

            // The following will help disable the taken button once user marks it as taken
            foreach (var medication in medications)
            {

                if (medication.IsTaken)
                {
                    var button = MedicationsList.ItemsSource.Cast<MedicationModel>()
                        .FirstOrDefault(m => m.ID == medication.ID);
                    // If the medication is taken, then the button will no longer be enabled
                    if (button != null)
                    {
                        button.IsEnabled = false;// Disables the taken button
                    }
                }
            }
        }
        // Add Medication button will open the MedicationPopup
        private async void MedicationPopup_Clicked(object sender, EventArgs e)
        {
            var popup = new MedicationPopup();
            var result = await this.ShowPopupAsync(popup);

            if (result != null)
            {
                var medication = (MedicationModel)result; // sends to Medication Model 
                await _medicationDatabase.SaveMedicationAsync(medication); // saves to database
                LoadMedicationsForToday(); // Loads it up to the list
            }
        }
        // When the user marks a medication as taken:
        private async void TakenButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var medication = button?.BindingContext as MedicationModel;

            if (medication != null)
            {
                medication.IsTaken = true; // Medication is noted as taken
                medication.TakenTime = DateTime.Now; // After that, it will record the time the button was pressed

                await _medicationDatabase.SaveMedicationAsync(medication); // Saves to database
                button.IsEnabled = false; // Button will become disabled

                LoadMedicationsForToday(); // Loads it up to the list with the disabled button
            }
        }
        // When the user removes a medication:
        private async void RemoveButtonClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var medication = button?.BindingContext as MedicationModel;

            if (medication != null)
            {
                await _medicationDatabase.DeleteMedicationAsync(medication); // deletes it from database
                LoadMedicationsForToday(); // Loads the updated list without the deleted medication
            }
        }
        //This will check if medications are due -- helps app know when to send notifications
        private void CheckForDueMedications()
        {
            var currentTime = DateTime.Now; // currentTime will be the actual current time

            foreach (var medication in Medications)
            {
                // If the medication has not been taken by the user ... 
                if (!medication.IsTaken && IsTimeForMedication(medication.MedicationTime, currentTime))
                {
                    ScheduleNotification(medication); // Schedules notification
                }
            }
        }
        //Sets up a wiggle-room of 5 minutes around the due time for the notification
        private bool IsTimeForMedication(DateTime medicationTime, DateTime currentTime)
        {
            var windowBefore = medicationTime.AddMinutes(-5); // 5 minutes before
            var windowAfter = medicationTime.AddMinutes(5); // 5 minutes after

            return currentTime >= windowBefore && currentTime <= windowAfter; // confirms that the current time is in range set above
        }
        //Notification undergoes scheduling here: 
        private void ScheduleNotification(MedicationModel medication)
        {
            var currentTime = DateTime.Now; // records the current time

            // notifyTime will be the same as the MedicationTime as saved in database
            var notifyTime = medication.MedicationTime;
            //Creates Notification Request
            var notificationRequest = new NotificationRequest
            {
                NotificationId = 022, // Gives notification an id number
                Title = "Medication Reminder", // Name of Notification -- Bold portion of notification
                Description = $"{medication.MedicationName} ({medication.Dosage}) is due", // Example: Keppra (500 mg) is due
                BadgeNumber = 42, // Notifications need specific badge numbers for the app 
                CategoryType = NotificationCategoryType.Reminder, // Types notification as a reminder
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notifyTime //Sets notification time to be the same as the time when the medication is due
                }
            };
            LocalNotificationCenter.Current.Show(notificationRequest); // Sends notification through the LocalNotificationCenter
        }
    }
}