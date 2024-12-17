using System;

namespace Bank.Classes
{
    public class BankAccount
    {
        public string AccountNumber { get; set; }
        public DateTime OpeningDate { get; set; }
        public decimal Balance { get; set; }
        public int DepositTermInMonths { get; set; }
        public string OwnerPassportNumber { get; set; }

        public BankAccount(string accountNumber, string ownerPassportNumber, decimal initialBalance, int depositTermInMonths)
        {
            AccountNumber = accountNumber;
            OwnerPassportNumber = ownerPassportNumber;
            OpeningDate = DateTime.Now;
            Balance = initialBalance;
            DepositTermInMonths = depositTermInMonths;
        }
    }
}
