using Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class ErrorResponse : Exception
    {
        public ErrorResponse()
        {
        }
        public ErrorResponse(ErrorObj internalServerError, ErrorObj notFound, ErrorObj badRequest)
        {
            InternalServerError = internalServerError;
            NotFound = notFound;
            BadRequest = badRequest;
        }

        public ErrorObj InternalServerError { get; set; } = new ErrorObj(code: (int)HttpStatusCode.InternalServerError, message: AppConstant.ErrMessage.Opps, description: AppConstant.ErrMessage.Internal_Server_Error);
        public ErrorObj NotFound { get; set; } = new ErrorObj(code: (int)HttpStatusCode.NotFound, message: AppConstant.ErrMessage.Not_Found_Resource, description: AppConstant.ErrMessage.Not_Found_Resource);
        public ErrorObj BadRequest { get; set; } = new ErrorObj(code: (int)HttpStatusCode.BadRequest, message: AppConstant.ErrMessage.Bad_Request, description: AppConstant.ErrMessage.Bad_Request);

        public ErrorObj Unauthorized { get; set; } = new ErrorObj(code: (int)HttpStatusCode.Unauthorized, message: AppConstant.ErrMessage.Unauthorized, description: AppConstant.ErrMessage.Unauthorized);

        public ErrorObj Error;

    }

}
