using System;
using System.IO;
using System.Threading.Tasks;
using Busidex.Http.Utils;
using Busidex.Models.Constants;
using Busidex.Professional.ViewModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Stormlion.ImageCropper;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex.Professional.Views.EditCard
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditCardImageView
	{
        protected CardVM _viewModel { get; set; }
        public UserCardDisplay DisplaySettings { get; set; }

        public EditCardImageView (ref CardVM vm)
		{
			InitializeComponent ();

            // var fileName = vm.SelectedCard.DisplaySettings.CurrentFileName;

            Title = "Choose your card picture";

            // vm.SelectedCard.DisplaySettings = new UserCardDisplay(fileName: fileName);
            DisplaySettings = new UserCardDisplay(
                DisplaySetting.Detail,
                vm.SelectedCard.Card.FrontOrientation == "H"
                    ? CardOrientation.Horizontal
                    : CardOrientation.Vertical,
                vm.SelectedCard.Card.FrontFileName,
                vm.SelectedCard.Card.FrontOrientation);

            _viewModel = vm;
            BindingContext = _viewModel;

            _viewModel.BackOrientation = vm.SelectedCard.Card.BackOrientation;
            _viewModel.FrontOrientation = vm.SelectedCard.Card.FrontOrientation;

            _viewModel.SelectedCardFrontImage = _viewModel.SelectedCard.Card.FrontFileName;
            _viewModel.SelectedCardBackImage = _viewModel.SelectedCard.Card.BackFileName;

            Task.Factory.StartNew(async () => await CheckImagesExist());

            setControls();
		}

        private async Task<bool> CheckImagesExist()
        {
            var fname = Path.Combine (Serialization.LocalStorageFolder, _viewModel.SelectedCardFrontImage);
            var bname = Path.Combine (Serialization.LocalStorageFolder, _viewModel.SelectedCardBackImage);
            if (File.Exists(fname) && File.Exists(bname)) return true;

            var card = await App.LoadOwnedCard(useThumbnail: false);
            return card != null;
        }

        private async void BtnSave_OnClicked(object sender, EventArgs e)
        {            
            await _viewModel.SaveCardImage();
        }

        private void BtnFront_OnClicked(object sender, EventArgs e)
        {
            btnFront.Style = (Style) Application.Current.Resources["toggleButtonOn"];
            btnBack.Style = (Style) Application.Current.Resources["toggleButtonOff"];
            _viewModel.SelectedSide = CardSide.Front;
            
            setControls();
        }

        private void BtnBack_OnClicked(object sender, EventArgs e)
        {
            btnFront.Style = (Style) Application.Current.Resources["toggleButtonOff"];
            btnBack.Style = (Style) Application.Current.Resources["toggleButtonOn"];
            _viewModel.SelectedSide = CardSide.Back;
                                    
            setControls();
        }

        private async void BtnChooseImage_OnClicked(object sender, EventArgs e)
        {
            const string OPT_CANCEL = "Cancel";
            const string OPT_CAMERA = "Camera";
            const string OPT_GALLERY = "Gallery";
            const string OPT_CLEAR = "Clear Image";

            var action = await DisplayActionSheet("Image Source", OPT_CANCEL, null, OPT_CAMERA, OPT_GALLERY, OPT_CLEAR);
            switch (action)
            {
                case OPT_CANCEL:
                {
                    break;
                }
                case OPT_CAMERA:
                {
                    takePicture();
                    break;
                }
                case OPT_GALLERY:
                {
                    choosePicture();
                    break;
                }
                case OPT_CLEAR:
                {
                    var forReals = await DisplayAlert("Clear Image", "Clear your card image?", "Yes", "Cancel");
                    if (forReals)
                    {
                        clearPicture();
                    }
                    
                    break;
                }
            }
        }

        async Task<PermissionStatus> checkPermissions(Permission permission)
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
            if (status != PermissionStatus.Granted)
            {
                var shouldShow = await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(permission);
                if (shouldShow || true)
                {
                    // await DisplayAlert("Permission", "Allow access your camera", "OK");
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { permission });
                    status = results[permission];
                }
            }

            return status;
        }

        async void clearPicture()
        {
            await setImageFromFile(null);
        }

        async void choosePicture()
        {
            var status = await checkPermissions(Permission.Storage);

            if (status == PermissionStatus.Granted)
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await DisplayAlert("Choose Picture", ":( Gallery is not available.", "OK");
                    return;
                }

                var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Small,
                    CompressionQuality = 80
                });

                if (file == null)
                    return;

                await setImageFromFile(file);
            }
            else if (status != PermissionStatus.Unknown)
            {
                await DisplayAlert("Gallery Denied", "Can not continue. This app needs to access your photo gallery. Please check your permissions and try again.", "OK");
            }
        }

        async void takePicture()
        {
            var status = await checkPermissions(Permission.Camera);

            if (status == PermissionStatus.Granted)
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await DisplayAlert("No Camera", ":( No camera available.", "OK");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    //AllowCropping = true,
                    PhotoSize = PhotoSize.Small,
                    CompressionQuality = 80
                });

                if (file == null)
                    return;

                var MAX_IMAGE_SIZE = 1024 * 225;
                var size = file.GetStream().Length;
                file.GetStream().Seek(0, SeekOrigin.Begin);
                if(size > MAX_IMAGE_SIZE)
                {
                    await DisplayAlert("Image Size", "Please choose an image smaller than 224KB", "Ok");
                    return;
                }
                await setImageFromFile(file);
                
            }
            else //if (status != PermissionStatus.Unknown)
            {
                await DisplayAlert("Camera Denied", "Can not continue. This app needs to access your camera. Please check your permissions and try again.", "OK");
            }
        }

        async Task<bool> setImageFromFile(MediaFile file)
        {
            bool ok = true;
            try
            {
                var str = file?.GetStream();

                var imageFile = string.Empty;

                new ImageCropper()
                {
                    Success = (img) =>
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            imageFile = img;
                            //imageView.Source = ImageSource.FromFile(img);
                            if (_viewModel.SelectedSide == CardSide.Front)
                            {
                                imgSelectedFrontImage.Source = ImageSource.FromFile(imageFile);
                            }
                            else
                            {
                                imgSelectedBackImage.Source = ImageSource.FromFile(imageFile);
                            }

                            byte[] b = { };
                            if (str != null)
                            {
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    str.CopyTo(ms);
                                    b = ms.ToArray();
                                }
                            }

                            var s = Convert.ToBase64String(b);
                            if (_viewModel.SelectedSide == CardSide.Front)
                            {
                                _viewModel.EncodedFrontCardImage = s != string.Empty ? s : null;
                                _viewModel.FrontFileId = s != string.Empty ? Guid.NewGuid() : Guid.Empty;
                                _viewModel.FrontImageChanged = true;
                            }
                            else
                            {
                                _viewModel.EncodedBackCardImage = s != string.Empty ? s : null;
                                _viewModel.BackFileId = s != string.Empty ? Guid.NewGuid() : Guid.Empty;
                                _viewModel.BackImageChanged = true;
                            }
                        });
                    }
                }.Show(this, file?.Path);

                //var cropResult = file != null
                //    ? await CropImageService.Instance.CropImage(file.Path, CropRatioType.None)
                //    : null;

                
            }
            catch
            {
                ok = false;
            }

            return await Task.FromResult(ok);
        }

        private void rdoFrontOrientation_OnClicked(object sender, EventArgs e)
        {
            if (!(sender is Plugin.InputKit.Shared.Controls.RadioButton radio))
            {
                return;
            }

            _viewModel.FrontOrientation = radio.Value.ToString();
            _viewModel.FrontOrientationChanged = _viewModel.FrontOrientation != _viewModel.SelectedCard.Card.FrontOrientation;
            setControls();
        }

        private void rdoBackOrientation_OnClicked(object sender, EventArgs e)
        {
            if (!(sender is Plugin.InputKit.Shared.Controls.RadioButton radio))
            {
                return;
            }

            _viewModel.BackOrientation = radio.Value.ToString();
            _viewModel.BackOrientationChanged = _viewModel.BackOrientation != _viewModel.SelectedCard.Card.BackOrientation;
            setControls();
        }

        private void setControls()
        {
            double vFrameHeight = 300;
            double vFrameWidth = 183;
            double vImageHeight = 250;
            double vImageWidth = 152;
            double hFrameHeight = 183;
            double hFrameWidth = 300;
            double hImageHeight = 152;
            double hImageWidth = 250;
            
            var frontOrientation = _viewModel.SelectedCard.Card.FrontOrientation;
            var backOrientation = _viewModel.SelectedCard.Card.BackOrientation;
            var selectedOrientation = string.Empty;

            if (_viewModel.SelectedSide == CardSide.Front)
            {
                selectedOrientation = _viewModel.FrontOrientation;
                frmSelectedCardImage.HeightRequest = selectedOrientation == "H"
                    ? hFrameHeight
                    : vFrameHeight;
                frmSelectedCardImage.WidthRequest = selectedOrientation == "H"
                    ? hFrameWidth
                    : vFrameWidth;
                imgSelectedFrontImage.HeightRequest = selectedOrientation == "H"
                    ? hImageHeight
                    : vImageHeight;
                imgSelectedFrontImage.WidthRequest = selectedOrientation == "H"
                    ? hImageWidth
                    : vImageWidth;
            }
            if (_viewModel.SelectedSide == CardSide.Back)
            {
                selectedOrientation = _viewModel.BackOrientation;
                frmSelectedCardImage.HeightRequest = selectedOrientation == "H"
                    ? hFrameHeight
                    : vFrameHeight;
                frmSelectedCardImage.WidthRequest = selectedOrientation == "H"
                    ? hFrameWidth
                    : vFrameWidth;
                imgSelectedFrontImage.HeightRequest = selectedOrientation == "H"
                    ? hImageHeight
                    : vImageHeight;
                imgSelectedFrontImage.WidthRequest = selectedOrientation == "H"
                    ? hImageWidth
                    : vImageWidth;
            }

            rdoFrontBtnHorizontal.IsChecked = _viewModel.FrontOrientation == "H";
            rdoBackBtnHorizontal.IsChecked = _viewModel.BackOrientation == "H";;
            rdoFrontBtnVertical.IsChecked = _viewModel.FrontOrientation == "V";
            rdoBackBtnVertical.IsChecked = _viewModel.BackOrientation == "V";;   
            
            _viewModel.SelectedCard.Card.FrontOrientation = frontOrientation;
            _viewModel.SelectedCard.Card.BackOrientation = backOrientation;
        }
    }
}