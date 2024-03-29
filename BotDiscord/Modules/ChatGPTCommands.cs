﻿using BotDiscord.Common;
using BotDiscord.Models;
using Discord;
using Discord.Commands;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;

namespace BotDiscord.Modules
{
    public sealed class ChatGPTCommands : ModuleBase<SocketCommandContext>
    {
        [Command("ctm", RunMode = RunMode.Async)]
        public async Task ChatGPT([Remainder] string? texto)
        {
            CompletionCreateResponse respChatGPT = await ObterCompletionCreateResponse(texto);
            ChatGPTResponse chatGPTResponse = ObterChatGPTResponse(respChatGPT);

            if (!chatGPTResponse.Successful)
            {
                await Logger.Log(LogSeverity.Critical, $"{nameof(ChatGPTCommands)}", chatGPTResponse?.Error?.Message ?? string.Empty);

                string erro = !string.IsNullOrEmpty(chatGPTResponse?.Error?.Message) ? $": {chatGPTResponse?.Error?.Message}" : string.Empty;
                await Context.Message.ReplyAsync($"Ops, {Context.User.Username}! Parece que houve um erro{erro}");
                return;
            }

            await Context.Message.ReplyAsync(text: chatGPTResponse?.Choices![0].Text);
        }

        private static async Task<CompletionCreateResponse> ObterCompletionCreateResponse(string? texto)
        {
            OpenAIService gpt3 = new(new OpenAiOptions()
            {
                ApiKey = StaticKeys.ChatGPTApiKey
            });

            CompletionCreateResponse respChatGPT = await gpt3.Completions.CreateCompletion(new CompletionCreateRequest()
            {
                Prompt = texto ?? string.Empty,
                Model = OpenAI.ObjectModels.Models.Gpt_3_5_Turbo,
                Temperature = 0.5F,
                MaxTokens = 2,
                N = 1
            });

            return respChatGPT;
        }

        private static ChatGPTResponse ObterChatGPTResponse(CompletionCreateResponse respChatGPT)
        {
            ChatGPTResponse response = new()
            {
                Choices = respChatGPT.Choices,
                CreatedAt = respChatGPT.CreatedAt,
                Error = respChatGPT.Error,
                Id = respChatGPT.Id,
                Model = respChatGPT.Model,
                Successful = respChatGPT.Successful
            };

            return response;
        }
    }
}