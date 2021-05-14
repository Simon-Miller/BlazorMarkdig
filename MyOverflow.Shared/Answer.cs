using System;

namespace MyOverflow.Shared
{
    public class Answer : CosmosDbEntityBase
    {
        // GUID?  (Partition Key) -- response to a given Question ID.
        public string QuestionId { get; set; }

        public string Type { get; set; } = "Answer";

        /// <summary>
        /// We'll use the ID of the user for whom the question is for, as our partition key.  So questions and answers are never far apart.
        /// </summary>
        public Guid AnswerForUser { get; set; }

        public string AnswerMarkdown { get; set; }

        public string PostedByUserName { get; set; }
        public DateTime PostedDate { get; set; }
    }
}
