using System.Net;

namespace TurnApp.Utils
{
    public class ErrorResponse
    {
        public ResponseMessage Message { get; }
        public HttpStatusCode StatusCode { get; set; }

        public ErrorResponse(HttpStatusCode code, string msg) : base(msg)
        {
            Message = new ResponseMessage(msg);
            StatusCode = code;
        }
    }
}
