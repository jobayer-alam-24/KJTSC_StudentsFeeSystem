using StudentsFeeSystem.Models;

namespace StudentsFeeSystem.ViewModel
{
    public class MakePaymentViewModel
    {
        public Student Student { get; set; }
        public List<FeeItemViewModel> FeeItems { get; set; }
    }

}
