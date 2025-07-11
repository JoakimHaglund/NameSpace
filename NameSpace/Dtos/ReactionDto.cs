﻿using NameSpace.Models;

namespace NameSpace.Dtos
{
    public class ReactionDto
    {
        public required Guid NameInfoId { get; set; }
        public required int Reaction { get; set; }
        public int? PartnerReaction { get; set; }
    }
}
