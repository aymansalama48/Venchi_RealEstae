using System.ComponentModel.DataAnnotations;

namespace TestDash.Models
{
    public class CLSproject
    {
        [Key]
        public int Id { get; set; }  // بيتولد تلقائي

        [Required(ErrorMessage = "اسم المشروع مطلوب")]
        public string Name { get; set; }  // اسم المشروع

        [Required(ErrorMessage = "رابط الصورة مطلوب")]
        [Display(Name = "رابط الصورة")]
        public string ImageUrl { get; set; }  // رابط الصورة (من الإنترنت أو من مجلد داخل الموقع)

        [Required(ErrorMessage = "الموقع مطلوب")]
        public string Location { get; set; }  // الموقع (مثلاً: القاهرة الجديدة)
    }
}
