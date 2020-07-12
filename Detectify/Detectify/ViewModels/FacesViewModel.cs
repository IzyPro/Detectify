using System.Collections.Generic;
using Plugin.Media.Abstractions;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.Linq;

namespace Detectify.ViewModels
{
    public class FacesViewModel : ViewModelBase
    {

        public FacesViewModel(MediaFile photo, IEnumerable<DetectedFace> detectedFaces)
        {
            Faces = detectedFaces.Select(f => new FaceViewModel(photo, f));
            SelectedFace = Faces.First();
        }
        public IEnumerable<FaceViewModel> Faces { get; }
        FaceViewModel _selectedFace;
        public FaceViewModel SelectedFace
        {
            get => _selectedFace;
            set => Set(ref _selectedFace, value);
        }
    }
}
