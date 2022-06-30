using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoreCodeCamp.Data.Models
{
    public class CampModel
    {
        [Required]
        [StringLength(maximumLength:100,MinimumLength =1,ErrorMessage ="Camp Name should be 1-100 character")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Moniker is required")]
        public string Moniker { get; set; }
        public DateTime EventDate { get; set; } = DateTime.MinValue;

        [Range(1,200)]
        public int Length { get; set; } = 1;
        public string Venue { get; set; }
        public string LocationAddress1 { get; set; }
        public string LocationAddress2 { get; set; }
        public string LocationAddress3 { get; set; }
        public string LocationCityTown { get; set; }
        public string LocationStateProvince { get; set; }
        public string LocationPostalCode { get; set; }
        public string LocationCountry { get; set; }

        public ICollection<TalkModel> Talks { get; set; }


    }
}
