using Inwinteck_CRM.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Inwinteck_CRM.Universal
{
    public class CommonMethod
    {
        public static async Task<TimeZoneInfo> GetTimeZoneAsync(string lat, string lng)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiKey = "AIzaSyBvhQbrULBXqk9wPb9RjTeacVk3A3g-ci8"; 
                string url = $"https://maps.googleapis.com/maps/api/timezone/json?location={lat},{lng}&timestamp={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}&key={apiKey}";

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var timeZoneInfo = JsonConvert.DeserializeObject<TimeZoneApiResponse>(responseBody);

                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(timeZoneInfo.timeZoneId);
                }
                catch (TimeZoneNotFoundException)
                {
                    var baseUtcOffset = TimeSpan.FromSeconds(timeZoneInfo.rawOffset + timeZoneInfo.dstOffset);
                    return TimeZoneInfo.CreateCustomTimeZone(timeZoneInfo.timeZoneId, baseUtcOffset, timeZoneInfo.timeZoneName, timeZoneInfo.timeZoneName);
                }
            }
        }

        public static async Task<(DateTime istToUs, DateTime istToLocal)> ConvertIstToLocalAndUtcAsync(string lat, string lng, DateTime istDate)
        {
            istDate = DateTime.SpecifyKind(istDate, DateTimeKind.Unspecified); // Ensure the kind is set
            TimeZoneInfo indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            TimeZoneInfo easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            TimeZoneInfo localTimeZone = await GetTimeZoneAsync(lat, lng);

            DateTime istToUtc = TimeZoneInfo.ConvertTimeToUtc(istDate, indiaTimeZone);  // IST to UTC
            DateTime istToLocal = TimeZoneInfo.ConvertTimeFromUtc(istToUtc, localTimeZone); // UTC to local
            DateTime istToUs = TimeZoneInfo.ConvertTimeFromUtc(istToUtc, easternTimeZone); // UTC to EST

            return (istToUs, istToLocal);
        }

        public static async Task<(DateTime localToUs, DateTime localToIst)> ConvertLocalToIstAndUtcAsync(string lat, string lng, DateTime localDate)
        {
            localDate = DateTime.SpecifyKind(localDate, DateTimeKind.Unspecified); // Ensure the kind is set
            TimeZoneInfo indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            TimeZoneInfo easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            TimeZoneInfo localTimeZone = await GetTimeZoneAsync(lat, lng);

            DateTime localToUtc = TimeZoneInfo.ConvertTimeToUtc(localDate, localTimeZone); // Local to UTC
            DateTime localToIst = TimeZoneInfo.ConvertTimeFromUtc(localToUtc, indiaTimeZone); // UTC to IST
            DateTime localToUs = TimeZoneInfo.ConvertTimeFromUtc(localToUtc, easternTimeZone); // UTC to EST

            return (localToUs, localToIst);
        }

        //public static async Task<(DateTime istToUs, DateTime istToLocal)> ConvertIstToLocalAndUtcAsync(string lat, string lng, DateTime istDate)
        //{
        //    TimeZoneInfo indiaTimeZone;
        //    TimeZoneInfo esternTimeZone;
        //    try
        //    {
        //        indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //    }
        //    catch (TimeZoneNotFoundException)
        //    {
        //        indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");
        //    }
        //    try
        //    {
        //        esternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        //    }
        //    catch (TimeZoneNotFoundException)
        //    {
        //        // Handle the case where the timezone could not be found
        //        throw new Exception("Eastern Standard Time zone not found.");
        //    }

        //    TimeZoneInfo localTimeZone = await GetTimeZoneAsync(lat, lng);

        //    DateTime istToUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(istDate, DateTimeKind.Local), indiaTimeZone);
        //    //DateTime istToUtc = TimeZoneInfo.ConvertTimeToUtc(istDate, indiaTimeZone);  //ist to utc
        //    DateTime istToLocal = TimeZoneInfo.ConvertTimeFromUtc(istToUtc, localTimeZone); // Utc to local
        //    DateTime istToUs = TimeZoneInfo.ConvertTimeFromUtc(istToUtc, esternTimeZone); // Utc to Est

        //    return (istToUs, istToLocal);
        //}

        //public static async Task<(DateTime localToUs, DateTime localToIst)> ConvertLocalToIstAndUtcAsync(string lat, string lng, DateTime localDate)
        //{
        //    TimeZoneInfo indiaTimeZone;
        //    TimeZoneInfo esternTimeZone;

        //    try
        //    {
        //        indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        //    }
        //    catch (TimeZoneNotFoundException)
        //    {
        //        indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");
        //    }
        //    try
        //    {
        //        esternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        //    }
        //    catch (TimeZoneNotFoundException)
        //    {
        //        // Handle the case where the timezone could not be found
        //        throw new Exception("Eastern Standard Time zone not found.");
        //    }

        //    TimeZoneInfo localTimeZone = await GetTimeZoneAsync(lat, lng);

        //    DateTime localToUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(localDate, DateTimeKind.Local), localTimeZone);
        //    //DateTime localToUtc = TimeZoneInfo.ConvertTimeToUtc(localDate, localTimeZone);
        //    DateTime localToIst = TimeZoneInfo.ConvertTimeFromUtc(localToUtc, indiaTimeZone);
        //    DateTime localToUs = TimeZoneInfo.ConvertTimeFromUtc(localToUtc, esternTimeZone);

        //    return (localToUs, localToIst);
        //}


        public static int CalculateSLA(DateTime? dispatchDate, DateTime? ticketDate)
        {
            if (dispatchDate.HasValue && ticketDate.HasValue)
            {
                TimeSpan difference = dispatchDate.Value - ticketDate.Value;

                if (difference.TotalHours <= 2)
                {
                    return 1357; //2hrs
                }
                else if (difference.TotalHours > 2 && difference.TotalHours <= 4)
                {
                    return 1358;//4hrs
                }
                else if (difference.TotalHours > 4 && difference.TotalHours <= 8)
                {
                    return 1359;//8hrs
                }
                else if (difference.TotalHours > 8 && dispatchDate.Value.Date == ticketDate.Value.Date)
                {
                    return 1598; //Same Day 1598 for live
                }
                else if (dispatchDate.Value.Date == ticketDate.Value.AddDays(1).Date)
                {
                    return 1360; //NBD
                }
                else
                {
                    return 1361; //Scheduled
                }
            }
            else
            {
                // Handle the case where either Dispatch_Date or Ticket_Date is null
                return 1361; // Assuming default SLA if dates are invalid
            }
        }


    }
}