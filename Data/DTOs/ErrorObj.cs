using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class ErrorObj : Exception
    {
        public ErrorObj()
        {
        }

        public ErrorObj(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public ErrorObj(int code, string description, string message)
        {
            Code = code;
            Message = message;
            Description = description;
        }

        public int Code { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}
