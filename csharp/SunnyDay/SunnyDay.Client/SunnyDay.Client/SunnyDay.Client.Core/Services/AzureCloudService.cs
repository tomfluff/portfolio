using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Files;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Connectivity;
using SunnyDay.Client.Core.Authentication;
using SunnyDay.Client.Core.Helpers;
using SunnyDay.Client.Core.Models;
using SunnyDay.Client.Core.Models.UserReport;
using SunnyDay.Client.Core.Static;
using SunnyDay.Client.Core.Utils;
using Xamarin.Forms;
using UvReading = SunnyDay.Client.Core.Models.UvReading;

namespace SunnyDay.Client.Core.Services
{
    public class AzureCloudService : ICloudService
    {
        public MobileServiceClient Client { get; private set; } = null;
        private IMobileServiceSyncTable<User> _users;
        private IMobileServiceSyncTable<Alarm> _alarms;
        private IMobileServiceSyncTable<UvReading> _uvReadings;
        private bool _isInitialized = false;
        private bool _isWorking = false;

        public AzureCloudService()
        {
            var appUrl = Static.Keys.AzureServiceUrl;

            // Create the client
            Client = new MobileServiceClient(appUrl, new AuthHandler())
            {
                SerializerSettings = new MobileServiceJsonSerializerSettings()
                {
                    CamelCasePropertyNames = false
                }
            };

            Debug.WriteLine("> Azure Client initiated");

            if (!string.IsNullOrWhiteSpace(Settings.AuthToken) && !string.IsNullOrWhiteSpace(Settings.UserId))
            {
                Debug.WriteLine("> User already logged in");
                Client.CurrentUser = new MobileServiceUser(Settings.UserId);
                Client.CurrentUser.MobileServiceAuthenticationToken = Settings.AuthToken;
            }
            else
            {
                Debug.WriteLine("> User not logged in");
            }
            Debug.WriteLine("> Azure client initiation complete");
        }

        public async Task InitializeAsync()
        {
            /*while (!_isWorking)
            {
                if (_isInitialized) return;
            }*/

            _isWorking = true;

            if (Client?.SyncContext?.IsInitialized ?? false)
                return;

            if (_isInitialized)
                return;

            try
            {
                Debug.WriteLine("> Azure Service initializing...");

                // Create local database and initialize it for local-cloud communication
                var store = new MobileServiceSQLiteStore("sunnyday.db");
                store.DefineTable<User>();
                store.DefineTable<Alarm>();
                store.DefineTable<UvReading>();

                Debug.WriteLine("> Local DB created");

                // Take care of table operations errors and push completion results
                // Initialize the sync context
                await Client.SyncContext.InitializeAsync(store,
                    new MobileServiceSyncHandler());

                // Get the sync table that will call out to azure
                _users = Client.GetSyncTable<User>();
                _alarms = Client.GetSyncTable<Alarm>();
                _uvReadings = Client.GetSyncTable<UvReading>();

                _isInitialized = true;
                _isWorking = false;

                // Sync data with the server
                await SynchronizeServiceAsync();
                Debug.WriteLine("> Initialize complete");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Source} : {e} ({e.InnerException})");
            }
            finally
            {
                _isWorking = false;
            }
        }

