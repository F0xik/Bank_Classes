using System;
using System.Collections.Generic;

namespace Bank.Classes
{
    internal class Client
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string PassportNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<BankAccount> Accounts { get; set; } = new List<BankAccount>();

        public Client( string surname, string name, string patronymic, string passportNumber, DateTime dateOfBirth)
        {
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            PassportNumber = passportNumber;
            DateOfBirth = dateOfBirth;

        }

    }
}
