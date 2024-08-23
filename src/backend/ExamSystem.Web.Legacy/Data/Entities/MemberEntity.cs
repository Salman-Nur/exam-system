namespace ExamSystem.Web.Data.Entities
{
    public class MemberEntity
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string IP { get; set; }
    }
}
