using Infrastructure.DTOs;
using Infrastructure.Helper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Utils
{
    public static class Common
    {
        /*  public static Boolean Operator(this string logic, decimal a, decimal b)
          {
              return logic switch
              {
                  AppConstant.Operator.GREATER_THAN => a > b,
                  AppConstant.Operator.GREATER_THAN_OR_EQUAL => a >= b,
                  AppConstant.Operator.LESS_THAN => a < b,
                  AppConstant.Operator.LESS_THAN_OR_EQUAL => a <= b,
                  AppConstant.Operator.EQUAL => a == b,
                  _ => throw new ErrorObj(code: (int)HttpStatusCode.BadRequest, message: AppConstant.ErrMessage.Invalid_Operator),
              };

          }*/
        public static bool Compare<T>(string op, T orderAmount, T minAmount) where T : IComparable<T>
        {
            return op.Trim() switch
            {
                AppConstant.Operator.GREATER_THAN => orderAmount.CompareTo(minAmount) > 0,
                AppConstant.Operator.GREATER_THAN_OR_EQUAL => orderAmount.CompareTo(minAmount) >= 0,
                AppConstant.Operator.LESS_THAN => orderAmount.CompareTo(minAmount) < 0,
                AppConstant.Operator.LESS_THAN_OR_EQUAL => orderAmount.CompareTo(minAmount) <= 0,
                AppConstant.Operator.EQUAL => orderAmount.CompareTo(minAmount) == 0,
                _ => throw new ErrorObj(code: (int)AppConstant.ErrCode.Invalid_Operator, message: AppConstant.ErrMessage.Invalid_Operator),
            };
        }
        public static DateTime GetCurrentDatetime()
        {
            return DateTime.UtcNow.AddHours(7);
        }

        public static bool CompareBinary(int input, int compareString)
        {
            return (input & compareString) == input;
        }

        public static string DecodeFromBase64(string encodedData)
        {
            try
            {
                UTF8Encoding encoder = new UTF8Encoding();
                Decoder utf8Decode = encoder.GetDecoder();
                byte[] todecode_byte = Convert.FromBase64String(encodedData);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string EncodeToBase64(string plainText)
        {
            try
            {
                byte[] encData_byte = new byte[plainText.Length];
                encData_byte = Encoding.UTF8.GetBytes(plainText);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GenerateAPIKey()
        {
            var key = new byte[16];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);
            string apiKey = Convert.ToBase64String(key);
            return apiKey;
        }
        public static string CreateApiKey()
        {
            var bytes = new byte[256 / 8];
            using (var random = RandomNumberGenerator.Create())
                random.GetBytes(bytes);
            return ToBase62String(bytes);
        }
        private static string ToBase62String(byte[] toConvert)
        {
            const string alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            BigInteger dividend = new BigInteger(toConvert);
            var builder = new StringBuilder();
            while (dividend != 0)
            {
                dividend = BigInteger.DivRem(dividend, alphabet.Length, out BigInteger remainder);
                builder.Insert(0, alphabet[Math.Abs(((int)remainder))]);
            }
            return builder.ToString();
        }

    }
}
