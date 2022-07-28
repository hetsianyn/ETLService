namespace ETL_Service
{
    public class City
    {
        private string _name;

        public City(string name)
        {
            _name = name;
        }
        
        // public string Name { get; set; }
        // public decimal Total { get; set; }
        
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}