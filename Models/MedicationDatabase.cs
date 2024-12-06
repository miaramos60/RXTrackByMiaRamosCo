// Author: Mia Ramos
// Date: December 1, 2024
// Email: miarramos29@gmail.com
// #####################################################################################################################################################
using SQLite;
using RXTrackByMiaRamosCo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RXTrackByMiaRamosCo.Models {
    public class MedicationDatabase {
        private readonly SQLiteAsyncConnection _database;

        // Initializes the database connection
        public MedicationDatabase(string dbPath) {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<MedicationModel>().Wait();  // Creates table if it doesn't exist
        }

        // Add or update a medication in the database
        public Task<int> SaveMedicationAsync(MedicationModel medication) {
            if (medication.ID != 0) { // Updates medication id  
                return _database.UpdateAsync(medication); // updates id if the medication already exists
            }
            else {
                // If it doesn't have an id, it puts it in the database
                return _database.InsertAsync(medication); 
            }
        }

        // Gets all medications in the database
        public Task<List<MedicationModel>> GetMedicationsAsync() =>
            _database.Table<MedicationModel>().ToListAsync(); // Will return a list of all the medications

        // Deletes a medication from the database
        public Task<int> DeleteMedicationAsync(MedicationModel medication) {
            return _database.DeleteAsync(medication);
        }

        // Gets medications for a specific date which is organized by chronological time
        public Task<List<MedicationModel>> GetMedicationsForDateAsync(DateTime date) {
            var startOfDay = date.Date; // Sets start time to 00:00:00
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1); // Sets end time to 23:59:59.9999999

            // Queries medications for the specific day and ordered chronological time
            return _database.Table<MedicationModel>()
                            .Where(m => m.MedicationTime >= startOfDay && m.MedicationTime <= endOfDay)
                            .OrderBy(m => m.MedicationTime) // Sorts meds according to MedicationTime
                            .ToListAsync();
        }

        // Collects medications for today
        public Task<List<MedicationModel>> GetMedicationsForTodayAsync() {
            var today = DateTime.Today; // Gets current date (Today)
            return GetMedicationsForDateAsync(today); // Reusing the existing method in order to collect today's medication(s)
        }
    }
}
