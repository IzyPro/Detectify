using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace Detectify.Models
{
    public class DetectedFaceExtended : DetectedFace
    {
        public string PredominantEmotion { get; set; }
    }
}
