using SkiaSharp;
using System;
using System.ComponentModel;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestProject.Views
{
    public partial class AboutPage : ContentPage
    {
        private string imagePath;

        public AboutPage()
        {
            InitializeComponent();
            this.BindingContext = this;
        }

        private async void OnSelectImageClicked(object sender, EventArgs e)
        {
            var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions { Title = "Test image" });

            if (photo == null)
                return;

            var folder = Path.Combine(FileSystem.AppDataDirectory, "Media");
            var imageName = $"{DateTime.Now.Ticks}.jpeg";
            Directory.CreateDirectory(folder);
            imagePath = Path.Combine(folder, imageName);

            using (var photoStream = await photo.OpenReadAsync())
            using (var targetStream = File.OpenWrite(imagePath))
            {
                await photoStream.CopyToAsync(targetStream);
                imageView.Source = ImageSource.FromFile(imagePath);
                OnPropertyChanged();
            }
        }

        private void OnCompressImageClicked(object sender, EventArgs e)
        {
            using (var stream = File.OpenRead(imagePath))
            {
                SKImage img = SKImage.FromEncodedData(stream);
                SKBitmap bitmap = SKBitmap.FromImage(img);

                using (MemoryStream memStream = new MemoryStream())
                using (SKManagedWStream wstream = new SKManagedWStream(memStream))
                {
                    bitmap.Encode(wstream, SKEncodedImageFormat.Jpeg, 100);
                    imageCompressedView.Source = ImageSource.FromStream(() => new MemoryStream(memStream.ToArray()));
                }
            }


            /*var codec = SKCodec.Create(imagePath);
            var skImage = SKBitmap.Decode(codec);

            using (MemoryStream memStream = new MemoryStream())
            using (SKManagedWStream wstream = new SKManagedWStream(memStream))
            {
                skImage.Encode(wstream, SKEncodedImageFormat.Jpeg, 100);
                imageCompressedView.Source = ImageSource.FromStream(() => new MemoryStream(memStream.ToArray()));
            }*/
        }
    }
}