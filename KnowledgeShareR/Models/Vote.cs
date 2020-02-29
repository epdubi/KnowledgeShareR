namespace KnowledgeShareR.Models
{
    public class Vote
    {
        public int VoteId { get; set; }
        public int UserId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
    }
}