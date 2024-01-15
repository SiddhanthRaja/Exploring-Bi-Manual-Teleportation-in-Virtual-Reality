
public class StudyDataLog
{ 
    public int ParticipantId { get; set; }
    public int checkpointIndex { get; set; }
    public System.DateTime timestamp  { get; set; }
    public float t { get; set; }
    public float gt { get; set; }
    public float fovt { get; set; }
    public float height { get; set; }
    public float distance { get; set; }
    public float distanceFromCenter { get; set; }
    public float distanceFromCenterWithSpike { get; set; }
    public int angle { get; set; }
    public string technique { get; set; }
    public string gestureSet { get; set; }

    public string pointerType { get; set; }
    public string dhCombos { get; set; }
    public int mistakeCount { get; set; }

    public string personalizationType { get; set; }

    public string curveCombo { get; set; }

    public float armDistance { get; set; }
    public float armHeight { get; set; }
    public float wristAngle { get; set; }
    public string pointerHand { get; set; }
    public string confirmationType { get; set; }
    public string targetSize { get; set; }


    public string toCSVString()
    {
        return wrap(this.timestamp) + wrap(this.ParticipantId) + wrap(technique) + wrap(gestureSet) + wrap(pointerType)
            + wrap(height) + wrap(distance) + wrap(t) + wrap(fovt) + wrap(gt) + wrap(mistakeCount)
            + wrap(distanceFromCenter) + wrap(distanceFromCenterWithSpike)
            + wrap(armDistance) + wrap(armHeight) + wrap(wristAngle)
            //+ wrap(dhCombos)
            + wrap(personalizationType) + wrap(curveCombo)
            + wrap(pointerHand) + wrap(confirmationType) +  wrap(targetSize)
            + wrap(checkpointIndex, true);
    }

    public static string wrap(object s, bool noTrailingComma = false)
    {
        string trailingComma = ",";
        if (noTrailingComma)
        {
            trailingComma = "";
        }

        if(s is null)
        {
            s = "-NULL-";
        }
        return "\"" + s.ToString() + "\"" + trailingComma;
    }
}
