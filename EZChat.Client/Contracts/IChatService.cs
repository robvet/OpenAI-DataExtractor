using Azure.AI.OpenAI;
using Shared.Models;

namespace EZChat.Client.Contracts
{
    public interface IChatService
    {
        Task<Completion> PostChatCompletion(EZCompletionOptions completionOptions);
    }
}