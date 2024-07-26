namespace English.Net8.Api.Models
{
    public class QuestionTopics : Entity
    {
        public string Topic { get; set; }
        public string[] Subtopics { get; set; }
    }
}
