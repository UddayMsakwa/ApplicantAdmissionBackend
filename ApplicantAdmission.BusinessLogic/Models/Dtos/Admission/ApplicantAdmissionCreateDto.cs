namespace ApplicantAdmission.BusinessLogic.Models.Dtos.Admission
{
    public class ApplicantAdmissionCreateDto
    {
        public Guid ApplicantId { get; set; }
        public Guid AdmissionProgramId { get; set; }
    }
}
