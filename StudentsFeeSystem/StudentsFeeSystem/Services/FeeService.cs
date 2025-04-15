using StudentsFeeSystem.Models;

namespace StudentsFeeSystem.Services
{
    public class FeeService
    {
        private readonly List<Student> students;
        private readonly List<FeeItem> feeItems;

        public FeeService(List<Student> students, List<FeeItem> feeItems)
        {
            this.students = students;
            this.feeItems = feeItems;
        }

        // This method calculates the total amount collected for each fee item
        public Dictionary<int, decimal> CalculateTotalAmountForAllFeeItems()
        {
            var totalAmounts = new Dictionary<int, decimal>();

            // Loop through all fee items
            foreach (var feeItem in feeItems)
            {
                // Calculate the total amount for this fee item
                decimal totalAmountForThisFeeItem = 0;

                foreach (var student in students)
                {
                    // Check if the student is eligible to pay for this fee item
                    if (IsStudentEligibleForFeeItem(student, feeItem))
                    {
                        // Add the student's fee to the total
                        totalAmountForThisFeeItem += student.Fee ?? 0;
                    }
                }

                // Store the total amount for the current fee item
                totalAmounts.Add(feeItem.Id, totalAmountForThisFeeItem);
            }

            return totalAmounts;
        }

        // This method checks if a student is eligible for a specific fee item
        private bool IsStudentEligibleForFeeItem(Student student, FeeItem feeItem)
        {
            // Check gender eligibility
            bool isGenderEligible = (feeItem.AssignedToMale && student.Gender == Gender.Male) ||
                                     (feeItem.AssignedToFemale && student.Gender == Gender.Female);

            // Check class eligibility
            bool isClassEligible = (feeItem.AssignedToClass6 && student.Class == 6) ||
                                   (feeItem.AssignedToClass7 && student.Class == 7) ||
                                   (feeItem.AssignedToClass8 && student.Class == 8) ||
                                   (feeItem.AssignedToClass9 && student.Class == 9) ||
                                   (feeItem.AssignedToClass10 && student.Class == 10) ||
                                   (feeItem.AssignedToClass11 && student.Class == 11) ||
                                   (feeItem.AssignedToClass12 && student.Class == 12);

            return isGenderEligible && isClassEligible;
        }
    }

}
