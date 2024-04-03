using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftitoFlix.Models
{
    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Surname { get; set; } = "";
    }
}