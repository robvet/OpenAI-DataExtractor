using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using EZChat.Client.Contracts;
using EZChat.Client.Models;
using Microsoft.AspNetCore.Components.Forms;
using Shared.Configuration;
using Shared.Models;

namespace EZChat.Client.Services
{
    public class UploadService(HttpClient httpClient) : IUploadService
    {
        public async Task<Completion> UploadDocumentsAsync(WebAssemblyTicket ticket,
                                                                        string prompt)
        {
            
            try
            {
                long maxFileSize = 5120000;
                //long MaxIndividualFileSize = 1_024L * 1_024;
                
                //// Create a new MultipartFormDataContent object to hold the file data for the HTTP request
                using MultipartFormDataContent content = new();

                //// Add form values
                //content.Add(new StringContent(ticket.Title), "Title");
                //content.Add(new StringContent(ticket.Description), "Description");


                // Iterate over each file in the list of files
                foreach (var file in ticket.Attachments)
                {
                    string safeFileName = WebUtility.HtmlEncode(file.Name);

                    // max allow size: 10mb
                    // Calculate the maximum size in bytes (maxAllowedSize is in MB, so we multiply by 1024 twice to get the size in bytes)
                    //var max_size = maxAllowedSize * 1024 * 1024;
                    // Create a new StreamContent object from the file's content, limiting the maximum size of the stream
                    // The CA2000 warning is disabled because the StreamContent will be disposed when the MultipartFormDataContent is disposed
                    var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));

                    // Set the content type of the fileContent to the file's content type
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                    // Add the fileContent to the MultipartFormDataContent object, using the file's name as the name and filename
                    //content.Add(fileContent, safeFileName, safeFileName);

                    content.Add(fileContent, "files", safeFileName);
                }

                //// set cookie
                //// Add the X-CSRF-TOKEN-FORM and X-CSRF-TOKEN-HEADER headers to the MultipartFormDataContent object
                //// These headers are used for Cross-Site Request Forgery (CSRF) protection
                //content.Headers.Add("X-CSRF-TOKEN-FORM", cookie);
                //content.Headers.Add("X-CSRF-TOKEN-HEADER", cookie);

                // Send a POST request to the "api/documents" endpoint with the MultipartFormDataContent object
                //var response = await httpClient.PostAsync("api/documents", content);
                var response = await httpClient.PostAsync($"api/upload?prompt=${prompt}", content);

                if (!response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();

                    // if status code 400 or greater, thrown exception so that
                    // exception handler takes care of it
                    if ((int)response.StatusCode == 400)
                    {
                        var error = $"Status Code of 400 returned in UI RestClient: {message}";
                        //_logger.LogError(error);
                        throw new HttpRequestException(error);
                    }

                    if ((int)response.StatusCode == 404)
                    {
                        //logger.LogError($"Status Code of 404 returned in UI RestClient: {message}");
                        var error = $"{response.StatusCode}:{message}";
                        throw new HttpRequestException(error);
                    }

                    if ((int)response.StatusCode >= 500)
                    {
                        var errorMessage = $"Status Code of 500 returned in UI RestClient Post(): {message}";
                        //_logger.LogError(errorMessage);
                        throw new HttpRequestException(errorMessage);
                    }
                }

                // Deserialize the HTTP response content to an UploadDocumentsResponse object

                return await response.Content.ReadFromJsonAsync<Completion>();

                // If the result is null, return an UploadDocumentsResponse indicating an unknown error; otherwise, return the result
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;

                //// If an exception occurs, return an UploadDocumentsResponse indicating the error
                //return UploadDocumentsResponse.FromError(ex.ToString());
            }
        }
    }
}
