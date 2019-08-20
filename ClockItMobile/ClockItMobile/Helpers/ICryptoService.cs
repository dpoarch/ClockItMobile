using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClockIt.Mobile.Helpers
{
    public interface ICryptoService // or whatever
    {
        string Cipher(string stringToCipher, string verb, string resourceType, string resourceId, string date);
        string Decipher(string stringToDecipher);
    }
}
