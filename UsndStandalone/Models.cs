using System.Xml.Serialization;

namespace UsndStandalone;

[Serializable]
public class LabelGroupData
{
    public string GroupName { get; set; } = string.Empty;
    public List<string> LabelNames { get; set; } = new();
}

// XSD に対応したシンプルなモデル

[XmlRoot("MasterSettings", Namespace = "")]
public class MasterSettings
{
    [XmlAttribute("xsi", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
    public string? Xsi { get; set; }

    [XmlElement("MasterSet")]
    public List<MasterSet> Items { get; set; } = new();
}

public class MasterSet
{
    public string? MasterName { get; set; }
    public string? Volume { get; set; }
}

[XmlRoot("CategorySettings", Namespace = "")]
public class CategorySettings
{
    [XmlAttribute("xsi", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
    public string? Xsi { get; set; }

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

[XmlRoot("LabelSettings", Namespace = "")]
public class LabelSettings
{
    [XmlAttribute("xsi", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
    public string? Xsi { get; set; }

    [XmlElement("LabelSet")]
    public List<LabelSet> Items { get; set; } = new();
}

public class LabelSet
{
    public string? LabelName { get; set; }
    public string? FileName { get; set; }
    public string? Loop { get; set; }
    
    // Loop: デフォルトはFALSE
    public bool ShouldSerializeLoop() => 
        !string.IsNullOrWhiteSpace(Loop) && 
        !Loop.Equals("false", StringComparison.OrdinalIgnoreCase) &&
        !Loop.Equals("FALSE", StringComparison.OrdinalIgnoreCase);
    
    // Volume: デフォルトは1
    public string? Volume { get; set; }
    public bool ShouldSerializeVolume() => 
        !string.IsNullOrWhiteSpace(Volume) && 
        Volume != "1" && Volume != "1.0";
    
    // CategoryBehavior: デフォルトはSTEAL_OLDEST
    public string? CategoryBehavior { get; set; }
    public bool ShouldSerializeCategoryBehavior() => 
        !string.IsNullOrWhiteSpace(CategoryBehavior) && 
        CategoryBehavior.Trim().Length > 0 &&
        !CategoryBehavior.Equals("STEAL_OLDEST", StringComparison.OrdinalIgnoreCase);
    
    // Priority: デフォルトは64
    public string? Priority { get; set; }
    public bool ShouldSerializePriority() => 
        !string.IsNullOrWhiteSpace(Priority) && 
        Priority != "64";
    
    // CategoryName: 空でなければ出力
    public string? CategoryName { get; set; }
    public bool ShouldSerializeCategoryName() => !string.IsNullOrWhiteSpace(CategoryName);
    
    // SingleGroup: 空でなければ出力
    public string? SingleGroup { get; set; }
    public bool ShouldSerializeSingleGroup() => 
        !string.IsNullOrWhiteSpace(SingleGroup) && 
        SingleGroup.Trim().Length > 0;
    
    // MaxNum: デフォルトは0
    public string? MaxNum { get; set; }
    public bool ShouldSerializeMaxNum() => 
        !string.IsNullOrWhiteSpace(MaxNum) && 
        MaxNum != "0";
    
    // IsStealOldest: デフォルトはFALSE
    public string? IsStealOldest { get; set; }
    public bool ShouldSerializeIsStealOldest() => 
        !string.IsNullOrWhiteSpace(IsStealOldest) &&
        !IsStealOldest.Equals("false", StringComparison.OrdinalIgnoreCase) &&
        !IsStealOldest.Equals("FALSE", StringComparison.OrdinalIgnoreCase);
    
    // UnityMixerName: 空でなければ出力
    public string? UnityMixerName { get; set; }
    public bool ShouldSerializeUnityMixerName() => !string.IsNullOrWhiteSpace(UnityMixerName);
    
    // SpatialGroup: 空でなければ出力
    public string? SpatialGroup { get; set; }
    public bool ShouldSerializeSpatialGroup() => !string.IsNullOrWhiteSpace(SpatialGroup);
    
    public string? Delay { get; set; }
    public bool ShouldSerializeDelay() => !string.IsNullOrWhiteSpace(Delay) && Delay != "0" && Delay != "0.0";
    
    public string? Interval { get; set; }
    public bool ShouldSerializeInterval() => !string.IsNullOrWhiteSpace(Interval) && Interval != "0" && Interval != "0.0";
    
    public string? Pan { get; set; }
    public bool ShouldSerializePan() => !string.IsNullOrWhiteSpace(Pan) && Pan != "0" && Pan != "0.0";
    
    public string? Pitch { get; set; }
    public bool ShouldSerializePitch() => !string.IsNullOrWhiteSpace(Pitch) && Pitch != "0" && Pitch != "0.0";
    
    // IsLastSamples: デフォルトはFALSE
    public string? IsLastSamples { get; set; }
    public bool ShouldSerializeIsLastSamples() => 
        !string.IsNullOrWhiteSpace(IsLastSamples) &&
        !IsLastSamples.Equals("false", StringComparison.OrdinalIgnoreCase) &&
        !IsLastSamples.Equals("FALSE", StringComparison.OrdinalIgnoreCase);
    
    public string? FadeInTime { get; set; }
    public bool ShouldSerializeFadeInTime() => !string.IsNullOrWhiteSpace(FadeInTime) && FadeInTime != "0" && FadeInTime != "0.0";
    
    public string? FadeOutTime { get; set; }
    public bool ShouldSerializeFadeOutTime() => !string.IsNullOrWhiteSpace(FadeOutTime) && FadeOutTime != "0" && FadeOutTime != "0.0";
    
    public string? FadeInOldSample { get; set; }
    public bool ShouldSerializeFadeInOldSample() => !string.IsNullOrWhiteSpace(FadeInOldSample) && FadeInOldSample != "0" && FadeInOldSample != "0.0";
    
    public string? FadeOutOnPause { get; set; }
    public bool ShouldSerializeFadeOutOnPause() => !string.IsNullOrWhiteSpace(FadeOutOnPause) && FadeOutOnPause != "0" && FadeOutOnPause != "0.0";
    
    public string? FadeInOffPause { get; set; }
    public bool ShouldSerializeFadeInOffPause() => !string.IsNullOrWhiteSpace(FadeInOffPause) && FadeInOffPause != "0" && FadeInOffPause != "0.0";
    
    // IsVolRnd: デフォルトはFALSE
    public string? IsVolRnd { get; set; }
    public bool ShouldSerializeIsVolRnd() => 
        !string.IsNullOrWhiteSpace(IsVolRnd) &&
        !IsVolRnd.Equals("false", StringComparison.OrdinalIgnoreCase) &&
        !IsVolRnd.Equals("FALSE", StringComparison.OrdinalIgnoreCase);
    
    // IncVol: デフォルトはFALSE
    public string? IncVol { get; set; }
    public bool ShouldSerializeIncVol() => 
        !string.IsNullOrWhiteSpace(IncVol) &&
        !IncVol.Equals("false", StringComparison.OrdinalIgnoreCase) &&
        !IncVol.Equals("FALSE", StringComparison.OrdinalIgnoreCase);
    
    public string? VolRndMin { get; set; }
    public bool ShouldSerializeVolRndMin() => !string.IsNullOrWhiteSpace(VolRndMin) && VolRndMin != "0";
    
    public string? VolRndMax { get; set; }
    public bool ShouldSerializeVolRndMax() => !string.IsNullOrWhiteSpace(VolRndMax) && VolRndMax != "0";
    
    public string? VolRndUnit { get; set; }
    public bool ShouldSerializeVolRndUnit() => !string.IsNullOrWhiteSpace(VolRndUnit) && VolRndUnit != "0";
    
    // IsPitchRnd: デフォルトはFALSE
    public string? IsPitchRnd { get; set; }
    public bool ShouldSerializeIsPitchRnd() => 
        !string.IsNullOrWhiteSpace(IsPitchRnd) &&
        !IsPitchRnd.Equals("false", StringComparison.OrdinalIgnoreCase) &&
        !IsPitchRnd.Equals("FALSE", StringComparison.OrdinalIgnoreCase);
    
    // IncPitch: デフォルトはFALSE
    public string? IncPitch { get; set; }
    public bool ShouldSerializeIncPitch() => 
        !string.IsNullOrWhiteSpace(IncPitch) &&
        !IncPitch.Equals("false", StringComparison.OrdinalIgnoreCase) &&
        !IncPitch.Equals("FALSE", StringComparison.OrdinalIgnoreCase);
    
    public string? PitchRndMin { get; set; }
    public bool ShouldSerializePitchRndMin() => !string.IsNullOrWhiteSpace(PitchRndMin) && PitchRndMin != "0";
    
    public string? PitchRndMax { get; set; }
    public bool ShouldSerializePitchRndMax() => !string.IsNullOrWhiteSpace(PitchRndMax) && PitchRndMax != "0";
    
    public string? PitchRndUnit { get; set; }
    public bool ShouldSerializePitchRndUnit() => !string.IsNullOrWhiteSpace(PitchRndUnit) && PitchRndUnit != "0";
    
    // IsPanRnd: デフォルトはFALSE
    public string? IsPanRnd { get; set; }
    public bool ShouldSerializeIsPanRnd() => 
        !string.IsNullOrWhiteSpace(IsPanRnd) &&
        !IsPanRnd.Equals("false", StringComparison.OrdinalIgnoreCase) &&
        !IsPanRnd.Equals("FALSE", StringComparison.OrdinalIgnoreCase);
    
    // IncPan: デフォルトはFALSE
    public string? IncPan { get; set; }
    public bool ShouldSerializeIncPan() => 
        !string.IsNullOrWhiteSpace(IncPan) &&
        !IncPan.Equals("false", StringComparison.OrdinalIgnoreCase) &&
        !IncPan.Equals("FALSE", StringComparison.OrdinalIgnoreCase);
    
    public string? PanRndMin { get; set; }
    public bool ShouldSerializePanRndMin() => !string.IsNullOrWhiteSpace(PanRndMin) && PanRndMin != "0";
    
    public string? PanRndMax { get; set; }
    public bool ShouldSerializePanRndMax() => !string.IsNullOrWhiteSpace(PanRndMax) && PanRndMax != "0";
    
    public string? PanRndUnit { get; set; }
    public bool ShouldSerializePanRndUnit() => !string.IsNullOrWhiteSpace(PanRndUnit) && PanRndUnit != "0";
    
    // IsRndSrc: デフォルトはFALSE
    public string? IsRndSrc { get; set; }
    public bool ShouldSerializeIsRndSrc() => 
        !string.IsNullOrWhiteSpace(IsRndSrc) &&
        !IsRndSrc.Equals("false", StringComparison.OrdinalIgnoreCase) &&
        !IsRndSrc.Equals("FALSE", StringComparison.OrdinalIgnoreCase);
    
    // IncSrc: デフォルトはFALSE
    public string? IncSrc { get; set; }
    public bool ShouldSerializeIncSrc() => 
        !string.IsNullOrWhiteSpace(IncSrc) &&
        !IncSrc.Equals("false", StringComparison.OrdinalIgnoreCase) &&
        !IncSrc.Equals("FALSE", StringComparison.OrdinalIgnoreCase);
    
    // RndSrc: 空でなければ出力
    public string? RndSrc { get; set; }
    public bool ShouldSerializeRndSrc() => !string.IsNullOrWhiteSpace(RndSrc);
    
    // IsMovePitch: デフォルトはFALSE
    public string? IsMovePitch { get; set; }
    public bool ShouldSerializeIsMovePitch() => 
        !string.IsNullOrWhiteSpace(IsMovePitch) &&
        !IsMovePitch.Equals("false", StringComparison.OrdinalIgnoreCase) &&
        !IsMovePitch.Equals("FALSE", StringComparison.OrdinalIgnoreCase);
    
    public string? PitchStart { get; set; }
    public bool ShouldSerializePitchStart() => !string.IsNullOrWhiteSpace(PitchStart) && PitchStart != "0";
    
    public string? PitchEnd { get; set; }
    public bool ShouldSerializePitchEnd() => !string.IsNullOrWhiteSpace(PitchEnd) && PitchEnd != "0";
    
    public string? PitchMoveTime { get; set; }
    public bool ShouldSerializePitchMoveTime() => !string.IsNullOrWhiteSpace(PitchMoveTime) && PitchMoveTime != "0";
    
    // IsMovePan: デフォルトはFALSE
    public string? IsMovePan { get; set; }
    public bool ShouldSerializeIsMovePan() => 
        !string.IsNullOrWhiteSpace(IsMovePan) &&
        !IsMovePan.Equals("false", StringComparison.OrdinalIgnoreCase) &&
        !IsMovePan.Equals("FALSE", StringComparison.OrdinalIgnoreCase);
    
    public string? PanStart { get; set; }
    public bool ShouldSerializePanStart() => !string.IsNullOrWhiteSpace(PanStart) && PanStart != "0";
    
    public string? PanEnd { get; set; }
    public bool ShouldSerializePanEnd() => !string.IsNullOrWhiteSpace(PanEnd) && PanEnd != "0";
    
    public string? PanMoveTime { get; set; }
    public bool ShouldSerializePanMoveTime() => !string.IsNullOrWhiteSpace(PanMoveTime) && PanMoveTime != "0";
    
    public string? DuckingCategory { get; set; }
    public bool ShouldSerializeDuckingCategory() => !string.IsNullOrWhiteSpace(DuckingCategory);
    
    public string? DuckStart { get; set; }
    public bool ShouldSerializeDuckStart() => !string.IsNullOrWhiteSpace(DuckStart) && DuckStart != "0";
    
    public string? DuckEnd { get; set; }
    public bool ShouldSerializeDuckEnd() => !string.IsNullOrWhiteSpace(DuckEnd) && DuckEnd != "0";
    
    public string? DuckVol { get; set; }
    public bool ShouldSerializeDuckVol() => !string.IsNullOrWhiteSpace(DuckVol) && DuckVol != "1";
    
    // AutoRestore: デフォルトはFALSE
    public string? AutoRestore { get; set; }
    public bool ShouldSerializeAutoRestore() => 
        !string.IsNullOrWhiteSpace(AutoRestore) &&
        !AutoRestore.Equals("false", StringComparison.OrdinalIgnoreCase) &&
        !AutoRestore.Equals("FALSE", StringComparison.OrdinalIgnoreCase);
    
    // RestoreTime: デフォルトは0
    public string? RestoreTime { get; set; }
    public bool ShouldSerializeRestoreTime() => 
        !string.IsNullOrWhiteSpace(RestoreTime) && 
        RestoreTime != "0" && RestoreTime != "0.0";
    
    // IsAndroidNative: デフォルトはFALSE
    public string? IsAndroidNative { get; set; }
    public bool ShouldSerializeIsAndroidNative() => 
        !string.IsNullOrWhiteSpace(IsAndroidNative) &&
        !IsAndroidNative.Equals("false", StringComparison.OrdinalIgnoreCase) &&
        !IsAndroidNative.Equals("FALSE", StringComparison.OrdinalIgnoreCase);
}




