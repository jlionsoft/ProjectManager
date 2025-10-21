using ProjectManager.Helper;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ProjectManager.Model
{
    public class User
    {
        public int Id { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Fullname
        {
            get => !string.IsNullOrEmpty(Firstname) || !string.IsNullOrEmpty(Lastname)
                ? $"{Firstname ?? ""}{(string.IsNullOrEmpty(Lastname) ? "" : " ")}{Lastname ?? ""}"
                : "Kein Name vorhanden";
        }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool imageHasBeenSet = false;
        public byte[]? ImageData { get; set; }

        private ImageSource? imageSource;

        [NotMapped]
        public ImageSource? Image
        {
            get
            {
                if (imageSource == null && ImageData != null)
                {
                    imageSource = ImageHelper.ConvertToImageSource(ImageData);
                }
                return imageSource;
            }
            set
            {
                imageSource = value;
                imageHasBeenSet = true;
                ImageData = ImageHelper.ImageSourceToBinary(value!);
            }
        }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
        public User()
        {
            
        }
    }
}
