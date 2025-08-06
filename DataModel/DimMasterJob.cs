using System;
using System.Collections.Generic;

namespace BDA.DataModel;

public partial class DimMasterJob
{
    public string JobId { get; set; } = null!;

    public string JobName { get; set; } = null!;

    public string? Scheduler { get; set; }
}
