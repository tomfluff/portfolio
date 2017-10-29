using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Connectivity;
using SunnyDay.Client.Core.Models;
using SunnyDay.Client.Core.Static;
using SunnyDay.Client.Core.Utils;

namespace SunnyDay.Client.Core.Services
{
    public class SkinToneProviderService : ISkinToneProviderService
    {
        public async Task<int> GetSkinToneFromImageUri(string fileUri, string fileName, int lightIntensity=0)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                throw new CannotConnectToTheDestination();
            }
            var spfServiceUri = $"{Keys.AzureSpfUrl}?light={lightIntensity}&fileName={fileName}&imgUrl={fileUri}";

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(spfServiceUri);

            string responseAsString = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"{responseAsString}");

            SkinToneResponse jsonRes = JsonConvert.DeserializeObject<SkinToneResponse>(responseAsString);
            if (jsonRes.Success)
            {
                return jsonRes.SkinType != string.Empty ? int.Parse(jsonRes.SkinType) : -1;
            }
            return -1;
        }
    }
}


