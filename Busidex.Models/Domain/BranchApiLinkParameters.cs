namespace Busidex.Models.Domain
{
    public class BranchApiLinkParameters
    {
        public string branch_key { get; set; }

        public string sdk { get; set; }

        public string campaign { get; set; }

        public string feature { get; set; }

        public string channel { get; set; }

        public string[] tags { get; set; }

        public string data { get; set; }
    }
}

