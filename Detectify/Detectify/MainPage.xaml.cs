using Acr.UserDialogs;
using Detectify.Models;
using Detectify.Packages;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Detectify
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private Lazy<List<MultipleFacesDetected>> multipleFaces = new Lazy<List<MultipleFacesDetected>>();
        private FaceAPI faceAPI;
        private SKBitmap image;
        private SkiaSharpDrawingPackage drawingPackage;
        private bool drawEmoji = true;
        public MainPage()
        {
            InitializeComponent();
            //NavigationPage.SetHasNavigationBar(this, false);
            Init();
        }

        private async void Init()
        {
            faceAPI = new FaceAPI();
            drawingPackage = new SkiaSharpDrawingPackage();
            await CrossMedia.Current.Initialize();
            TakePictureAndAnalizeImage();
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            var mode = sender as Switch;
            if (mode.IsToggled)
            {
                drawEmoji = true;
            }
            else
            {
                drawEmoji = false;
            }
        }

        private void Capture_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            var info = e.Info;
            var canvas = e.Surface.Canvas;
            drawingPackage.ClearCanvas(info, canvas);
            if(image != null)
            {
                var scale = Math.Min(info.Width / (float)image.Width, info.Height / (float)image.Height);
                var scaleHeight = scale * image.Height;
                var scaleWidth = scale * image.Width;
                var top = (info.Height - scaleHeight) / 2;
                var left = (info.Width - scaleWidth) / 2;

                canvas.DrawBitmap(image, new SKRect(left, top, left + scaleWidth, top + scaleHeight));

                if(multipleFaces.Value.Count > 0)
                {
                    foreach(var face in multipleFaces.Value)
                    {
                        drawingPackage.DrawPrediction(canvas, face.FaceRectangle, left, top, scale, face.PredominantEmotion, drawEmoji);
                    }
                }
            }
        }
        private void HideProgressDialog()
        {
            UserDialogs.Instance.HideLoading();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            TakePictureAndAnalizeImage();
        }
        private void SetImageInImageView(MediaFile mediaImage)
        {
            image = SKBitmap.Decode(mediaImage.GetStreamWithImageRotatedForExternalStorage());
            Capture.InvalidateSurface();
        }
        private void ShowProgressDialog()
        {
            UserDialogs.Instance.ShowLoading("Analyzing", MaskType.Black);
        }
        private async Task<MediaFile> TakePicture()
        {
            image = null;
            MediaFile mediaFile = null;
            if(CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                mediaFile = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Medium,
                    RotateImage = true,
                    DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Front,
                    Directory = "FaceAPI",
                    Name = "face.jpg"
                });
            }
            else
            {
                await DisplayAlert("Camera not Found", ":(No Camera Available.", "OK");
            }
            return mediaFile;
        }
        private async void TakePictureAndAnalizeImage()
        {
            var capturedImage = await TakePicture();
            if(multipleFaces.Value.Count > 0)
            {
                multipleFaces.Value.Clear();
            }
            if(capturedImage != null)
            {
                ShowProgressDialog();
                SetImageInImageView(capturedImage);
                try
                {
                    var foundFaces = await faceAPI.GetMultipleFaces(capturedImage);
                    if(foundFaces != null && foundFaces.Count > 0)
                    {
                        multipleFaces.Value.AddRange(foundFaces);
                        Capture.InvalidateSurface();
                    }
                    else
                    {
                        UserDialogs.Instance.Toast("No Face Found");
                    }
                    HideProgressDialog();
                }
                catch(Exception e)
                {
                    HideProgressDialog();
                    UserDialogs.Instance.Toast("No Face Found");
                }
            }
        }
    }
}
