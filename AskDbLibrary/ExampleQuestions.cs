using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace AskDb.Library
{
    public class ExampleQuestions
    {
        public string ContextDocument { get; set; }
        public IEnumerable<ValueTuple<string,string>> QuestionsAndAnswers { get; set; }

        public ExampleQuestions()
        {
            ContextDocument =
                "<h3>4.10. Separating an Object From Its Background</h3><p>Figure 3.39. Object with Background</p><p>Sometimes you need to separate the subject of an image from its background. You may want to have the subject on a flat color, or keep the background transparent so you can use it on an existing background, or any other thing you have in mind. To do this, you must first use GIMP's selection tools to draw a selection around your subject. This is not an easy task, and selecting the correct tool is crucial. You have several tools to accomplish this:</p><p>The “Free Select Tool” allows you to draw a border using either freehand or straight lines. Use this when the subject has a relatively simple shape. Read more about this tool here: Section 2.4, “Free Selection (Lasso)”</p><p>Figure 3.40. Free Select Tool</p><p>The “Intelligent Scissors Select tool” lets you select a freehand border and uses edge-recognition algorithms to better fit the border around the object. Use this when the subject is complex but distinct enough against its current background. Read more about this tool here: Section 2.7, “Intelligent Scissors”</p><p>Figure 3.41. Intelligent Scissors Select Tool</p><p>The “Foreground Select Tool” lets you mark areas as “Foreground” or “Background” and refines the selection automatically. Read more about this tool here: Section 2.8, “Foreground Select”</p><p>Figure 3.42. Foreground Select Tool</p><h4>4.10.1. Once you have selected your subject</h4><p>Once you have selected your subject successfully, use Select → Invert. Now, instead of the subject, the background is selected. What you do now depends on what you intended to do with the background:</p><p>To fill the background with a single color:</p><p>Click the foreground color swatch (the top left of the two overlapping colored rectangles) in the toolbox and select the desired color. Next,use Section 3.4, “Bucket Fill” to replace the background with your chosen color.</p><p>Figure 3.43. Result of Adding a Plain Color Background</p><p>To make a black-and-white background while keeping the subject in color:</p><p>Use Colors → Desaturate. In the dialog that opens, cycle between the modes and select the best-looking one, then click OK.</p><p>Figure 3.44. Result of Desaturating the Background</p>";
            QuestionsAndAnswers = new List<ValueTuple<string, string>>
            {
                ("How do you select the background of a simple object?",
                    "Use the free select tool to select the object, then use Select →  Invert to select the background"),
                ("How can I make a selection grayscale?",
                    "Make a selection black-and-white using Colors → Desaturate"),
                ("How can I make a pofew?", "I don't know what that is")
            };
        }

        public string[][] ExamplesAsArray()
        {
            return QuestionsAndAnswers.Select(qa => new string[] {qa.Item1, qa.Item2}).ToArray();
        }
    }
}
