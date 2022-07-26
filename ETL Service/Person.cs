using System;

namespace ETL_Service
{
    public class Person
    {
        private string _firstName;
        private string _lastName;
        private long _accountNumber;
        private DateTime _date;
        private decimal _payment;
        private string _city;
        private string _service;


        public Person(string firstName, string lastName, long accountNumber, DateTime date, decimal payment, string city, string service)
        {
            _firstName = firstName;
            _lastName = lastName;
            _accountNumber = accountNumber;
            _date = date;
            _payment = payment;
            _city = city;
            _service= service;
        }
        
        // public string FirstName { get; set; }
        // public string LastName { get; set; }
        // public long AccountNumber { get; set; }
        //public DateTime Date { get; set; }
        //public decimal Payment { get; set; }
        
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }
        public long AccountNumber
        {
            get { return _accountNumber; }
            set { _accountNumber = value; }
        }
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }
        public decimal Payment
        {
            get { return _payment; }
            set { _payment = value; }
        }
        public string City
        {
            get { return _city; }
            set { _city = value; }
        }
        public string Service
        {
            get { return _service; }
            set { _service = value; }
        }
    }
}