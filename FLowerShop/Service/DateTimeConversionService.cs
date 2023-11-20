using System;

namespace FlowerShop.Service
{
    public static class DateTimeConversionService
    {
        public static string ConvertToVietnamTimeZone(DateTime? utcDateTime)
        {
            if (utcDateTime.HasValue)
            {
                string timeZoneId = "SE Asia Standard Time";

                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

                DateTime vietnamDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime.Value, vietnamTimeZone);

                return vietnamDateTime.ToString("dd/MM/yyyy HH:mm");
            }
            return string.Empty;
        }
    }
}
