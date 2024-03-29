﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClockIt.Mobile.Helpers;
using CommandScheduling.Mobile.iOS;
using Foundation;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(CryptoService))]
namespace CommandScheduling.Mobile.iOS
{
    class CryptoService : ICryptoService
    {
        public string Cipher(string stringToCipher, string verb, string resourceType, string resourceId, string date)
        {
            var hmacSha256 = new System.Security.Cryptography.HMACSHA256 { Key = Convert.FromBase64String(stringToCipher) };
            //var date = DateTime.UtcNow.ToString("r");
            verb = verb ?? "";
            resourceType = resourceType ?? "";
            resourceId = resourceId ?? "";

            string payLoad = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}\n{1}\n{2}\n{3}\n{4}\n",
                    verb.ToLowerInvariant(),
                    resourceType.ToLowerInvariant(),
                    resourceId,
                    date.ToLowerInvariant(),
                    ""
            );

            byte[] hashPayLoad = hmacSha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payLoad));
            string signature = Convert.ToBase64String(hashPayLoad);

            return System.Net.WebUtility.UrlEncode(String.Format(System.Globalization.CultureInfo.InvariantCulture, "type={0}&ver={1}&sig={2}",
                "master",
                "1.0",
                signature));
        }

        public string Decipher(string stringToDecipher)
        {
            return "";
        }
    }
}