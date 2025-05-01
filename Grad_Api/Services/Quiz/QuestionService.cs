using CsvHelper;
using CsvHelper.Configuration;
using Grad_Api.Data;
using System.Globalization;

namespace Grad_Api.Services
{
    public class QuestionService
    {
        public List<Question> ReadQuestionsFromCsv(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            // Map CSV columns to the Question model
            csv.Context.RegisterClassMap<QuestionMap>();
            return csv.GetRecords<Question>().ToList();
        }
    }
    public class QuestionMap : ClassMap<Question>
    {
        public QuestionMap()
        {
            Map(q => q.QuestionText).Name("question text");
            Map(q => q.OptionA).Name("option 1");
            Map(q => q.OptionB).Name("option 2");
            Map(q => q.OptionC).Name("option 3");
            Map(q => q.OptionD).Name("option 4");
            Map(q => q.CorrectAnswer).Name("correct answer");
            Map(q => q.Defficulty).Name("difficulty level");
            Map(q => q.Subject).Name("subject");
            Map(q => q.CourseCategory).Name("prep level");
            Map(q => q.UnitNumber).Name("unit number");
         


        }
    }
}
