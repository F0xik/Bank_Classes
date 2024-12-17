using Bank.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Newtonsoft.Json;





namespace Bank
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        static string filePath { get; set; }
        static public int count;




        private void SaveFile()
        {
            string fileSaveName = "clientBank";
            string fileFolderPath = System.IO.Path.GetTempPath();
            filePath = fileFolderPath + fileSaveName;

            if (File.Exists(filePath) == false)
            {
                var file = File.Create(filePath);
                file.Close();

            }
        }

        public MainWindow()
        {
            InitializeComponent();
            SaveFile();
            ListNumCards();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void Clear_DataUser(object sender, RoutedEventArgs e)
        {
            Clear_Data();
            InfoAddUser.Clear();
        }

        private void Clear_Data()
        {
            userName.Clear();
            userFamily.Clear();
            userMiddleName.Clear();
            userNumberPass.Clear();
            datePicker1.SelectedDate = null;
        }


        private void ListNumCards()
        {
            try
            {
                var allUsers = ReadAllUsers();
                if (allUsers == null || allUsers.Count == 0)
                {
                    MessageBox.Show("Нет доступных аккаунтов для отображения.");
                    return;
                }

                List<string> numCards = new List<string>();

                foreach (var user in allUsers)
                {
                    foreach (var account in user.Accounts)
                    {
                        numCards.Add(account.AccountNumber);
                    }
                }

                numCards = numCards.Distinct().ToList();

                cardListWithdraw.ItemsSource = numCards;
                cardListDeposit.ItemsSource = numCards;
                cardListFrom.ItemsSource = numCards;
                cardListTo.ItemsSource = numCards;

                cardListWithdraw.SelectedIndex = 0;
                cardListDeposit.SelectedIndex = 0;
                cardListFrom.SelectedIndex = 0;
                cardListTo.SelectedIndex = 1;

                formMoveMoney.SelectedIndex = 0;


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке списка номеров карт: {ex.Message}");
            }
        }






        private void formMoveMoney_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
          

            string text = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content as string;


            switch (text)
            {
                case "Начислить":
                    depositMoney.Visibility = Visibility.Visible;
                    depositUserInfoStack.Visibility = Visibility.Visible;
                    withdrawMoney.Visibility = Visibility.Hidden;
                    withdrawUserInfo.Visibility = Visibility.Hidden;
                    transferInfo.Visibility = Visibility.Hidden;
                    transferFromTo.Visibility = Visibility.Hidden;
                    break;

                case "Снять":
                    depositMoney.Visibility = Visibility.Hidden;
                    depositUserInfoStack.Visibility = Visibility.Hidden;
                    withdrawMoney.Visibility = Visibility.Visible;
                    withdrawUserInfo.Visibility = Visibility.Visible;
                    transferInfo.Visibility = Visibility.Hidden;
                    transferFromTo.Visibility = Visibility.Hidden;
                    break;

                case "Перевести":
                    depositMoney.Visibility = Visibility.Hidden;
                    depositUserInfoStack.Visibility = Visibility.Hidden;
                    withdrawMoney.Visibility = Visibility.Hidden;
                    withdrawUserInfo.Visibility = Visibility.Hidden;
                    transferInfo.Visibility = Visibility.Visible;
                    transferFromTo.Visibility = Visibility.Visible;
                    break;
            }
        }



        //Add user
        static void SaveClient(Client client)
        {
            List<Client> allCurrentUsers = ReadAllUsers();
            allCurrentUsers.Add(client);

            string serializedUsers = JsonConvert.SerializeObject(allCurrentUsers, Formatting.Indented);
            File.WriteAllText(filePath, serializedUsers);
        }

        //Read users
        static List<Client> ReadAllUsers()
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new InvalidOperationException("Путь к файлу не установлен.");
            }

            if (!File.Exists(filePath))
            {
                return new List<Client>();
            }

            string json = File.ReadAllText(filePath);
            List<Client> currentUsers = JsonConvert.DeserializeObject<List<Client>>(json);
            return currentUsers ?? new List<Client>();
        }


        //Update data Json
        private void updateJs(List<Client> allUsers)
        {
            string updatedJson = JsonConvert.SerializeObject(allUsers, Formatting.Indented);
            File.WriteAllText(filePath, updatedJson);
        }


        //Move money
        //Add money
        private void SendMoney_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string selectedAccountNumberFrom = cardListFrom.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedAccountNumberFrom))
                {
                    MessageBox.Show("Пожалуйста, выберите номер счета, с которого совершается перевод.");
                    return;
                }

                string selectedAccountNumberTo = cardListTo.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedAccountNumberTo))
                {
                    MessageBox.Show("Пожалуйста, выберите номер счета, на который совершается перевод.");
                    return;
                }

                if (!decimal.TryParse(inputDepositMoney.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Пожалуйста, введите корректную сумму для перевода.");
                    return;
                }

                var allUsers = ReadAllUsers();
                bool fromAccountFound = false;
                bool toAccountFound = false;
                BankAccount fromAccount = null;
                BankAccount toAccount = null;
                Client fromUser = null;
                Client toUser = null;

                foreach (var user in allUsers)
                {
                    foreach (var account in user.Accounts)
                    {
                        if (account.AccountNumber == selectedAccountNumberFrom)
                        {
                            fromAccount = account;
                            fromUser = user;
                            fromAccountFound = true;
                            break;
                        }
                    }
                    if (fromAccountFound) break;
                }

                foreach (var user in allUsers)
                {
                    foreach (var account in user.Accounts)
                    {
                        if (account.AccountNumber == selectedAccountNumberTo)
                        {
                            toAccount = account;
                            toUser = user;
                            toAccountFound = true;
                            break;
                        }
                    }
                    if (toAccountFound) break;
                }

                if (!fromAccountFound || !toAccountFound)
                {
                    MessageBox.Show("Один или оба счета не найдены.");
                    return;
                }

                // Проверка достаточности средств на счете отправителя
                if (fromAccount.Balance < amount)
                {
                    MessageBox.Show("На счете отправителя недостаточно средств.");
                    return;
                }

                fromAccount.Balance -= amount;
                toAccount.Balance += amount;

                UserInfo1.Text = $"Клиент:\n{fromUser.Surname} {fromUser.Name} {fromUser.Patronymic}\n" +
                                 $"Паспортные данные: {fromUser.PassportNumber}\nДата рождения: {fromUser.DateOfBirth}\n" +
                                 $"Баланс: {fromAccount.Balance}\nСчёт: {fromAccount.AccountNumber}";

                UserInfo2.Text = $"Клиент:\n{toUser.Surname} {toUser.Name} {toUser.Patronymic}\n" +
                                 $"Паспортные данные: {toUser.PassportNumber}\nДата рождения: {toUser.DateOfBirth}\n" +
                                 $"Баланс: {toAccount.Balance}\nСчёт: {toAccount.AccountNumber}";

                MessageBox.Show($"Перевод успешно выполнен!\nСо счета {fromAccount.AccountNumber} на счет {toAccount.AccountNumber}.");

                updateJs(allUsers);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }





        //Deposit
        private void Deposited_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string selectedAccountNumber = cardListDeposit.SelectedItem.ToString();

                if (string.IsNullOrEmpty(selectedAccountNumber))
                {
                    MessageBox.Show("Пожалуйста, выберите номер счета для пополнения.");
                    return;
                }

                if (!decimal.TryParse(UserDeposit.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Пожалуйста, введите корректную сумму для пополнения.");
                    return;
                }


                var allUsers = ReadAllUsers();

                bool accountFound = false;

                foreach (var user in allUsers)
                {
                    foreach (var account in user.Accounts)
                    {
                        if (account.AccountNumber == selectedAccountNumber)
                        {

                            account.Balance += amount;
                            accountFound = true;

                            UserInfoDeposit.Text = $"Клиент:\n{user.Surname} {user.Name} {user.Patronymic}\nПаспортные данные: {user.PassportNumber}\nДата рождения: {user.DateOfBirth}\nБаланс: {account.Balance}\nСрок счёта: {account.DepositTermInMonths} месяцев\nСчёт: {account.AccountNumber}";

                            MessageBox.Show($"Счет {selectedAccountNumber} успешно пополнен на {amount} руб.\nТекущий баланс: {account.Balance} руб.");
                            break;
                        }
                    }
                    if (accountFound) break;
                }

                if (!accountFound)
                {
                    MessageBox.Show("Счет не найден.");
                    return;
                }

                updateJs(allUsers);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }

        ////Withdraw
        private void Withdraw_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string selectedWithdrawNubmer = cardListWithdraw.SelectedItem.ToString();
                if (string.IsNullOrEmpty(selectedWithdrawNubmer))
                {
                    MessageBox.Show("Пожжалуйста выберите счёт для снятия средств.");
                }

                if (!decimal.TryParse(inputWithdrawMoney.Text, out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Пожалуйста, введите корректную сумму для снятия.");
                    return;
                }

                var allUsers = ReadAllUsers();
                bool accountFound = false;

                foreach (var user in allUsers)
                {
                    foreach (var account in user.Accounts)
                    {
                        if (account.AccountNumber == selectedWithdrawNubmer)
                        {

                            account.Balance -= amount;
                            accountFound = true;

                            UserInfoWithdraw.Text = $"Клиент:\n{user.Surname} {user.Name} {user.Patronymic}\nПаспортные данные: {user.PassportNumber}\nДата рождения: {user.DateOfBirth}\nБаланс: {account.Balance}\nСрок счёта: {account.DepositTermInMonths} месяцев\nСчёт: {account.AccountNumber}";

                            MessageBox.Show($"Со счета {selectedWithdrawNubmer} была успешно снята сумма на {amount} руб.\nТекущий баланс: {account.Balance} руб.");
                            break;
                        }
                    }
                    if (accountFound) break;
                }
                if (!accountFound)
                {
                    MessageBox.Show("Счет не найден.");
                    return;
                }

                updateJs(allUsers);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
        }

        //Generate random number for account
        static public string GenerateAccountNumber()
        {
            return $"FOX-{DateTime.Now.Ticks}";
        }

        private void Add_User(object sender, RoutedEventArgs e)
        {
            string surname = userFamily.Text;
            string name = userName.Text;
            string patronymic = userMiddleName.Text;
            string passportNumber = userNumberPass.Text;
            DateTime dateOfBirth = datePicker1.SelectedDate.Value.Date;

            Client newClient = new Client(surname, name, patronymic, passportNumber, dateOfBirth);

            string accountNumber = GenerateAccountNumber();
            decimal initialBalance = 0.0m;
            int depositTermInMonths = 12;

            BankAccount newAccount = new BankAccount(accountNumber, passportNumber, initialBalance, depositTermInMonths);


            newClient.Accounts.Add(newAccount);

            // Сохраняем клиента
            SaveClient(newClient);

            Clear_Data();
            InfoAddUser.Text = $"Клиент добавлен:\n{newClient.Surname} {newClient.Name} {newClient.Patronymic},\nПаспортные данные: {newClient.PassportNumber}" +
                $"\nДата рождения: {newClient.DateOfBirth}\nБаланс: {newAccount.Balance}\nСрок счёта: {newAccount.DepositTermInMonths} месяцев \nСчёт: {newAccount.AccountNumber}";
            MessageBox.Show("Клиент успешно добавлен.");
            ListNumCards();

        }



        //combo boxes cardList
        private void cardListWithdraw_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedAccountNumber = cardListWithdraw.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedAccountNumber)) return;

            var allUsers = ReadAllUsers();
            foreach (var user in allUsers)
            {
                foreach (var account in user.Accounts)
                {
                    if (account.AccountNumber == selectedAccountNumber)
                    {
                        UserInfoWithdraw.Text = $"Клиент:\n{user.Surname} {user.Name} {user.Patronymic}\nПаспортные данные: {user.PassportNumber}\nДата рождения: {user.DateOfBirth}\nБаланс: {account.Balance}\nСрок счёта: {account.DepositTermInMonths} месяцев\nСчёт: {account.AccountNumber}";                     
                    }
                }
            }
        }

        private void cardListDeposit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedAccountNumber = cardListDeposit.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedAccountNumber)) return;

            var allUsers = ReadAllUsers();
            foreach (var user in allUsers)
            {
                foreach (var account in user.Accounts)
                {
                    if (account.AccountNumber == selectedAccountNumber)
                    {
                        UserInfoDeposit.Text = $"Клиент:\n{user.Surname} {user.Name} {user.Patronymic}\nПаспортные данные: {user.PassportNumber}\nДата рождения: {user.DateOfBirth}\nБаланс: {account.Balance}\nСрок счёта: {account.DepositTermInMonths} месяцев\nСчёт: {account.AccountNumber}";
                    }
                }
            }
        }

        private void cardListFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedAccountNumber = cardListFrom.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedAccountNumber)) return;

            var allUsers = ReadAllUsers();
            foreach (var user in allUsers)
            {
                foreach (var account in user.Accounts)
                {
                    if (account.AccountNumber == selectedAccountNumber)
                    {
                        UserInfo1.Text = $"Клиент:\n{user.Surname} {user.Name} {user.Patronymic}\nПаспортные данные: {user.PassportNumber}\nДата рождения: {user.DateOfBirth}\nБаланс: {account.Balance}\nСрок счёта: {account.DepositTermInMonths} месяцев\nСчёт: {account.AccountNumber}";
                    }
                }
            }
        }
        private void cardListTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedAccountNumber = cardListTo.SelectedItem as string;
            if (string.IsNullOrEmpty(selectedAccountNumber)) return;

            var allUsers = ReadAllUsers();
            foreach (var user in allUsers)
            {
                foreach (var account in user.Accounts)
                {
                    if (account.AccountNumber == selectedAccountNumber)
                    {
                        UserInfo2.Text = $"Клиент:\n{user.Surname} {user.Name} {user.Patronymic}\nПаспортные данные: {user.PassportNumber}\nДата рождения: {user.DateOfBirth}\nБаланс: {account.Balance}\nСрок счёта: {account.DepositTermInMonths} месяцев\nСчёт: {account.AccountNumber}";
                    }
                }
            }
        }

     
    }
}
