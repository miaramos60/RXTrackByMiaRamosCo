// Author: Mia Ramos
// Date: December 1, 2024
// Email: miarramos29@gmail.com
// #####################################################################################################################################################
using CommunityToolkit.Maui.Views;
using RXTrackByMiaRamosCo.Models;
namespace RXTrackByMiaRamosCo.Views;

public partial class MedicationPopup : Popup {
	public MedicationPopup() {
		InitializeComponent();
	}
    // When the Cancel Button is clicked, the popup will close
    private void CancelButton_Clicked(object sender, EventArgs e) {
        Close();
    }
    private void SaveButton_Clicked(object sender, EventArgs e) {
        // Will reflect today's date and time
        var medicationTime = DateTime.Today.Add(MedicationTime.Time);

        // Create a new MedicationModel with the updated MedicationTime
        MedicationModel medication = new MedicationModel { 
            // The following 3 lines are input from user which gathers Time, Name, and Dosage of Medication
            MedicationTime = medicationTime, 
            MedicationName = MedicationName.Text,
            Dosage = Dosage.Text,
            IsTaken = false,  // As a Default, Medications will be loaded and added as "Not Taken"
            TakenTime = DateTime.MinValue // Default value for TakenTime
        };

        // After fields are filled out and save is clicked, the popup will close
        Close(medication);
    }

}