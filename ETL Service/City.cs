namespace ETL_Service
{
    public class City
    {
        private string _name;
        private decimal _total;

        public City(string name, decimal total)
        {
            _name = name;
            _total = total;
        }
        
        // public string Name { get; set; }
        // public decimal Total { get; set; }
        
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public decimal Total
        {
            get { return _total; }
            set { _total = value; }
        }
    }
}