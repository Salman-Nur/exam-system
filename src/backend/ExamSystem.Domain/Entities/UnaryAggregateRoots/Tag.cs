using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExamSystem.Domain.Entities.Shared;

namespace ExamSystem.Domain.Entities.UnaryAggregateRoots
{
    public class Tag : IEntity<Guid>, ITimestamp
    {
        public required Guid Id { get; init; }
        public required string Name { get; set; }
        public required DateTime CreatedAtUtc { get; set ; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
