namespace Domain.Entities
{
    public class Example : EntityBase
    {
        public Example(string zipCode, 
                       string street, 
                       string complement, 
                       string unit, 
                       string neighborhood, 
                       string city, 
                       string state)
        {
            ZipCode = zipCode;
            Street = street;
            Complement = complement;
            Unit = unit;
            Neighborhood = neighborhood;
            City = city;
            State = state;
        }

        public string ZipCode { get; private set; } = string.Empty;
        public string Street { get; private set; } = string.Empty;
        public string Complement { get; private set; } = string.Empty;
        public string Unit { get; private set; } = string.Empty;
        public string Neighborhood { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public string State { get; private set; } = string.Empty;
    }
}
