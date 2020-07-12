using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;

namespace Detectify.ViewModels
{
    public class FaceViewModel : ViewModelBase
    {
        public StreamImageSource Photo { get; }
        public string Description { get; }
        public string Details { get; }

        public FaceViewModel (MediaFile photo, DetectedFace detectedFace)
        {
            Photo = (StreamImageSource)ImageSource.FromStream(() => photo.GetStreamWithImageRotatedForExternalStorage());

            var builder = new StringBuilder();
            builder.AppendLine($"Age: {detectedFace.FaceAttributes.Age} years old");
            builder.AppendLine($"Gender: {detectedFace.FaceAttributes.Gender}");
            builder.AppendLine($"Hair: {GetHair(detectedFace)}");
            builder.AppendLine($"Facial Hair: { GetFacialHair(detectedFace) }");
            builder.AppendLine($"Glasses: {detectedFace.FaceAttributes.Glasses}");
            builder.AppendLine($"Makeup: {GetMakeup(detectedFace)}");
            builder.AppendLine($"Emotion: {GetEmotion(detectedFace)}");

            Details = builder.ToString();
            Description = $"{detectedFace.FaceAttributes.Age} year old {detectedFace.FaceAttributes.Gender}";
        }

        private static string GetMakeup(DetectedFace detectedFace)
        {
            var makeup = (new[]
            {
                detectedFace.FaceAttributes.Makeup.EyeMakeup ? "Eyes" : "",
                detectedFace.FaceAttributes.Makeup.LipMakeup ? "Lips" : "",
            }).Where(m => !string.IsNullOrEmpty(m));

            var makeups = string.Join(", ", makeup);
            return string.IsNullOrEmpty(makeups) ? "None" : makeups;
        }
        private string GetHair(DetectedFace detectedFace)
        {
            if (detectedFace.FaceAttributes.Hair.Invisible)
                return "Hidden";
            if (detectedFace.FaceAttributes.Hair.Bald > 0.75)
                return "Bald";
            var hairColor = detectedFace.FaceAttributes.Hair.HairColor.OrderByDescending(h => h.Confidence).FirstOrDefault();
            if (hairColor == null)
                return "Unknown";
            return $"{hairColor.Color}";
        }
        private string GetFacialHair(DetectedFace detectedFace)
        {
            if (detectedFace.FaceAttributes.FacialHair.Beard < 0.1 &&
                detectedFace.FaceAttributes.FacialHair.Moustache < 0.1 &&
                detectedFace.FaceAttributes.FacialHair.Sideburns < 0.1)
                return "None";
            return $"Beard ({ detectedFace.FaceAttributes.FacialHair.Beard}), " +
                $"Moustache ({ detectedFace.FaceAttributes.FacialHair.Moustache}), " +
                $"Sideburns ({ detectedFace.FaceAttributes.FacialHair.Sideburns})";
        }
        private string GetEmotion(DetectedFace detectedFace)
        {
            var emotion = new Dictionary<String, double>
            {
                {nameof(detectedFace.FaceAttributes.Emotion.Anger), detectedFace.FaceAttributes.Emotion.Anger },
                {nameof(detectedFace.FaceAttributes.Emotion.Contempt), detectedFace.FaceAttributes.Emotion.Contempt },
                {nameof(detectedFace.FaceAttributes.Emotion.Disgust), detectedFace.FaceAttributes.Emotion.Disgust },
                {nameof(detectedFace.FaceAttributes.Emotion.Fear), detectedFace.FaceAttributes.Emotion.Fear },
                {nameof(detectedFace.FaceAttributes.Emotion.Happiness), detectedFace.FaceAttributes.Emotion.Happiness },
                {nameof(detectedFace.FaceAttributes.Emotion.Neutral), detectedFace.FaceAttributes.Emotion.Neutral },
                {nameof(detectedFace.FaceAttributes.Emotion.Sadness), detectedFace.FaceAttributes.Emotion.Sadness },
                {nameof(detectedFace.FaceAttributes.Emotion.Surprise), detectedFace.FaceAttributes.Emotion.Surprise },
            };
            return emotion.OrderByDescending(e => e.Value).First().Key;
        }
    }
}
