using Azure.AI.OpenAI;
using Shared.Models;

namespace EZChat.Client.Contracts
{
    public interface ISimpleChatService
    {
        Task<Completion> PostChatCompletion(EZCompletionOptions completionOptions);
    }
}