using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;

namespace MemeBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private MemeAPI _api = new MemeAPI();
        private Meme _meme;
        private string _topText;
        private string _bottomText;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var memes = new Meme[] { Meme.Brian, Meme.Fry, Meme.GrumpyCat, Meme.OMG, Meme.Spiderman, Meme.Troll, Meme.Wonka };
            var descriptions = new string[] { "Bad luck Brian", "Fry", "Grumpy Cat", "OMG", "Spiderman", "Troll", "Wonka" };
            PromptDialog.Choice<Meme>(context, ResumeAfterMemeSelectionClarification,
                memes, "Hi, I'm MemeBot. Which meme should I generate?", descriptions: descriptions);
        }

        private async Task ResumeAfterMemeSelectionClarification(IDialogContext context, IAwaitable<Meme> result)
        {
            _meme = await result;
            PromptDialog.Text(context, ResumeAfterTopTextClarification, "Awesome, I can work with that. What will the *top text* be?");
        }

        private async Task ResumeAfterTopTextClarification(IDialogContext context, IAwaitable<string> result)
        {
            _topText = await result;
            PromptDialog.Text(context, ResumeAfterBottomTextClarification, "Good, now what will the *bottom text* be?");
        }

        private async Task ResumeAfterBottomTextClarification(IDialogContext context, IAwaitable<string> result)
        {
            _bottomText = await result;

            // setup a message that can hold the generated image
            var replyMessage = context.MakeMessage();
            var image = new Attachment();
            image.ContentType = "image/jpeg";
            image.ContentUrl = $"data:image/jpeg;base64,{ await _api.GenerateMeme(_meme, _topText, _bottomText) }";
            replyMessage.Attachments = new List<Attachment> { image };

            // return our reply to the user
            await context.PostAsync(replyMessage);
            await context.PostAsync("I hope you like your meme. If not, generate a new one!");

            context.Wait(MessageReceivedAsync);
        }
    }
}