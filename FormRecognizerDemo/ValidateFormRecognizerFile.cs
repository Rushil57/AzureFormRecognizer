using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FormRecognizerDemo
{
    public class ValidateFormRecognizerFile
    {
        /// <summary>
        /// The ValidateFile method is an asynchronous task that takes a Stream object representing a file and a ModelID string as input. It performs document analysis using the Azure Cognitive Services Document Analysis API. The method retrieves the required configuration values, including the Azure Blob storage endpoint and API key, and initializes the DocumentAnalysisClient with these values.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="ModelID"></param>
        /// <returns></returns>
        #region Validate File with Form recognizer
        public async Task<string> ValidateFile(Stream file, string ModelID)
        {
            var Configuration = GetConfig();
            string endpoint = Configuration["AzureWebBlobEndpoint"].ToString();
            string apiKey = Configuration["ApiKey"];
            var credential = new AzureKeyCredential(apiKey);
            string JsonValuePair = "";
            if (file != null)
            {
                DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);
                AnalyzeDocumentOperation operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, ModelID, file);
                if (operation != null)
                {
                    AnalyzeResult result = operation.Value;
                    if (result != null)
                    {
                        foreach (AnalyzedDocument document in result.Documents)
                        {
                            if (document.Fields.Any())
                            {
                                var remainingFields = document.Fields.Where(x => x.Value != null).ToList();
                                if (remainingFields.Any())
                                    JsonValuePair = JsonConvert.SerializeObject(remainingFields);
                            }
                        }
                    }

                }
            }

            return JsonValuePair;
        }
        #endregion

        #region Config Values
        public IConfiguration GetConfig()
        {
            string fileName = "local.settings.json";
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(fileName, optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            return builder.Build();
        }
        #endregion

    }
}
