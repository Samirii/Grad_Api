using System.Text.Json.Serialization;

namespace Grad_Api.Models.PythonAi
{
    public class FlaskPerformanceInput
    {

        [JsonPropertyName("AvgPreviousScore")] // Explicitly map to expected name
        public double AvgPreviousScore { get; set; }

        [JsonPropertyName("NumQuizzesTaken")]
        public int NumQuizzesTaken { get; set; }

        [JsonPropertyName("LastScore")]
        public double LastScore { get; set; }
    }
}