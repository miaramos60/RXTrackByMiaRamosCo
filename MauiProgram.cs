// Author: Mia Ramos
// Date: December 1, 2024
// Email: miarramos29@gmail.com
// #####################################################################################################################################################
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using RXTrackByMiaRamosCo.Models;
using System.IO;
using Microsoft.Maui.Storage;
using CommunityToolkit.Maui;
using Plugin.LocalNotification;
using RXTrackByMiaRamosCo;

namespace RXTrackByMiaRamosCo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit() // Enables the use of Maui Community Toolkit extension
                .UseLocalNotification() // Enables the use of Plugin.LocalNotification
                .ConfigureFonts(fonts => {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddTransient<Views.MedicationsView>();

            builder.Services.AddSingleton<MedicationDatabase>(serviceProvider => { // registers MedicationDatabase 
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "medication.db3");
                return new MedicationDatabase(dbPath);
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}