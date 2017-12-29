using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using unirest_net.http;
using System.IO;

namespace MemeBot
{
    public enum Meme
    {
        Brian, Fry, GrumpyCat, OMG, Spiderman, Troll, Wonka
    }

    [Serializable]
    public class MemeAPI
    {
        private static string RAPID_API_KEY = "<RAPID_API_KEY>";

        /// <summary>
        /// This methods calls the APIMeme ( http://apimeme.com/ ) through RapidAPI ( https://rapidapi.com/user/ronreiter/package/Meme%20Generator )
        /// and returns the generated meme as Base64String
        /// </summary>
        public async Task<string> GenerateMeme(Meme meme, string topText, string bottomText)
        {
            var response = await Unirest.get($"https://ronreiter-meme-generator.p.mashape.com/meme?bottom={ HttpUtility.HtmlEncode(bottomText) }&font=Impact&font_size=50&meme={ ConvertMemeToURL(meme) }&top={ HttpUtility.HtmlEncode(topText) }")
                .header("X-Mashape-Key", RAPID_API_KEY )
                .header("X-Mashape-Host", "ronreiter-meme-generator.p.mashape.com")
            .asJsonAsync<MemoryStream>();

            return Convert.ToBase64String(response.Body.ToArray());
        }

        private string ConvertMemeToURL(Meme meme)
        {
            switch(meme)
            {
                case Meme.Brian:
                    return "Bad-Luck-Brian";
                case Meme.Fry:
                    return "Futurama-Fry";
                case Meme.GrumpyCat:
                    return "Grumpy-Cat";
                case Meme.OMG:
                    return "OMG-Cat";
                case Meme.Spiderman:
                    return "Spiderman-Computer-Desk";
                case Meme.Troll:
                    return "Troll-Face";
                case Meme.Wonka:
                    return "Condescending-Wonka";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}