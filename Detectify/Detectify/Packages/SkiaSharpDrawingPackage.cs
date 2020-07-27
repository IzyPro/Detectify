using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Detectify.Packages
{
    public class SkiaSharpDrawingPackage
    {
        public void ClearCanvas(SKImageInfo info, SKCanvas canvas)
        {
            var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.White
            };
            canvas.DrawRect(info.Rect, paint);
        }
        public void DrawPrediction(SKCanvas canvas, FaceRectangle rectangle, float left,float top,float scale,string emotion, bool showEmoji)
        {
            var scaledRectangleLeft = left + (scale * rectangle.Left);
            var scaledRectangleWidth = scale * rectangle.Width;
            var scaledRectangleTop = top + (scale * rectangle.Top);
            var scaledRectangleHeight = scale * rectangle.Height;

            if (showEmoji)
            {
                SKBitmap image = GetEmojiBitmap(emotion);
                canvas.DrawBitmap(image, new SKRect(scaledRectangleLeft, scaledRectangleTop, scaledRectangleLeft + scaledRectangleWidth, scaledRectangleTop + scaledRectangleHeight));
            }
            else
            {
                DrawRectangle(canvas, scaledRectangleLeft, scaledRectangleTop, scaledRectangleWidth, scaledRectangleHeight);
                DrawText(canvas, emotion, scaledRectangleLeft, scaledRectangleTop, scaledRectangleWidth, scaledRectangleHeight);
            }
        }
        public void DrawEmoticon(SKImageInfo info, SKCanvas canvas, string emotion)
        {
            SKBitmap image = GetEmojiBitmap(emotion);
            var scale = Math.Min(info.Width / (float)image.Width, info.Height / (float)image.Height);

            var scaleHeight = scale * image.Height;
            var scaleWidth = scale * image.Width;

            var top = (info.Height - scaleHeight) / 2;
            var left = (info.Width - scaleWidth) / 2;

            canvas.DrawBitmap(image, new SKRect(left, top, left + scaleWidth, top + scaleHeight));
        }
        private SKBitmap GetEmojiBitmap(string emotion)
        {
            //string resourceID = GetImageResourceID(emotion).ToString();
            //Assembly assembly = GetType().GetTypeInfo().Assembly;
            //SKBitmap resourceBitmap = null;
            //using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            string resourceID = GetImageResourceID(emotion).ToString();
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            SKBitmap resourceBitmap = null;
            using (Stream stream = assembly.GetManifestResourceStream(resourceID))
            {
                resourceBitmap = SKBitmap.Decode(stream);
            }
            return resourceBitmap;
        }

        private StringBuilder GetImageResourceID(string emotion)
        {
            StringBuilder resID = new StringBuilder("Detectify.Emojis.");
            switch (emotion)
            {
                case "Anger" : resID.Append("Anger");
                    break;
                case "Contempt" : resID.Append("Dislike");
                    break;
                case "Fear": resID.Append("Fear");
                    break;
                case "Disgust": resID.Append("Disgust");
                    break;
                case "Happiness": resID.Append("Happy");
                    break;
                case "Neutral": resID.Append("Neutral");
                    break;
                case "Sadness": resID.Append("Sad");
                    break;
                case "Surprise": resID.Append("Surprise");
                    break;
            }
            resID.Append(".png");
            return resID;
        }
        private SKPath CreateRectanglePath (float startLeft, float startTop, float scaledRectangleWidth, float scaledRectangleHeight)
        {
            var path = new SKPath();
            path.MoveTo(startLeft, startTop);

            path.LineTo(startLeft + scaledRectangleWidth, startTop);
            path.LineTo(startLeft + scaledRectangleWidth, startTop + scaledRectangleHeight);
            path.LineTo(startLeft, startTop + scaledRectangleHeight);
            path.LineTo(startLeft, startTop);

            return path;
        }
        private void DrawRectangle(SKCanvas canvas, SKPaint paint, float startLeft, float startTop, float scaledRectangleWidth, float scaledRectangleHeight)
        {
            var path = CreateRectanglePath(startLeft, startTop, scaledRectangleWidth, scaledRectangleHeight);
            canvas.DrawPath(path, paint);
        }
        private void DrawRectangle(SKCanvas canvas, float startLeft, float startTop, float scaledRectangleWidth, float scaledRectangleHeight)
        {
            var strokePaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Green,
                StrokeWidth = 5,
                //PathEffect = SKPathEffect.CreateDash(new[] { 20f, 20f }, 20f)
            };
            DrawRectangle(canvas, strokePaint, startLeft, startTop, scaledRectangleWidth, scaledRectangleHeight);

            var blurStrokePaint = new SKPaint
            {
                Color = SKColors.Green,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 5,
                PathEffect = SKPathEffect.CreateDash(new[] { 20f, 20f }, 20f),
                IsAntialias = true,
                MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 0.57735f * 1.0f + 0.5f)
            };
            DrawRectangle(canvas, blurStrokePaint, startLeft, startTop, scaledRectangleWidth, scaledRectangleHeight);
        }
        private void DrawText(SKCanvas canvas, string tag, float startLeft, float startTop, float scaledRectangleWidth, float scaledRectangleHeight)
        {
            var textPaint = new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.White,
                Style = SKPaintStyle.Fill,
                Typeface = SKTypeface.FromFamilyName("Montserrat")
            };
            var text = tag;

            var textWidth = textPaint.MeasureText(text);
            textPaint.TextSize = 0.4f * scaledRectangleWidth * textPaint.TextSize / textWidth;

            var textBounds = new SKRect();
            textPaint.MeasureText(text, ref textBounds);

            var xText = startLeft + 10;
            var yText = startTop + (scaledRectangleHeight - 25);

            var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(0, 0, 0, 120)
            };

            var backgroundRect = textBounds;
            backgroundRect.Offset(xText, yText);
            backgroundRect.Inflate(10, 10);

            canvas.DrawRoundRect(backgroundRect, 5, 5, paint);
            canvas.DrawText(text, xText, yText, textPaint);
        }
    }
}
