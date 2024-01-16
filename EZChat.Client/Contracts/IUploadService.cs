using EZChat.Client.Models;
using Shared.Models;

namespace EZChat.Client.Contracts
{
    public interface IUploadService
    {
        Task<Completion> UploadDocumentsAsync(WebAssemblyTicket ticket, string prompt);
    }
}