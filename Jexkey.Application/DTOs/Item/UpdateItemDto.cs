using Jexkey.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jexkey.Application.DTOs.Item {
    public class UpdateItemDto {
        public ItemType ItemType { get; set; }
        public required string EncTitle { get; set; }
        public string? EncPassword { get; set; }
        public string? EncUsername { get; set; }
        public string? EncUrl { get; set; }
        public string? EncNote { get; set; }
    }
}
