using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace ServicePleaseService.Helpers
{
    public static class Cryptography
    {
        public static string GetChecksum(string secret)
        {
            Encoding ae = new System.Text.UTF8Encoding();
            HMACSHA1 signature = new HMACSHA1(ae.GetBytes(secret));
            string rawSignature = secret;
            string encodedSignature = Convert.ToBase64String(signature.ComputeHash(ae.GetBytes(rawSignature.ToCharArray())));
            return encodedSignature;
        }
    }
}
