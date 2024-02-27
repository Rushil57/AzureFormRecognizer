using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace FormRecognizerDemo
{
    public static class FormRecognizerValidation
    {   /// <summary>
        /// The provided code represents an Azure Function named FormRecognizerValidation. It is triggered by an HTTP POST request with the route "/validatefile". The function reads form data from the request, checks if a file is present, extracts the ModelID from the form data, and opens a stream to the file. 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="ModelID"></param>
        /// <returns></returns>
        [FunctionName("FormRecognizerValidation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "validatefile")] HttpRequest req,
            ILogger log)
        {
            var formdata = await req.ReadFormAsync();
            if (formdata != null && formdata.Files != null && formdata.Files.Count > 0)
            {
                string ModelID = formdata["ModelID"].ToString();
                Stream requestedfile = formdata.Files[0].OpenReadStream();
                string JsonValue = await new ValidateFormRecognizerFile().ValidateFile(requestedfile, ModelID);
                return new OkObjectResult(JsonValue);
            }
            else
                return new OkObjectResult("File not found.");
        }
    }
}
