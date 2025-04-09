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
        public string Name { get; set; }
        [Required(ErrorMessage = "Fathers Name is Required.")]
        [DisplayName("Father's Name")]
        public string FathersName { get; set; }
        [Required(ErrorMessage = "Roll is Required.")]
        [DisplayName("Roll Number")]
        [Remote(action: "CheckRoll", controller: "Student", AdditionalFields = "IsEdit")]
        public int Roll {  get; set; }
        [Required(ErrorMessage = "Class is Required.")]
        [DisplayName("Class")]
        public bool IsEdit { get; set; }
        public int Class {  get; set; }
        [DisplayName("Date")]
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
