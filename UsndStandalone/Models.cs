using System.Xml.Serialization;

namespace UsndStandalone;

// XSD に対応したシンプルなモデル

[XmlRoot("MasterSettings")]
public class MasterSettings
{
    [XmlElement("MasterSet")]
    public List<MasterSet> Items { get; set; } = new();
}

public class MasterSet
{
    public string? MasterName { get; set; }
    public string? Volume { get; set; }
}

[XmlRoot("CategorySettings")]
public class CategorySettings
{
    [XmlElement("CategorySet")]
    public List<CategorySet> Items { get; set; } = new();
}

public class CategorySet
{
    public string? CategoryName { get; set; }
    public string? Volume { get; set; }
    public string? MaxNum { get; set; }
    public string? MasterName { get; set; }
}

[XmlRoot("LabelSettings")]
public class LabelSettings
{
    [XmlElement("LabelSet")]
    public List<LabelSet> Items { get; set; } = new();
}

public class LabelSet
{
    public string? LabelName { get; set; }
    public string? FileName { get; set; }
    public string? Loop { get; set; }
    public string? Volume { get; set; }
    public string? CategoryBehavior { get; set; }
    public string? Priority { get; set; }
    public string? CategoryName { get; set; }
    public string? SingleGroup { get; set; }
    public string? MaxNum { get; set; }
    public string? IsStealOldest { get; set; }
    public string? UnityMixerName { get; set; }
    public string? SpatialGroup { get; set; }
    public string? Delay { get; set; }
    public string? Interval { get; set; }
    public string? Pan { get; set; }
    public string? Pitch { get; set; }
    public string? IsLastSamples { get; set; }
    public string? FadeInTime { get; set; }
    public string? FadeOutTime { get; set; }
    public string? FadeInOldSample { get; set; }
    public string? FadeOutOnPause { get; set; }
    public string? FadeInOffPause { get; set; }
    public string? IsVolRnd { get; set; }
    public string? IncVol { get; set; }
    public string? VolRndMin { get; set; }
    public string? VolRndMax { get; set; }
    public string? VolRndUnit { get; set; }
    public string? IsPitchRnd { get; set; }
    public string? IncPitch { get; set; }
    public string? PitchRndMin { get; set; }
    public string? PitchRndMax { get; set; }
    public string? PitchRndUnit { get; set; }
    public string? IsPanRnd { get; set; }
    public string? IncPan { get; set; }
    public string? PanRndMin { get; set; }
    public string? PanRndMax { get; set; }
    public string? PanRndUnit { get; set; }
    public string? IsRndSrc { get; set; }
    public string? IncSrc { get; set; }
    public string? RndSrc { get; set; }
    public string? IsMovePitch { get; set; }
    public string? PitchStart { get; set; }
    public string? PitchEnd { get; set; }
    public string? PitchMoveTime { get; set; }
    public string? IsMovePan { get; set; }
    public string? PanStart { get; set; }
    public string? PanEnd { get; set; }
    public string? PanMoveTime { get; set; }
    public string? DuckingCategory { get; set; }
    public string? DuckStart { get; set; }
    public string? DuckEnd { get; set; }
    public string? DuckVol { get; set; }
    public string? AutoRestore { get; set; }
    public string? RestoreTime { get; set; }
    public string? IsAndroidNative { get; set; }
}




