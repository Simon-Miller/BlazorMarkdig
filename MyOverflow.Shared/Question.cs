using System;

namespace MyOverflow.Shared
{
    public class Question : CosmosDbEntityBase
    {
        public string Type { get; set; } = "Question";

        /// <summary>
        /// we'll use this as partition key.  We'll want to groups questions and answers together in the same container.
        /// </summary>
        public Guid UserId { get; set; }
        public string PostedByUserName { get; set; } // denormalized

        public string Title { get; set; }
        public string QuestionMarkdown { get; set; }


        // not needed when creating a question, but later on when a user comes back to select which answer best fits their question.
        public int? SelectedAnswerId { get; set; }
    }
}
