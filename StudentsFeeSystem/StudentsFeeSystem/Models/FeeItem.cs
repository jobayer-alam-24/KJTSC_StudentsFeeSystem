namespace StudentsFeeSystem.Models
{
    public class FeeItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public bool AssignedToMale { get; set; }
        public bool AssignedToFemale { get; set; }
        public bool AssignedToClass6 { get; set; }
        public bool AssignedToClass7 { get; set; }
        public bool AssignedToClass8 { get; set; }
        public bool AssignedToClass9 { get; set; }
        public bool AssignedToClass10 { get; set; }
        public bool AssignedToClass11 { get; set; }
        public bool AssignedToClass12 { get; set; }
        public virtual ICollection<GenderFeeAssignment> GenderAssignments { get; set; }
    }

}
