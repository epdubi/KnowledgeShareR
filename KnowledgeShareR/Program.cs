using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using KnowledgeShareR.Data;
using Microsoft.Extensions.DependencyInjection;

namespace KnowledgeShareR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            CreateDbIfNotExists(host);

            host.Run();
        }

         private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<KnowledgeShareDbContext>();
                    context.Database.EnsureCreated();

                    var isQuestionTableEmpty = context.Questions.Any();
                    var isAnswerTableEmpty = context.Answers.Any();

                    if (!isQuestionTableEmpty)
                    {
                        context.Questions.Add(new Models.Question { Text = "What is your favorite color?", IsActive = true });
                        context.Questions.Add(new Models.Question { Text = "Favorite lunch and learn restaurant?", IsActive = false });
                        context.SaveChanges();
                    }

                    if (!isAnswerTableEmpty)
                    {
                        var firstQuestion = context.Questions.First(x => x.Text == "What is your favorite color?");
                        context.Answers.Add(new Models.Answer { QuestionId = firstQuestion.QuestionId, Text = "Blue", IsCorrect = false });
                        context.Answers.Add(new Models.Answer { QuestionId = firstQuestion.QuestionId, Text = "Green", IsCorrect = true });
                        context.Answers.Add(new Models.Answer { QuestionId = firstQuestion.QuestionId, Text = "Pink", IsCorrect = false });
                        context.Answers.Add(new Models.Answer { QuestionId = firstQuestion.QuestionId, Text = "Orange", IsCorrect = false });

                        var secondQuestion = context.Questions.First(x => x.Text == "Favorite lunch and learn restaurant?");
                        context.Answers.Add(new Models.Answer { QuestionId = secondQuestion.QuestionId, Text = "Jimmy Johns", IsCorrect = false });
                        context.Answers.Add(new Models.Answer { QuestionId = secondQuestion.QuestionId, Text = "China King", IsCorrect = false });
                        context.Answers.Add(new Models.Answer { QuestionId = secondQuestion.QuestionId, Text = "Noodles & Company", IsCorrect = true });
                        context.Answers.Add(new Models.Answer { QuestionId = secondQuestion.QuestionId, Text = "Chick-fil-A", IsCorrect = false });

                        context.SaveChanges();
                    }
                    
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
