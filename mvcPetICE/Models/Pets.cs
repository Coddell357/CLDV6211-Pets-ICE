using System.ComponentModel.DataAnnotations;

namespace mvcPetICE.Models
{
    public class Pets
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Breed {  get; set; }
        public string AdoptionStatus { get; set; }
        public string Images {  get; set; }
    }
}
