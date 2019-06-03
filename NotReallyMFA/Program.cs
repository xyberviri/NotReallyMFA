using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NotReallyMFA
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    Console.WriteLine("{0}", GetCode(arg, GetInterval(DateTime.UtcNow)));
                }
            } else
            {
                Console.WriteLine("{0}", GenerateSecretKey());
            }
        }

        //https://blogs.technet.microsoft.com/cloudpfe/2014/10/26/using-time-based-one-time-passwords-for-multi-factor-authentication-in-ad-fs-3-0/
        #region TOTP
        #region TOTP Settings
        static int intervalDrift = 3;
        static int validityPeriodSeconds = 30;                                              //Don't change this
        static int secretKeyLength = 16;                                                    //Or this
        static DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);    //Or this
        static string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";                  //Or this
        #endregion
        public static string GenerateSecretKey()
        {
            Random random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
            return new string((new char[secretKeyLength]).Select(c => c = Base32Alphabet[random.Next(0, Base32Alphabet.Length)]).ToArray());
        }

        private static long GetInterval(DateTime dateTime, long offset = 0)
        {
            TimeSpan elapsedTime = dateTime.ToUniversalTime() - unixEpoch;
            return (long)elapsedTime.TotalSeconds / validityPeriodSeconds;
        }
        private static string GetCode(string secretKey, long timeIndex)
        {
            var secretKeyBytes = Base32Encode(secretKey);
            HMACSHA1 hmac = new HMACSHA1(secretKeyBytes);
            byte[] challenge = BitConverter.GetBytes(timeIndex);
            if (BitConverter.IsLittleEndian) Array.Reverse(challenge);
            byte[] hash = hmac.ComputeHash(challenge);
            int offset = hash[19] & 0xf;
            int truncatedHash = hash[offset] & 0x7f;
            for (int i = 1; i < 4; i++)
            {
                truncatedHash <<= 8;
                truncatedHash |= hash[offset + i] & 0xff;
            }
            truncatedHash %= 1000000;
            return truncatedHash.ToString("D6");
        }

        private static bool CheckCode(string secretKey, string code, string upn, DateTime when)
        {
            long currentInterval = GetInterval(when);
            bool success = false;
            for (long timeIndex = currentInterval - intervalDrift; timeIndex <= currentInterval + intervalDrift; timeIndex++)
            {
                string intervalCode = GetCode(secretKey, timeIndex);
                //bool intervalCodeHasBeenUsed = CodeIsUsed(upn, timeIndex);
                if (/*!intervalCodeHasBeenUsed &&*/ ConstantTimeEquals(intervalCode, code))
                {
                    success = true;
                    //SetUsedCode(upn, timeIndex);
                    break;
                }
            }
            return success;
        }
        protected static bool ConstantTimeEquals(string a, string b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)a[i] ^ (uint)b[i];
            }
            return diff == 0;
        }


        private static byte[] Base32Encode(string source)
        {
            var bits = source.ToUpper().ToCharArray().Select(c =>
                Convert.ToString(Base32Alphabet.IndexOf(c), 2).PadLeft(5, '0')).Aggregate((a, b) => a + b);
            return Enumerable.Range(0, bits.Length / 8).Select(i => Convert.ToByte(bits.Substring(i * 8, 8), 2)).ToArray();
        }

        //http://scottless.com/blog/archive/2014/02/15/base32-encoder-and-decoder-in-c.aspx
        internal static string ToBase32String(byte[] bytes, int InByteSize = 8, int OutByteSize = 5)
        {
            // Prepare container for the final value
            StringBuilder builder = new StringBuilder(bytes.Length * InByteSize / OutByteSize);

            // Position in the input buffer
            int bytesPosition = 0;

            // Offset inside a single byte that <bytesPosition> points to (from left to right)
            // 0 - highest bit, 7 - lowest bit
            int bytesSubPosition = 0;

            // Byte to look up in the dictionary
            byte outputBase32Byte = 0;

            // The number of bits filled in the current output byte
            int outputBase32BytePosition = 0;

            // Iterate through input buffer until we reach past the end of it
            while (bytesPosition < bytes.Length)
            {
                // Calculate the number of bits we can extract out of current input byte to fill missing bits in the output byte
                int bitsAvailableInByte = Math.Min(InByteSize - bytesSubPosition, OutByteSize - outputBase32BytePosition);

                // Make space in the output byte
                outputBase32Byte <<= bitsAvailableInByte;

                // Extract the part of the input byte and move it to the output byte
                outputBase32Byte |= (byte)(bytes[bytesPosition] >> (InByteSize - (bytesSubPosition + bitsAvailableInByte)));

                // Update current sub-byte position
                bytesSubPosition += bitsAvailableInByte;

                // Check overflow
                if (bytesSubPosition >= InByteSize)
                {
                    // Move to the next byte
                    bytesPosition++;
                    bytesSubPosition = 0;
                }

                // Update current base32 byte completion
                outputBase32BytePosition += bitsAvailableInByte;

                // Check overflow or end of input array
                if (outputBase32BytePosition >= OutByteSize)
                {
                    // Drop the overflow bits
                    outputBase32Byte &= 0x1F;  // 0x1F = 00011111 in binary

                    // Add current Base32 byte and convert it to character
                    builder.Append(Base32Alphabet[outputBase32Byte]);

                    // Move to the next byte
                    outputBase32BytePosition = 0;
                }
            }

            // Check if we have a remainder
            if (outputBase32BytePosition > 0)
            {
                // Move to the right bits
                outputBase32Byte <<= (OutByteSize - outputBase32BytePosition);

                // Drop the overflow bits
                outputBase32Byte &= 0x1F;  // 0x1F = 00011111 in binary

                // Add current Base32 byte and convert it to character
                builder.Append(Base32Alphabet[outputBase32Byte]);
            }

            return builder.ToString();
        }
        #endregion


    }
}
