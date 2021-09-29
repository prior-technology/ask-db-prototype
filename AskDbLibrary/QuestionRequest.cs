namespace AskDb.Library
{
    class QuestionRequest
    {
        public string model;
        public string question;
        public string[][] examples;
        public string examples_context;
        public string[] documents;
        //public string file;
        public bool return_metadata;

    }
}
