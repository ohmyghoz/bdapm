using System;
using System.Collections.Generic;

namespace BDA.DataModel;

public partial class DimJobProc
{
    public string ProcId { get; set; } = null!;

    public string ProcName { get; set; } = null!;

    public string JobId { get; set; } = null!;

    public int SeqNo { get; set; }

    public string ScriptLocation { get; set; } = null!;
}
