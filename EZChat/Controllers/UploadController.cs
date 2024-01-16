using Contracts;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Models;


namespace EZChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UploadController : ControllerBase
    {
        private readonly IDataExtractionCompletion _chatCompletion;
        private readonly IDataExtraction _dataExtraction;
        List<ChatMessage> chatMessages = [];

        public UploadController(IDataExtractionCompletion chatCompletion,
                                IDataExtraction dataExtraction)
        {
            _chatCompletion = chatCompletion;
            _dataExtraction = dataExtraction;
        }

        //***************************************************************************************
        //** Explanation of the code below
        //***************************************************************************************
        // The client-side code is sending an HTTP POST request to the "api/documents" endpoint
        // with a MultipartFormDataContent object, which contains the file data.
        // On the server side, the OnPostDocumentAsync method is set up to receive this request.
        //
        // The[FromForm] IFormFileCollection files parameter is used to bind the uploaded files
        // from the request.The[FromForm] attribute tells ASP.NET Core to bind the data from the
        // form body of the HTTP request to this parameter.
        //
        // When the client sends the request, the files are included in the body of the request
        // as multipart/form-data.When the server receives the request, it reads the
        // multipart/form-data and binds each part to an IFormFile object. These IFormFile objects
        // are then collected into an IFormFileCollection, which is what the files parameter represents.
        //
        // So, even though the client is sending a MultipartFormDataContent object and the server is
        // receiving an IFormFileCollection, they are compatible because they are both ways of
        // handling multipart/form-data content. The MultipartFormDataContent is used for sending
        // the data, and the IFormFileCollection is used for receiving the data.
        //***************************************************************************************

        //POST: POCController/Create
        [HttpPost]
        public async Task<ActionResult> PostPdf([FromQuery] string prompt, [FromForm] IFormFileCollection files)
        {
            try
            {
                // Call to extractor service
                var response = await _dataExtraction.ExtractDataFromPDF(files);

                // Add the new message to chatMessages
                chatMessages.Add(new ChatMessage(Role.User, prompt + " " + response.UploadedFiles[0], 0));

                var completionOptions = new EZCompletionOptions()
                {
                    Temperature = (float)0.2,
                    MaxTokens = 4000,
                    //NucleusSamplingFactor = (float)0.95,
                    FrequencyPenalty = 0,
                    PresencePenalty = 0,
                    //UserPrompt = response.UploadedFiles[0],
                    ChatMessages = chatMessages
                };

                // Call to chat service
                var completion = await _chatCompletion.ChatCompletionAsync(completionOptions);

                return Ok(completion);

                //(ChatCompletions response, ChatCompletions followup, int promptTokens, int responseTokens, int suggestionTokens) = await _chatCompletion.ChatCompletionAsync(completionOptions);
                //var completion = await _chatCompletion.ChatCompletionAsync(completionOptions);

                return Ok(response);
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw;
            }


            //return null;

            //return Ok(new Completion
            //{
            //    Response = response.Choices.FirstOrDefault()?.Message?.Content,
            //    Suggestions = followup.Choices.FirstOrDefault()?.Message?.Content,
            //    PromptTokens = promptTokens,
            //    ResponseTokens = responseTokens,
            //    SuggestionTokens = suggestionTokens
            //});


            //var request = System.Text.Json.JsonSerializer.Deserialize(customer);
            //await ChatCompletion.ChatCompletionAsync(ezCompletionOptions);

            //var json = System.Text.Json.JsonSerializer.Serialize("Post");
            //return Content(json, "application/json");

        }
    }
}






// The code you've provided is a method for uploading files to a server using the HttpClient class in .NET. Here's a step-by-step explanation:
//   1.A MultipartFormDataContent object is created. This object will hold the file data for the HTTP request.
//   2.	The method iterates over each file in the files list. Each file is an IBrowserFile, which represents a file selected from a <InputFile> component in a Blazor WebAssembly app.
//   3.	For each file, it opens a read stream with a maximum size limit. This stream is wrapped in a StreamContent object, which represents the HTTP content as a stream.
//   4.	The ContentType header of the StreamContent is set to the MIME type of the file.
//   5.	The StreamContent is added to the MultipartFormDataContent object. The name and filename of the content are both set to the name of the file.
//   6.	Two headers, X-CSRF-TOKEN-FORM and X-CSRF-TOKEN-HEADER, are added to the MultipartFormDataContent. These headers are used for Cross-Site Request Forgery (CSRF) protection.
//   7.	An HTTP POST request is sent to the "api/documents" endpoint with the MultipartFormDataContent object. The response from the server is awaited and stored in the response variable.
// This method allows you to upload multiple files to the server in a single HTTP request. The files are sent as multipart/form-data, which is a common format for file upload requests. The server would then process each part of the request as a separate file.


// The OnPostDocumentAsync method you've highlighted is an ASP.NET Core action method that handles HTTP POST requests to the "documents" endpoint. This method is designed to receive file uploads and store them using the Azure Blob Storage service. Here's a breakdown of the parameters:
//   [FromForm] IFormFileCollection files: This parameter represents the files that are uploaded in the HTTP request. The [FromForm] attribute indicates that the files are sent in the body of the HTTP request using the "multipart/form-data" content type. IFormFileCollection is a collection of IFormFile objects, where each IFormFile represents an individual file that was uploaded.
// 	 [FromServices] AzureBlobStorageService service: This parameter is an instance of AzureBlobStorageService, which is a service class that encapsulates interactions with Azure Blob Storage. The [FromServices] attribute is used to request this service from the dependency injection container.
//	 [FromServices] ILogger<AzureBlobStorageService> logger: This parameter is a logger that can be used to log messages.It's specifically a logger for the AzureBlobStorageService class. Like the service parameter, it's requested from the dependency injection container using the [FromServices] attribute.
//   CancellationToken cancellationToken: This parameter is a CancellationToken that can be used to cancel the operation if necessary.
// Inside the method, it calls service.UploadFilesAsync(files, cancellationToken) to upload the files to Azure Blob Storage. The result of this operation is then returned as the response of the HTTP request.




