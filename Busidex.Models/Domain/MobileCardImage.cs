using System;

namespace Busidex.Models.Domain
{
    public class MobileCardImage
    {
        //public enum DisplayMode
        //{
        //    Front = 0,
        //    Back = 1
        //}

        public Guid? FrontFileId { get; set; }
        public Guid? BackFileId { get; set; }
        public string FrontOrientation { get; set; }
        public string BackOrientation { get; set; }
        public string EncodedCardFrontImage { get; set; }
        public string EncodedCardBackImage { get; set; }
        //public DisplayMode Side { get; set; }
    }
}