        public async Task SynchronizeServiceAsync()
        {
            var connected =
                await CrossConnectivity.Current.IsRemoteReachable(Static.Keys.AzureServiceUrl);
            if (!connected || _isWorking)
            {
                return;
            }

            Debug.WriteLine("> Trying to sync data...");

            try
            {
                var id = Settings.UserId;

                await Client.SyncContext.PushAsync();

                await _users.PullAsync("allUsers", _users.CreateQuery().Where(v => v.GroupId == id));
                await _alarms.PullAsync("allAlarms", _alarms.CreateQuery().Where(v => v.UserId == id));
                await _uvReadings.PullAsync("allUvReadings", _uvReadings.CreateQuery().Where(v => v.UserId == id));

                Debug.WriteLine("> Finished syncing!");
            }
            catch (MobileServicePushFailedException)
            {
                return;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Source} : {e} ({e.InnerException})");
            }
        }

        public async Task<User> GetUser(string userId)
        {
            await InitializeAsync();

            var user = (await _users.ReadAsync(_users.CreateQuery().Where(u => u.UserId == userId))).FirstOrDefault();
            return user;
        }

        public async Task<IEnumerable<User>> GetUserSubGroup(string groupId)
        {
            await InitializeAsync();

            var users = await _users.ReadAsync(_users.CreateQuery().Where(u => (u.GroupId == groupId && u.UserId != groupId)));
            return users;
        }

        public async Task<User> AddUser(string name, string email, int skinTone, int spfLevel, bool isVerified, string imageUrl)
        {
            await InitializeAsync();

            var userT = new User()
            {
                UserId = isVerified ? Settings.UserId : $"sub:{Guid.NewGuid():N}",
                GroupId = Settings.UserId,
                Name = name,
                Email = email,
                SkinTone = skinTone,
                SpfLevel = spfLevel,
                IsVerified = isVerified,
                ImageUrl = imageUrl
            };

            // Save locally
            await _users.InsertAsync(userT);

            // Syncronize with cloud
            await SynchronizeServiceAsync();
            return userT;
        }

        public async Task<User> UpdateUser(User user)
        {
            await InitializeAsync();

            await _users.UpdateAsync(user);
            Debug.WriteLine($"> Updated {user.Name} ({user.Id})");

            // Syncronize with cloud
            await SynchronizeServiceAsync();
            return user;
        }

        public async Task DeleteUser(User user)
        {
            await InitializeAsync();

            await _users.DeleteAsync(user);

            await SynchronizeServiceAsync();
        }

        public async Task<Alarm> AddAlarm(string groupId, string userId, DateTime starTime, int durationSeconds, string text)
        {
            await InitializeAsync();

            var a = new Alarm()
            {
                GroupId = groupId,
                UserId = userId,
                StartTime = starTime,
                EndTime = starTime.AddSeconds(durationSeconds),
                DurationSeconds = durationSeconds,
                IsActive = true,
                Text = text
            };

            await _alarms.InsertAsync(a);
            
            // Syncronize with cloud
            await SynchronizeServiceAsync();
            return a;
        }

        public async Task<Alarm> GetAlarm(string alarmId)
        {
            await InitializeAsync();

            var a = await _alarms.LookupAsync(alarmId);
            return a;
        }

        public async Task<IEnumerable<Alarm>> GetAllUserAlarmsFromDate(string userId, DateTime date)
        {
            await InitializeAsync();

            var alarms = await _alarms.ReadAsync(_alarms.CreateQuery().Where(a => a.UserId == userId));
            var alarmsF = from alarm in alarms where alarm.StartTime.Date >= date.Date select alarm;
            return alarmsF;
        }

        public async Task<Alarm> GetActiveUserAlarm(string userId)
        {
            await InitializeAsync();

            var alarm =
                (await _alarms.ReadAsync(_alarms.CreateQuery().Where(a => a.UserId == userId && a.IsActive)))
                .FirstOrDefault();
            return alarm;
        }

        public async Task<IEnumerable<Alarm>> GetActiveGroupAlarms(string groupId)
        {
            await InitializeAsync();

            var alarms = await _alarms.ReadAsync(_alarms.CreateQuery().Where(a => a.GroupId == groupId && a.IsActive));
            return alarms;
        }

        public async Task<Alarm> DisableAlarm(string alarmId)
        {
            await InitializeAsync();

            var a = await _alarms.LookupAsync(alarmId);
            a.IsActive = false;
            await _alarms.UpdateAsync(a);
            
            // Syncronize with cloud
            await SynchronizeServiceAsync();
            return a;
        }

        public async Task DisableAllGroupAlarms(string groupId)
        {
            await InitializeAsync();

            try
            {
                var alms = await _alarms.ReadAsync(_alarms.CreateQuery().Where(x => x.IsActive && x.GroupId == groupId));
                var enumerable = alms as IList<Alarm> ?? alms.ToList();
                foreach (var a in enumerable)
                {
                    await DisableAlarm(a.Id);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Source} : {e} ({e.InnerException})");
            }
        }

        public async Task<IEnumerable<UvReading>> GetUserUvReadingsOnDate(string userId, DateTime date)
        {
            await InitializeAsync();

            var uvReads =
                await _uvReadings.ReadAsync(
                    _uvReadings.CreateQuery().Where(r => r.UserId == userId && r.ReadTime.Day == date.Day && r.ReadTime.Month == date.Month && r.ReadTime.Year == date.Year));
            return uvReads;
        }

        public async Task<IEnumerable<UvReading>> GetUserUvReadingsFromDate(string userId, DateTime date)
        {
            await InitializeAsync();

            var uvReads =
                await _uvReadings.ReadAsync(_uvReadings.CreateQuery().Where(r => r.UserId == userId));
            var uvReadsF = from uvR in uvReads where uvR.ReadTime.Date >= date.Date select uvR;
            return uvReadsF;
        }

        public async Task<IEnumerable<UvReading>> GetUserLastUvReadingsByCount(string userId, int count)
        {
            await InitializeAsync();

            var uvReads = (await _uvReadings.ReadAsync(_uvReadings.CreateQuery().Where(r => r.UserId == userId))).ToList();
            var uvRlst = uvReads.OrderByDescending(r => r.ReadTime).ToList();
            List<UvReading> res = new List<UvReading>();
            foreach (UvReading read in uvRlst)
            {
                res.Add(read);
                count--;
                if (count == 0) break;
            }
            return res;
        }

        public async Task<UvReading> AddUserUvReading(string userId, DateTime readTime, string location, int localIndex, int light, int dailyExpo)
        {
            await InitializeAsync();

            try
            {
                var uvRead = new UvReading()
                {
                    UserId = userId,
                    LocalIndex = localIndex,
                    ReadTime = readTime,
                    PublicIndex = SunnyDay.Client.Core.Utils.DataConvert.Uv.UV_ToPublic(localIndex),
                    Description = SunnyDay.Client.Core.Utils.DataConvert.Uv.UV_ToDescription(localIndex),
                    ReadLocation = location,
                    LightIntensity = light,
                    DailyExposure = dailyExpo
                };
                await _uvReadings.InsertAsync(uvRead);
                
                // Syncronize with cloud
                await SynchronizeServiceAsync();

                Debug.WriteLine($@"> Added UV reading to database: 
    UserId = {userId}, 
    LocalIndex = {localIndex}, 
    ReadTime = {readTime}, 
    PublicIndex = {SunnyDay.Client.Core.Utils.DataConvert.Uv.UV_ToPublic(localIndex)}, 
    Description = {SunnyDay.Client.Core.Utils.DataConvert.Uv.UV_ToDescription(localIndex)}, 
    ReadLocation = {location}, 
    LightIntensity = {light}, 
    DailyExposure = {dailyExpo}");
                return uvRead;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Source} : {e} ({e.InnerException})");
                return null;
            }
        }

        public async Task<bool> LoginAsync()
        {
            if (Client == null)
                return false;

            Debug.WriteLine("> Azure service login execution");

            MobileServiceUser user = null;
            var auth = DependencyService.Get<IAuthentication>();
            try
            {
                switch (Settings.AuthProvider)
                {
                    case "facebook":
                        Debug.WriteLine("> Signing in with Facebook...");
                        user = await auth.LoginAsync(Client, MobileServiceAuthenticationProvider.Facebook);
                        break;
                    case "google":
                        Debug.WriteLine("> Signing in with Google...");
                        user = await auth.LoginAsync(Client, MobileServiceAuthenticationProvider.Google);
                        break;
                    default:
                        Debug.WriteLine("> Cannot sign in!");
                        break;
                }
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Source} : {e} ({e.InnerException})");
            }

            if (user == null)
            {
                Settings.AuthToken = string.Empty;
                Settings.UserId = string.Empty;
                Settings.AuthProvider = string.Empty;
                Settings.IsLoggedIn = false;
                return false;
            }
            else
            {
                Settings.AuthToken = user.MobileServiceAuthenticationToken;
                Settings.UserId = user.UserId;
                Settings.IsLoggedIn = true;

                Client.CurrentUser = new MobileServiceUser(user.UserId)
                {
                    MobileServiceAuthenticationToken = user.MobileServiceAuthenticationToken
                };

                return true;
            }
        }

        public async Task<Dictionary<string,string>>  GetVerifiedUserSocialInfo()
        {
            try
            {
                Dictionary<string, string> header = new Dictionary<string, string>();

                var connected =
                await CrossConnectivity.Current.IsRemoteReachable(Static.Keys.AzureServiceUrl);
                if (!connected)
                {
                    throw new CannotConnectToTheDestination();
                }

                // The authentication provider could also be Facebook, Twitter, or Microsoft
                var userInfo = JObject.Parse((await Client.InvokeApiAsync("SocialInfo", HttpMethod.Get, null)).ToString());

                Debug.WriteLine($"> Got user social info: {userInfo}");

                header.Add("id", userInfo["id"]?.ToString() ?? string.Empty);
                header.Add("name", userInfo["name"]?.ToString() ?? string.Empty);
                header.Add("picture", userInfo["picture"]?["data"]?["url"]?.ToString() ?? string.Empty);
                header.Add("email", userInfo["email"]?.ToString() ?? string.Empty);
                return header;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
        }

        public async Task<StorageTokenElement> GetStorageToken(string blobname)
        {
            await InitializeAsync();

            var connected =
                await CrossConnectivity.Current.IsRemoteReachable(Static.Keys.AzureServiceUrl);
            if (!connected)
            {
                throw new CannotConnectToTheDestination();
            }

            return await Client.InvokeApiAsync<StorageTokenElement>("GetStorageToken", HttpMethod.Get, new Dictionary<string, string>() {{ "name", blobname }});
        }

        public async Task SendReportForUser(string userEmail, int reportSize)
        {
            int reportLength = reportSize;
            SunnyDay.Client.Core.Models.UserReport.RootObject obj = new RootObject();

            var user = await GetUser(Settings.UserId);
            obj.SkinTone = user.SkinTone;
            obj.ImageUrl = user.ImageUrl;
            obj.SpfLevel = user.SpfLevel;
            obj.email = userEmail;
            obj.userName = user.Name;

            obj.UvReadings = new List<Models.UserReport.UvReading>();
            var uvReads = await GetUserUvReadingsFromDate(Settings.UserId, DateTime.UtcNow.AddDays(-reportLength));
            foreach (var read in uvReads)
            {
                obj.UvReadings.Add(new Models.UserReport.UvReading() {ReadLocation = read.ReadLocation, ReadTime = ConvertToUnixTimestamp(read.ReadTime), UVRead = read.PublicIndex});
            }

            Debug.WriteLine($"> obj.UvReadings OK!");

            obj.UvPerDay = new List<UvPerDay>();
            for (int i = 0; i < reportLength; i++)
            {
                var read =
                    (await GetUserUvReadingsOnDate(Settings.UserId, DateTime.UtcNow.AddDays(-i).Date)).OrderByDescending
                    (x => x.DailyExposure).FirstOrDefault();
                if (read == null)
                    continue;
                obj.UvPerDay.Add(new UvPerDay() {ReadTime = ConvertToUnixTimestamp(read.ReadTime), UVRead = read.DailyExposure});
            }
            Debug.WriteLine($"> obj.UvPerDay OK!");

            var alarms = await GetAllUserAlarmsFromDate(Settings.UserId, DateTime.UtcNow.AddDays(-reportLength));
            obj.Alarms = new List<int>();
            foreach (var a in alarms)
            {
                obj.Alarms.Add(ConvertToUnixTimestamp(a.StartTime));
            }
            Debug.WriteLine($"> obj.Alarms OK!");

            obj.childUsers = new List<ChildUser>();
            var children = await GetUserSubGroup(Settings.UserId);
            foreach (var child in children)
            {
                var als = await GetAllUserAlarmsFromDate(child.UserId, DateTime.UtcNow.AddDays(-reportLength));
                var chdals = new List<int>();
                foreach (var al in als)
                {
                    chdals.Add(ConvertToUnixTimestamp(al.StartTime));
                }

                obj.childUsers.Add(new ChildUser()
                {
                    SkinTone = child.SkinTone,
                    SpfLevel = child.SpfLevel,
                    ImageUrl = child.ImageUrl,
                    userName = child.Name,
                    Alarms = chdals
                });
                Debug.WriteLine($"> child {child.Name} OK!");
            }

            if (!(CrossConnectivity.Current.IsConnected))
            {
                throw new CannotConnectToTheDestination();
            }

            // Send the POST Request to the Authentication Server
            // Error Here
            string json = await Task.Run(() => JsonConvert.SerializeObject(obj));
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            using (var httpClient = new HttpClient())
            {
                Debug.WriteLine($"> Sending requesr for reports");
                // Do the actual request and await the response
                var httpResponse = await httpClient.PostAsync(Keys.AzureSpfUrl, httpContent);

                // If the response contains content we want to read it!
                if (httpResponse.Content != null)
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();
                    Debug.WriteLine($"> Request sent, response is: {responseContent}");
                }
            }
        }

        private static int ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return (int)Math.Floor(diff.TotalSeconds);
        }
    }
}
