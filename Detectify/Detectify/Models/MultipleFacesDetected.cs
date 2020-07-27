using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace Detectify.Models
{
    public class MultipleFacesDetected : DetectedFace
    {
        public string PredominantEmotion { get; set; }
    }
}
