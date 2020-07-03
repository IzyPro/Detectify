using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Detectify.Models
{
    public class MultipleFacesDetected : DetectedFace
    {
        public string PredominantEmotion { get; set; }
    }
}
