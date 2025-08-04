using System;
using System.Collections.Generic;

namespace BDA.DataModel;

public partial class FctLogProcess
{
    public string? Periode { get; set; }

    public string? MonthId { get; set; }

    public string? ProcId { get; set; }

    public string? DataName { get; set; }

    public string? Path { get; set; }

    public int? FileRec { get; set; }

    public string? FlagStatus { get; set; }

    public string? Status { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }
}
