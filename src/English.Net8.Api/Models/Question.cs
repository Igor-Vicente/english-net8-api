namespace English.Net8.Api.Models
{
    public class Question : Entity
    {
        public string Header { get; set; }
        public Alternative[] Alternatives { get; set; }
        public Difficulty Difficulty { get; set; }
        public QuestionType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RightAnswer { get; set; }
        public string Explanation { get; set; }
        public string Topic { get; set; }
        public string[] Subtopics { get; set; }
    }

    public class Alternative
    {
        public int Id { get; set; }
        public string Content { get; set; }
    }

    public enum QuestionType
    {
        MultipleChoice = 1,
    }

    public enum Difficulty
    {
        Basic = 1,
        Intermediate = 2,
        Advanced = 3
    }
}
