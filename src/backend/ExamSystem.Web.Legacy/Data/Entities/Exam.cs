namespace ExamSystem.Web.Data.Entities
{
    public class Exam
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Score { get; set; }
        public int Participant { get; set; }
        public string Type { get; set; }
    }
}
