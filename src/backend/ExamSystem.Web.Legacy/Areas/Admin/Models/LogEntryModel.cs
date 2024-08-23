namespace ExamSystem.Web.Areas.Admin.Models
{
    public class LogEntryModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public LogLevels Level { get; set; }
        public DateTime Time { get; set; }
        public string LevelName => Level.ToString();
    }

    public enum LogLevels
    {
        Trace = 1,
        Debug,
        Information,
        Warning,
        Error,
        Critical
    }
}
