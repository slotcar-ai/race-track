using System.ComponentModel.DataAnnotations;
using Scai.Driver.Data;

namespace Scai.Driver.Web.Dto
{
    public class RunRequest
    {
        [StringLength(3000, MinimumLength = 1)]
        [Required]
        public string Code { get; set; }

        [Required]
        public TrackState TrackState { get; set; }
    }
}
