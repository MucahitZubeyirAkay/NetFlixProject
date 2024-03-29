using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
	public class MediaRestriction
	{
        public int MediaId { get; set; }

        public byte RestrictionId { get; set; }



        public Media? Media { get; set; }

        public Restriction? Restriction { get; set; }
    }
}

