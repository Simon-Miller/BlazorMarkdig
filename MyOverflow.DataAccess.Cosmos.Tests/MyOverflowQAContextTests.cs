using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace MyOverflow.DataAccess.Cosmos.Tests
{
    [TestClass]
    public class MyOverflowQAContextTests
    {

        [TestMethod]
        public void Can_do_something()
        {
            // Arrange
            var ctx = new FakeStartup();
            var api = new MyOverflowQAContext(ctx.Instance.Configuration);
            var task = Task.Run(async()=> await api.Initialize());
            task.Wait();

            // Act
            task = Task.Run(async () => 
            {
                var question = new Shared.Question
                {
                    PostedByUserName = "IntegrationTestUser",
                    Title = "Integration Test",
                    QuestionMarkdown = "Integration Test question",
                    Type="Question"
                };

                try
                {
                    var response = await api.StoreQuestion(question);
                }
                catch (Exception ex)
                { 
                }
            });

            task.Wait();

            // Assert

        }


    }
}
