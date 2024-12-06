// Author: Mia Ramos
// Date: December 1, 2024
// Email: miarramos29@gmail.com
// #####################################################################################################################################################
using SQLite;

namespace RXTrackByMiaRamosCo.Models {
    public class MedicationModel {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }  // Uniquely identifies each medication
        public string MedicationName { get; set; } // For medication's name
        public DateTime MedicationTime { get; set; } // For medication's time by using DateTime feature
        public string Dosage { get; set; } // For medication's dosage
        public bool IsTaken { get; set; } // To track if medication is taken
        public DateTime? TakenTime { get; set; } // To store when the medication was taken
        public bool IsEnabled { get; internal set; } // If Taken Button is Enabled or Disabled
        public object TakenButton { get; internal set; }
    }
}
