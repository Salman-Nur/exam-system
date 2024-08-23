using ExamSystem.Domain.Entities.UnaryAggregateRoots;
using ExamSystem.Infrastructure.Identity.Constants;

namespace ExamSystem.Infrastructure.Persistence.Seeds;

public static class SystemClaimsSeed
{
    public static SystemClaim[] SystemClaims
    {
        get
        {
            return new[]
            {
                new SystemClaim
                {
                    Id = Guid.Parse("c32ad88e-09f7-c6f0-ce5b-08dc8937bc4f"),
                    Name = ClaimTypeConstants.Member
                },
                new SystemClaim
                {
                    Id = Guid.Parse("1ef4babe-2dfa-ca2d-eb41-08dc8937be87"),
                    Name = ClaimTypeConstants.InternalUser
                },
                new SystemClaim
                {
                    Id = Guid.Parse("3eaa627b-e56f-c685-235d-08dc8937c081"),
                    Name = ClaimTypeConstants.ViewDashboard
                },
                new SystemClaim
                {
                    Id = Guid.Parse("7990988a-cf75-ce00-cb47-08dc8937c377"),
                    Name = ClaimTypeConstants.ManageMemberClaim
                },
                new SystemClaim
                {
                    Id = Guid.Parse("844b3328-080f-c935-ffea-08dc8937c5b2"),
                    Name = ClaimTypeConstants.ManageMember
                },
                new SystemClaim
                {
                    Id = Guid.Parse("abf0c086-0c63-c906-7dca-08dc8937c7e8"),
                    Name = ClaimTypeConstants.ManageQuestion
                },
                new SystemClaim
                {
                    Id = Guid.Parse("4e70efc8-77af-c047-2c9a-08dc8937ca23"),
                    Name = ClaimTypeConstants.QuestionCreate
                },
                new SystemClaim
                {
                    Id = Guid.Parse("ff3058a6-bcf3-c296-ba35-08dc8937cd57"),
                    Name = ClaimTypeConstants.QuestionView
                },
                new SystemClaim
                {
                    Id = Guid.Parse("3bf76b81-16fb-cf73-09d8-08dc8937d04b"),
                    Name = ClaimTypeConstants.QuestionEdit
                },
                new SystemClaim
                {
                    Id = Guid.Parse("b7842991-b887-cfc5-d19b-08dc8937d37f"),
                    Name = ClaimTypeConstants.QuestionDelete
                },
                new SystemClaim
                {
                    Id = Guid.Parse("dde5d028-8ce1-cc74-c2ad-08dc8937d5f6"),
                    Name = ClaimTypeConstants.ManageExam
                },
                new SystemClaim
                {
                    Id = Guid.Parse("ccfd2634-ada3-cc17-d019-08dc8937d82f"),
                    Name = ClaimTypeConstants.ExamCreate
                },
                new SystemClaim
                {
                    Id = Guid.Parse("61dc3de3-65a1-c21d-da2d-08dc8937daa7"),
                    Name = ClaimTypeConstants.ExamView
                },
                new SystemClaim
                {
                    Id = Guid.Parse("e262c5dc-94e5-c8f4-8e30-08dc8937dce0"),
                    Name = ClaimTypeConstants.ExamEdit
                },
                new SystemClaim
                {
                    Id = Guid.Parse("684433eb-bf84-cc23-f24a-08dc8937ded9"),
                    Name = ClaimTypeConstants.ExamDelete
                },
                new SystemClaim
                {
                    Id = Guid.Parse("589e7ea7-754e-c471-f4e3-08dc8937e29d"),
                    Name = ClaimTypeConstants.ManageLog
                }
            };
        }
    }
}
