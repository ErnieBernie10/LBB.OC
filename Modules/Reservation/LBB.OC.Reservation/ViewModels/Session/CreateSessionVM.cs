using System.ComponentModel.DataAnnotations;

namespace LBB.OC.Reservation.ViewModels.Session;

public record CreateSessionVM(CreateSessionVM.CreateSessionForm Form)
{
    public class CreateSessionForm
    {
        [Required] [StringLength(5)] public string Title { get; set; } = "";

        public string Description { get; set; } = "";
    }
}