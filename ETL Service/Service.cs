namespace ETL_Service
{
    public class Service
    {
        private string _name;
        private string _city;
        private decimal _payment;

        public Service(string name, string city, decimal payment)
        {
            _name = name;
            _city = city;
            _payment = payment;
        }
        
        // public string Name { get; set; }
        // public decimal Total { get; set; }
        
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string City
        {
            get { return _city; }
            set { _city = value; }
        }
        public decimal Payment
        {
            get { return _payment; }
            set { _payment = value; }
        }
    }
}