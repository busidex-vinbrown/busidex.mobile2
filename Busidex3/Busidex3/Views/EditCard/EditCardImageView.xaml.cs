using System;
using System.IO;
using System.Threading.Tasks;
using Busidex3.DomainModels;
using Busidex3.ViewModels;
using Plugin.InputKit.Shared.Controls;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarians.CropImage;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Busidex3.Views.EditCard
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditCardImageView
	{
        protected CardVM ViewModel { get; set; }

		public EditCardImageView (ref CardVM vm)
		{
			InitializeComponent ();

            var fileName = vm.SelectedCard.DisplaySettings.CurrentFileName;

            Title = "Choose your card picture";

            vm.SelectedCard.DisplaySettings = new UserCardDisplay(fileName: fileName);
            ViewModel = vm;
            BindingContext = ViewModel;

            ViewModel.BackOrientation = vm.SelectedCard.Card.BackOrientation;
            ViewModel.FrontOrientation = vm.SelectedCard.Card.FrontOrientation;

            ViewModel.SelectedCardFrontImage = ViewModel.SelectedCard.Card.FrontFileName;
            ViewModel.SelectedCardBackImage = ViewModel.SelectedCard.Card.BackFileName;

            setControls();
		}

        private async void BtnSave_OnClicked(object sender, EventArgs e)
        {            
            await ViewModel.SaveCardImage();
        }

        private void BtnFront_OnClicked(object sender, EventArgs e)
        {
            btnFront.Style = (Style) Application.Current.Resources["toggleButtonOn"];
            btnBack.Style = (Style) Application.Current.Resources["toggleButtonOff"];
            ViewModel.SelectedSide = UserCardDisplay.CardSide.Front;
            
            setControls();
        }

        private void BtnBack_OnClicked(object sender, EventArgs e)
        {
            btnFront.Style = (Style) Application.Current.Resources["toggleButtonOff"];
            btnBack.Style = (Style) Application.Current.Resources["toggleButtonOn"];
            ViewModel.SelectedSide = UserCardDisplay.CardSide.Back;
                                    
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

        async Task<PermissionStatus> checkPermissions()
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                {
                    await DisplayAlert("Camera Permission", "Allow SavR to access your camera", "OK");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera });
                status = results[Permission.Camera];
            }

            return status;
        }

        async void clearPicture()
        {
            await setImageFromFile(null);
        }

        async void choosePicture()
        {
            var status = await checkPermissions();

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
                    CompressionQuality = 70
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
            var status = await checkPermissions();

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
                    AllowCropping = true,
                    PhotoSize = PhotoSize.Small,
                    CompressionQuality = 70
                });

                if (file == null)
                    return;

                await setImageFromFile(file);
                
            }
            else if (status != PermissionStatus.Unknown)
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

                var cropResult = file != null
                    ? await CropImageService.Instance.CropImage(file.Path, CropRatioType.None)
                    : null;

                if (ViewModel.SelectedSide == UserCardDisplay.CardSide.Front)
                {
                    imgSelectedFrontImage.Source = ImageSource.FromFile(cropResult?.FilePath);
                }
                else
                {
                    imgSelectedBackImage.Source = ImageSource.FromFile(cropResult?.FilePath);
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
                if (ViewModel.SelectedSide == UserCardDisplay.CardSide.Front)
                {
                    ViewModel.EncodedFrontCardImage = s != string.Empty ? s : null;
                    ViewModel.FrontFileId = s != string.Empty ? Guid.NewGuid() : Guid.Empty;
                    ViewModel.FrontImageChanged = true;
                }
                else
                {
                    ViewModel.EncodedBackCardImage = s != string.Empty ? s : null;
                    ViewModel.BackFileId = s != string.Empty ? Guid.NewGuid() : Guid.Empty;
                    ViewModel.BackImageChanged = true;
                }
            }
            catch (Exception ex)
            {
                ok = false;
            }

            return await Task.FromResult(ok);
        }

        private void rdoFrontOrientation_OnClicked(object sender, EventArgs e)
        {
            if (!(sender is RadioButton radio))
            {
                return;
            }

            ViewModel.FrontOrientation = radio.Value.ToString();
            setControls();
        }

        private void rdoBackOrientation_OnClicked(object sender, EventArgs e)
        {
            if (!(sender is RadioButton radio))
            {
                return;
            }

            ViewModel.BackOrientation = radio.Value.ToString();
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
            
            var frontOrientation = ViewModel.SelectedCard.Card.FrontOrientation;
            var backOrientation = ViewModel.SelectedCard.Card.BackOrientation;
            var selectedOrientation = string.Empty;

            if (ViewModel.SelectedSide == UserCardDisplay.CardSide.Front)
            {
                selectedOrientation = ViewModel.FrontOrientation;
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
            if (ViewModel.SelectedSide == UserCardDisplay.CardSide.Back)
            {
                selectedOrientation = ViewModel.BackOrientation;
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

            rdoFrontBtnHorizontal.IsChecked = ViewModel.FrontOrientation == "H";
            rdoBackBtnHorizontal.IsChecked = ViewModel.BackOrientation == "H";;
            rdoFrontBtnVertical.IsChecked = ViewModel.FrontOrientation == "V";
            rdoBackBtnVertical.IsChecked = ViewModel.BackOrientation == "V";;   
            
            ViewModel.SelectedCard.Card.FrontOrientation = frontOrientation;
            ViewModel.SelectedCard.Card.BackOrientation = backOrientation;
        }
    }
}