using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using SunnyDay.Backend.DataObjects;
using SunnyDay.Backend.Models;
using Owin;

namespace SunnyDay.Backend
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            // Register the StorageController routes
            config.MapHttpAttributeRoutes();

            new MobileAppConfiguration()
                .UseDefaultConfiguration()
                .ApplyTo(config);

            // Use Entity Framework Code First to create database tables based on your DbContext
            Database.SetInitializer(new MobileServiceInitializer());

            MobileAppSettingsDictionary settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
                {
                    // This middleware is intended to be used locally for debugging. By default, HostName will
                    // only have a value when running in an App Service application.
                    SigningKey = ConfigurationManager.AppSettings["SigningKey"],
                    ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                    ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                    TokenHandler = config.GetAppServiceTokenHandler()
                });
            }

            app.UseWebApi(config);
        }
    }

    public class MobileServiceInitializer : CreateDatabaseIfNotExists<MobileServiceContext>
    {
        protected override void Seed(MobileServiceContext context)
        {
            List<User> users = new List<User>
            {
                new User { Id = Guid.NewGuid().ToString(), UserId = $"sid:{Guid.NewGuid().ToString()}", GroupId = "", Name = "Yotam S", Email = "", SkinTone = 0, SpfLevel = 50, IsVerified = true, ImageUrl = ""},
                new User { Id = Guid.NewGuid().ToString(), UserId = $"sid:{Guid.NewGuid().ToString()}", GroupId = "", Name = "Mia J", Email = "", SkinTone = 2, SpfLevel = 15, IsVerified = true, ImageUrl = ""}
            };

            List<UvReading> uvReadings = new List<UvReading>
            {
                new UvReading {Id = Guid.NewGuid().ToString(), ReadTime = DateTime.Now, UserId = users[0].Id, LocalIndex = 1, PublicIndex = 3, Description = "Low", ReadLocation = "Tel-Aviv", LightIntensity = 500, DailyExposure = 10},
                new UvReading {Id = Guid.NewGuid().ToString(), ReadTime = DateTime.Now, UserId = users[0].Id, LocalIndex = 1, PublicIndex = 3, Description = "Low", ReadLocation = "Tel-Aviv", LightIntensity = 500, DailyExposure = 12},
                new UvReading {Id = Guid.NewGuid().ToString(), ReadTime = DateTime.Now, UserId = users[1].Id, LocalIndex = 1, PublicIndex = 3, Description = "Low", ReadLocation = "Tokyo", LightIntensity = 100, DailyExposure = 10},
                new UvReading {Id = Guid.NewGuid().ToString(), ReadTime = DateTime.Now, UserId = users[1].Id, LocalIndex = 0, PublicIndex = 0, Description = "None", ReadLocation = "Tokyo", LightIntensity = 100, DailyExposure = 14}
            };

            List<Alarm> alarms = new List<Alarm>
            {
                new Alarm {Id = Guid.NewGuid().ToString(), UserId = users[0].UserId, GroupId = "", StartTime = DateTime.Now, EndTime = DateTime.Now, DurationSeconds = 60, IsActive = false, Text = "This is an alarm"},
                new Alarm {Id = Guid.NewGuid().ToString(), UserId = users[1].UserId, GroupId = "", StartTime = DateTime.Now, EndTime = DateTime.Now, DurationSeconds = 120, IsActive = false, Text = "This is another alarm"}
            };

            foreach (User item in users)
            {
                context.Set<User>().Add(item);
            }
            foreach (Alarm item in alarms)
            {
                context.Set<Alarm>().Add(item);
            }
            foreach (UvReading item in uvReadings)
            {
                context.Set<UvReading>().Add(item);
            }

            base.Seed(context);
        }
    }
}

