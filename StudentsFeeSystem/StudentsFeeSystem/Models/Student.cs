using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace StudentsFeeSystem.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Student Name is Required.")]
        [DisplayName("Student Name")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Fathers Name is Required.")]
        [DisplayName("Father's Name")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Father's Name must be between 3 and 100 characters.")]
        public string FathersName { get; set; }

        [Required(ErrorMessage = "Roll is Required.")]
        [DisplayName("Roll Number")]
        [Remote(action: "CheckRoll", controller: "Student", AdditionalFields = "Class,IsEdit,Id")]
        [Range(1, int.MaxValue, ErrorMessage = "Roll must be a positive number.")]
        public int Roll { get; set; }
        public bool IsEdit { get; set; }

        [Required(ErrorMessage = "Class is Required.")]
        [Range(6, 12, ErrorMessage = "Class must be between 6 and 12.")]
        [DisplayName("Class")]
        public int Class { get; set; }

        [DisplayName("Date")]
        [Range(0, double.MaxValue, ErrorMessage = "Fee must be a positive number.")]
        public decimal? Fee { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Department is Required.")]
        [Display(Name = "Department")]
        public bool HasPaid { get; set; } = false;

        public Department Department { get; set; }
    }
    public enum Department
    { 
        Farm,
        Machinery,
        [Description("IT Support & IOT Basics")]
        ITSupport,
        [Description("General Electronics")]
        Genereal_Electronics,
        Automobile
    }

}
