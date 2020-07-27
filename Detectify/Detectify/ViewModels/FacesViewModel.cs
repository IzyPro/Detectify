using System.Collections.Generic;
using Plugin.Media.Abstractions;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System.Linq;
using System;
using Acr.UserDialogs;

namespace Detectify.ViewModels
{
    public class FacesViewModel : ViewModelBase
    {

        public FacesViewModel(MediaFile photo, IEnumerable<DetectedFace> detectedFaces)
        {
            try
            {
                Faces = detectedFaces.Select(f => new FaceViewModel(photo, f));
                SelectedFace = Faces.First();
            }
            catch (Exception e)
            {
                UserDialogs.Instance.Toast("No Face Found");
            }
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
