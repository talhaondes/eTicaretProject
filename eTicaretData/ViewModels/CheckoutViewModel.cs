using Microsoft.AspNetCore.Mvc.Rendering;

namespace eTicaretData.ViewModels
{
    public class CheckoutViewModel
    {
        public ShippingDetail ShippingDetail { get; set; }
        public List<SelectListItem> Addresses { get; set; }
        public int? SelectedAddressId { get; set; }
    }
}
