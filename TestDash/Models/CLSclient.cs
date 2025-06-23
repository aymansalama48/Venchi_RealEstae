using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding; // تأكد من وجود هذه الـ using
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; // 👈 أضف هذه الـ using لكلاس CLSclient

namespace TestDash.Models
{
    public class CLSclient
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم يجب ألا يزيد عن 100 حرف")]
        public string Name { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        // [RegularExpression(@"^\d{10,15}$", ErrorMessage = "رقم الهاتف يجب أن يكون أرقاماً بين 10-15 خانة")]
        public string Phone { get; set; }

        // هذه الخاصية نملأها في الأكشن ولا نريد binding من الفورم
        // لا يوجد [Required] عليها، لذا [BindNever] كافية.
        [ValidateNever]
        public string Status { get; set; }

        [Required(ErrorMessage = "يجب اختيار مشروع")]
        public int ProjectId { get; set; }

        // نتجاهل الـ navigation property من binding والـ validation
        [ValidateNever] // 👈 التعديل هنا: استخدام [ValidateNever]
        public CLSproject Project { get; set; }

        [Required(ErrorMessage = "المصدر مطلوب")]
        public string Source { get; set; }

        public string? Notes { get; set; }

        // نملأها في الأكشن أيضاً
        // لا يوجد [Required] عليها، لذا [BindNever] كافية.
        [BindNever]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // من binding، نملأ هذا في الأكشن بناءً على الـ Claim
        // لا يوجد [Required] عليها، لذا [BindNever] كافية.
        [ValidateNever]
        [ForeignKey("AddedBy")]
        public int AddedById { get; set; }

        // نتجاهل الـ navigation property من binding والـ validation
        [ValidateNever] // 👈 التعديل هنا: استخدام [ValidateNever]
        public CLSuser AddedBy { get; set; }



        [ValidateNever]
        [ForeignKey("AssignedToEmployeeId")]
        public int? AssignedToEmployeeId { get; set; }

        [ValidateNever]
        public virtual CLSuser? AssignedToEmployee { get; set; }


        [BindNever]
        public DateTime? AssignedAt { get; set; }

        [ValidateNever]
        [ForeignKey("AssignedBy")]
        public int? AssignedById { get; set; } // 👈 ID الأدمن

        [ValidateNever]
        public virtual CLSuser? AssignedBy { get; set; } // 👈 اسم الأدمن


    }
}