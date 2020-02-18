using System;
using System.Net;

namespace SEO_Automation.Model
{
    /// <summary>
    /// Defines the model class RestException used for specific or generalized exceptions with HttpStatus code and error description.
    /// </summary>
    public class RestException : Exception
    {

        public RestException(HttpStatusCode code, object errors = null)
        {
            this.Errors = errors;
            this.Code = code;

        }
        public HttpStatusCode Code { get; set; }
        public object Errors { get; set; }
    }
}
