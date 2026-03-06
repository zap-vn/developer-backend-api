using System;
using ZAP.BuildingBlocks.Models;

namespace ZAP.Report.Application.Features.Reports.DTOs
{
    public class ReportFilterDto : PaginationDto
    {
        public string? Keyword { get; set; }
        public string? Type { get; set; }
    }
}
