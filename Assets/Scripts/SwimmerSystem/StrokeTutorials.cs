using System.Collections.Generic;

public static class StrokeTutorials
{
    public static readonly Dictionary<SwimStroke, string[]> Steps =
        new Dictionary<SwimStroke, string[]>
    {
        { SwimStroke.Freestyle, new[] {
            "Body: long & flat, eyes down.",
            "Arms: high elbow catch; hand enters in line with shoulder.",
            "Breathing: exhale in water, quick inhale to the side.",
            "Kick: small flutter from hips, steady rhythm."
        }},
        { SwimStroke.Breaststroke, new[] {
            "Glide: streamline, head neutral.",
            "Out-sweep: draw a small heart shape with hands.",
            "Breathe during in-sweep; recover arms forward together.",
            "Kick: whip kick—heels up, then snap together."
        }},
        { SwimStroke.Butterfly, new[] {
            "Body wave: chest press then hips up—smooth undulation.",
            "Arms: enter wide, pull under chest, recover low over water.",
            "Breathing: quick forward breath as hands exit.",
            "Kick: two dolphin kicks per stroke cycle."
        }},
        { SwimStroke.Backstroke, new[] {
            "Body: flat, hips high, eyes up.",
            "Arms: pinky enters first; rotate shoulders.",
            "Breathing: steady—no timing constraint.",
            "Kick: steady flutter with small amplitude."
        }},
    };
}
