using System.ComponentModel.DataAnnotations;

namespace TestDash.Models
{
    public class CLSuser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }  // أحمد السيد

        public string JobTitle { get; set; }  // مدير مبيعات

        public string? Department { get; set; }  // قسم المبيعات

        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public DateTime HireDate { get; set; }  // تاريخ التعيين

        public string? Notes { get; set; }  // ملاحظات عن الموظف

        public string PasswordHash { get; set; }  // كلمة المرور (مش واضحة في التصميم لكن مهم لو فيه تسجيل دخول)

        public string Role { get; set; }  // Admin / Employee / Manager ...etc
        public string? ProfileImageFileName { get; set; }  // اسم صورة الموظف

    }
}
