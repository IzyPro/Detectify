using System;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Plugin.Media.Abstractions;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Detectify.Models;

namespace Detectify.Packages
{
    public class FaceAPI
    {
        private string APIKEY = "0349b12cf6094eeab1600ee79937ee81";
        private string ENDPOINT = "https://detectify.cognitiveservices.azure.com/";
        private FaceClient faceClient;

        public FaceAPI()
        {
            InitFaceClient();
        }

         public async Task<List<MultipleFacesDetected>> GetMultipleFaces(MediaFile image)
        {
            List<MultipleFacesDetected> multipleFaces = null;
            var faceApiResponseList = await faceClient.Face.DetectWithStreamAsync(image.GetStream(), returnFaceAttributes: new List<FaceAttributeType> { { FaceAttributeType.Emotion}});
            MultipleFacesDetected multipleFacesDetected = null;

            if(faceApiResponseList.Count > 0)
            {
                multipleFaces = new List<MultipleFacesDetected>();

                foreach(DetectedFace detectedFace in faceApiResponseList)
                {
                    multipleFacesDetected = new MultipleFacesDetected
                    {
                        FaceRectangle = detectedFace.FaceRectangle,
                    };
                    multipleFacesDetected.PredominantEmotion = FindDetectedEmotion(detectedFace.FaceAttributes.Emotion);
                    multipleFaces.Add(multipleFacesDetected);
                }
            }
            return multipleFaces;
        }

        private string FindDetectedEmotion(Emotion emotion)
        {
            double max = 0;
            PropertyInfo info = null;

            var valueOfEmotions = typeof(Emotion).GetProperties();
            foreach(PropertyInfo propertyInfo in valueOfEmotions)
            {
                var value = (double)propertyInfo.GetValue(emotion);

                if(value > max)
                {
                    max = value;
                    info = propertyInfo;
                }
            }
            return info.Name.ToString();
        }

        void InitFaceClient()
        {
            ApiKeyServiceClientCredentials credentials = new ApiKeyServiceClientCredentials(APIKEY);
            faceClient = new FaceClient(credentials);
            faceClient.Endpoint = ENDPOINT;
            FaceOperations faceOperations = new FaceOperations(faceClient);
        }
    }
}
