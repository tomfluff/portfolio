using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Files;
using SunnyDay.Client.Core.Models;

namespace SunnyDay.Client.Core.Services
{
    public interface ICloudService
    {
        /// <summary>
        /// The service client for communication (in mock is null)
        /// </summary>
        MobileServiceClient Client { get; }

        /// <summary>
        /// Innitialized the service. Making sure everything needed for the service to work is initiated.
        /// The service will use a static connection to the data storage mechanism,
        /// for preventing more than one connection instanse at a time.
        /// </summary>
        /// <returns></returns>
        Task InitializeAsync();

        /// <summary>
        /// Syncs all data from the local database to the service's online database. 
        /// Mostly used internally, might be removed later on from interface.
        /// In the mock service the function is implemented with no inner implementation (i.e. empty).
        /// </summary>
        /// <returns></returns>
        Task SynchronizeServiceAsync();

        /// <summary>
        /// Gets a user based on the user's ID.
        /// </summary>
        /// <param name="userId">The user ID to search by</param>
        /// <returns>Returns the requested user</returns>
        Task<User> GetUser(string userId);

        /// <summary>
        /// Gets the group of the sub-users that belongs to the current user.
        /// </summary>
        /// <param name="groupId">The group ID to search by (should be the main user's ID)</param>
        /// <returns>A collection of sub-users</returns>
        Task<IEnumerable<User>> GetUserSubGroup(string groupId);

        /// <summary>
        /// Creates and adds a new user to the database, based on the input variables.
        /// In the mock service a new GUID needs to be created.
        /// </summary>
        /// <param name="name">The name of the user</param>
        /// <param name="email">The user's email address</param>
        /// <param name="skinTone">The skin tone index number of the user (0-6)</param>
        /// <param name="spfLevel">the SPF index level of the user (0,15,30,40,50)</param>
        /// <param name="isVerified">Boolean representing wether the user is verified or not</param>
        /// <param name="imageUrl">A URL to the user's profile picture</param>
        /// <returns>Returns the newly created user</returns>
        Task<User> AddUser(string name, string email, int skinTone, int spfLevel, bool isVerified, string imageUrl);

        /// <summary>
        /// Updates a user based on the updated input user.
        /// </summary>
        /// <param name="user">The updated user object. The ID field mustn't be changed.</param>
        /// <returns>Returns the newly updated user</returns>
        Task<User> UpdateUser(User user);

        /// <summary>
        /// Deletes the user from the database.
        /// </summary>
        /// <param name="user">The user to delete</param>
        /// <returns></returns>
        Task DeleteUser(User user);

        /// <summary>
        /// Add a new ACTIVE alarm for the user.
        /// </summary>
        /// <param name="groupId">The user's group ID</param>
        /// <param name="userId">The user's ID</param>
        /// <param name="starTime">The starting time of the alarm</param>
        /// <param name="durationSeconds">The duration in seconds of the alarm</param>
        /// <param name="text">A piece of text to attached to the alarm</param>
        /// <returns></returns>
        Task<Alarm> AddAlarm(string groupId, string userId, DateTime starTime, int durationSeconds, string text);

        /// <summary>
        /// Get's an alarm based on the alarm ID.
        /// </summary>
        /// <param name="alarmId">The alarm ID to search by</param>
        /// <returns>Returns the requested alarm</returns>
        Task<Alarm> GetAlarm(string alarmId);

        /// <summary>
        /// Gets the active user alarm by the user's ID.
        /// </summary>
        /// <param name="userId">The user ID to search by</param>
        /// <returns>Returns the active user alarm</returns>
        Task<Alarm> GetActiveUserAlarm(string userId);

        /// <summary>
        /// Gets a collection of the active alrms in the group by the group ID.
        /// </summary>
        /// <param name="groupId">The group ID to search by</param>
        /// <returns>A collection of alarms</returns>
        Task<IEnumerable<Alarm>> GetActiveGroupAlarms(string groupId);

        /// <summary>
        /// Disables the alarm based on the alarm ID parameter.
        /// </summary>
        /// <param name="alarmId">The alarm ID to search by</param>
        /// <returns>Returns the updated alarm</returns>
        Task<Alarm> DisableAlarm(string alarmId);

        /// <summary>
        /// Disables all alarms for the group based on group ID parameter.
        /// </summary>
        /// <param name="groupId">The group ID to search by</param>
        /// <returns></returns>
        Task DisableAllGroupAlarms(string groupId);

        /// <summary>
        /// Get's an IEnumerable collection of UV readings from a certain day/date.
        /// </summary>
        /// <param name="userId">The user ID to search by</param>
        /// <param name="date">The day/date to filter by</param>
        /// <returns>Returns a collection of UV readings</returns>
        Task<IEnumerable<UvReading>> GetUserUvReadingsFromDate(string userId, DateTime date);

        /// <summary>
        /// Get's the last [count] UV reading for the user.
        /// </summary>
        /// <param name="userId">The user ID to search by</param>
        /// <param name="count">The amount of readings to get</param>
        /// <returns>Returns a collection of UV readings</returns>
        Task<IEnumerable<UvReading>> GetUserLastUvReadingsByCount(string userId, int count);

        /// <summary>
        /// Creates and adds a new UV reading to the database. Updating any offline/online sources.
        /// Based on the [localIndex] parameter, get's the [PublicIndex] and [Description].
        /// </summary>
        /// <param name="userId">The user ID to search by</param>
        /// <param name="readTime">The DateTime (string) of the reading time</param>
        /// <param name="location">The string representation of the read location</param>
        /// <param name="localIndex">The local index of the reading (0-4)</param>
        /// <param name="light">The light intensity of the reading</param>
        /// <param name="dailyExpo">The daily exposure value</param>
        /// <returns>Returns the newly created UV reading</returns>
        Task<UvReading> AddUserUvReading(string userId, DateTime readTime, string location, int localIndex, int light, int dailyExpo);

        /// <summary>
        /// Login with authentication handeling (in mock only returns true)
        /// </summary>
        /// <returns></returns>
        Task<bool> LoginAsync();

        /// <summary>
        /// Get the ligged-in user's social information, social profile id, 
        /// the user's name and a url to the user's profile picture.
        /// </summary>
        /// <returns>A dictionary contains "id", "name" and "picture"</returns>
        Task<Dictionary<string, string>> GetVerifiedUserSocialInfo();

        /// <summary>
        /// Get a storage token for the blob storage container in the cloude, with the mntioned blobname.
        /// </summary>
        /// <param name="blobname">The name of the new blob</param>
        /// <returns>A storage toekn element</returns>
        Task<StorageTokenElement> GetStorageToken(string blobname);

        /// <summary>
        /// Send a report for the user via the requested email address.
        /// </summary>
        /// <param name="userEmail">The email address to send the report to</param>
        /// <param name="reportSize">The number fo past days to include in the report</param>
        /// <returns></returns>
        Task SendReportForUser(string userEmail, int reportSize);
    }
}
