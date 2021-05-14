using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using MyOverflow.Shared;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyOverflow.DataAccess.Cosmos
{
    // see better ways here: https://www.c-sharpcorner.com/article/appsettings-6-ways-to-read-the-config-in-asp-net-core-3-0/

    public class MyOverflowQAContext : IQAContext
    {
        private const int TIMEOUT = 10 * 1000; // 10 seconds.

        public MyOverflowQAContext(IConfiguration configuration)
        {
            /* I want to get access to :
                    EndpointUri,
                    PrimaryAccessKey,
                    ApplicationName,
                    DatabaseName
             */

            var configSection = configuration.GetSection("MyOverflow.DataAccess.Cosmos");

            this.endpointUri = configSection["EndpointUri"];
            this.primaryAccessKey = configSection["PrimaryAccessKey"];
            this.applicationName = configSection["ApplicationName"];
            this.databaseName = configSection["DatabaseName"];
            this.defaultRequestUnitsProvision = int.Parse(configSection["DefaultRequestUnitsProvision"]);
        }

        private readonly string endpointUri;
        private readonly string primaryAccessKey;
        private readonly string applicationName;
        private readonly string databaseName;
        private readonly int defaultRequestUnitsProvision;

        // so we follow best practices, and don't but complex code (or async code) inside the constructor.
        // we'll need to call this to ensure the instance is fully initialized.
        // we could do that with a factory function when registering this.
        public async Task Initialize() 
        {
            var client = new CosmosClient(this.endpointUri, this.primaryAccessKey, new CosmosClientOptions()
            {
                ApplicationName = applicationName
            });

            var database = (Database)await client.CreateDatabaseIfNotExistsAsync(databaseName);

            this.QuestionsContainer = await getOrMakeContainer(("questions", $"/{nameof(Question.UserId)}", defaultRequestUnitsProvision));
            this.AnswersContainer = await getOrMakeContainer(("answers", $"/{nameof(Answer.AnswerForUser)}", defaultRequestUnitsProvision));

            // inline function
            async Task<Container> getOrMakeContainer((string id, string partitionKeyPath, int throughput) containerConfig) => 
                await database.CreateContainerIfNotExistsAsync(
                    id: containerConfig.id,
                    partitionKeyPath: containerConfig.partitionKeyPath,
                    throughput: containerConfig.throughput
                );

            // setup checks complete. Containers populated.
        }

        protected Container QuestionsContainer;
        protected Container AnswersContainer;

        private async Task<ItemResponse<T>> saveWrap<T>(T entityToSave) where T : CosmosDbEntityBase
        {
            if (string.IsNullOrWhiteSpace(entityToSave.Id))
            {
                // for mega resilience, and that one in a 2^128 (bits) or 10^38 (approx) chance a collision,
                // we could test the failure to create (duplicate) and assign another GUID to the Id, and try again..

                // see also: https://github.com/Azure/azure-cosmos-dotnet-v3/issues/815
                // best practice for us to generate the ID instead of Cosmos... its cheaper!

                entityToSave.Id = Guid.NewGuid().ToString();
                return await this.QuestionsContainer.CreateItemAsync(entityToSave);
            }
            else
                return await this.QuestionsContainer.ReplaceItemAsync(entityToSave, entityToSave.Id);
        }

        public Task<ItemResponse<Question>> StoreQuestion(Question question)
        {
            return saveWrap(question);
        }

        public Task<ItemResponse<Answer>> StoreAnswer(Answer answer)
        {
            return saveWrap(answer);
        }

        public Task<ItemResponse<Question>> GetQuestionById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task GetQuestionsBySearchText(string searchText)
        {
            throw new NotImplementedException();
        }

        public Task GetAnswersToQuestion(Guid questionId)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// you can use this factory to create an instance.
    /// It forces you to await instantiation, which can take some time - before you can first use the context.
    /// </summary>
    public class MyOverflowQAContextFactory
    {
        public MyOverflowQAContextFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private readonly IConfiguration configuration;

        public async Task<MyOverflowQAContext> Make()
        {
            var instance = new MyOverflowQAContext(this.configuration);
            await instance.Initialize();

            return instance;
        }
    }
}
