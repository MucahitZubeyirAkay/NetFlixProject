using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class MediaRestriction
	{
        [ForeignKey(nameof(MediaId))]
        public int MediaId { get; set; }

        [ForeignKey(nameof(RestrictionId))]
        public int RestrictionId { get; set; }



        public Media? Media { get; set; }

        public Restriction? Restriction { get; set; }
    }
}

