using System;
using System.Collections.Generic;

namespace BDA.DataModel;

public partial class DimIoProc
{
    public string ProcId { get; set; } = null!;

    public string DataId { get; set; } = null!;

    public string IoStatus { get; set; } = null!;
}
