namespace StudentsFeeSystem.Models
{
    public class GenderFeeAssignment
    {
        public int Id { get; set; }
        public int FeeItemId { get; set; }
        public Gender Gender { get; set; }

        public virtual FeeItem FeeItem { get; set; }
    }
    
    
}
