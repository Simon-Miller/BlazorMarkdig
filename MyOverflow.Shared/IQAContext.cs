using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;

namespace MyOverflow.Shared
{
    public interface IQAContext
    {
        /// <summary>
        /// Save a new question or save changes to an existing question.
        /// </summary>
        Task<ItemResponse<Question>> StoreQuestion(Question question);

        /// <summary>
        /// Save a new answer to a question, or store the changes to an existing answer.
        /// </summary>
        Task<ItemResponse<Answer>> StoreAnswer(Answer answer);

        /// <summary>
        /// this would be useful if we want to create hyperlinks to questions.
        /// </summary>
        Task<ItemResponse<Question>> GetQuestionById(Guid id);

        /// <summary>
        /// a full text index based search returning ranked results.
        /// </summary>
        Task GetQuestionsBySearchText(string searchText);

        /// <summary>
        /// get all the answers to a given question that we identify by its id.
        /// </summary>
        Task GetAnswersToQuestion(Guid questionId);
    }
}
