using System;
using System.Collections;
using System.Collections.Generic;
using OpenAI;
namespace AskDb.Library
{
    public class QuestionAsker
    {
        public OpenAIClient Client { get; set; }

        public QuestionAsker(OpenAIClient client = null)
        {
            Client = client ??  new OpenAIClient();
        }

        public async void AskQuestion(string question, string fileId)
        {
            var answersEndpoint = Client.AnswersEndpoint;
            if (fileId == null)
            {
                throw new NotImplementedException("Only file-id supported");
            }

            var example1 = new[] { "How do you select all non transparent pixels in a layer?",
                "Use the color picker, set \"select by: alpha\", check \"Select transparent areas\" and set the threshold to 0" };
            var example2 = new[] { "How can I crop a layer without shrinking the canvas?", "Use the \"current layer only\" option in the crop tool" };
            var context =
                "By default the crop tool deals with the image, it also has a Current layer only option. You can also make a selection(any shape) and use Layer > Crop to selection. This will crop the layer to the bounding box of the selection(which is the same as the selection for a rectangle selection).\n"
                + "Select Transparent Areas This option gives the Magic Wand the ability to select areas that are completely transparent.If this option is not checked, transparent areas will never be included in the selection.";


            var results = await answersEndpoint.GetAnswersAsync(question, fileId,new [] {example1,example2}, context);


            foreach (var answerList in results)
            {
                Console.WriteLine("========== Next Result ==========");
                foreach (var answer in answerList)
                {
                    Console.WriteLine(answer);
                }
            }
        }
    }
}
