using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KursClient.ASP;
using KursClient.WCF;
using System.ServiceModel;
using System.Text.RegularExpressions;


namespace KursClient
{
    public partial class Form1 : Form
    {
        public static Form1 myForm = null;

        static ServiceCallback service = new ServiceCallback();
        iWCFClient WCF_service = new iWCFClient(new InstanceContext(service));
        KursClient.ASP.ASPSoapClient ASP_service;

        bool fl_add = false; //флаг добавления новой строки в таблицу (!)
        bool fl_ch = false; //флаг, сигнализирующий о завершении загрузки или обновления (!) таблиц
        bool fl_err = false; //флаг ошибки корректности введенных данных

        string update_table_name = "";
        DataSet update_table = new DataSet();

        string str = "";
        //int exc_kol = 0;

        List<DataGridView> dgv = new List<DataGridView>();
        List<Button> butt = new List<Button>();

        String[] Name_Table = { "tKlienty", "tRashodTTN", "tRabotniki", "tKategorii_tovara", "tEdIzmerenija", 
                                  "tProizvoditeli", "tPostavschiki", "tTovary", "tPrihodTovara", "tRashodTovara", 
                                    "tPrihodTTN" };

        public Form1()
        {
            InitializeComponent();
            myForm = this;

            //Формирование List "dgv" - списка всех таблиц
            dgv.Add(dgvTov); dgv.Add(dgvKat); dgv.Add(dgvEdIzm); dgv.Add(dgvProizv);
            dgv.Add(dgvRab); dgv.Add(dgvKlienty); dgv.Add(dgvPost);
            dgv.Add(dgvRashod); dgv.Add(dgvRashodTTN); dgv.Add(dgvPrihod); dgv.Add(dgvPrihodTTN);
            //Формирование List "butt" - списка всех кнопок главной формы
            butt.Add(buttSearch1); butt.Add(buttRabNow); butt.Add(buttRashTTN); butt.Add(buttPrihTTN);

            tableLoad();//загрузка таблиц для каждой из вкладок (tab1, tab2, tab3)
            tableFormat();//форматирование таблиц для каждой из вкладок (tab1, tab2, tab3)

            func_min();//предупреждение о необходимости дозаказа товара
            func_max();//предупреждение об избытке товара

            WCF_service.NewClient();

        }

        
        //загрузка таблиц
        public void tableLoad()
        {
            ASP_service = new ASPSoapClient();
            
            #region tab1_load
            //..........................tab1...........................//
            
            //получение таблиц из базы данных
            DataSet dt3 = ASP_service.Get_Table(Name_Table[3]);//tKategorii_tovara
            dgvKat.DataSource = dt3.Tables[0];
            DataSet dt4 = ASP_service.Get_Table(Name_Table[4]);//tEdIzmerenija
            dgvEdIzm.DataSource = dt4.Tables[0];
            DataSet dt5 = ASP_service.Get_Table(Name_Table[5]);//tProizvoditeli
            dgvProizv.DataSource = dt5.Tables[0];
            DataSet dt7 = ASP_service.Get_Table(Name_Table[7]);//tTovary
            dgvTov.DataSource = dt7.Tables[0];

            //создание выпадающих списков по полям id
            DataGridViewComboBoxColumn cm1 = new DataGridViewComboBoxColumn();
            DataGridViewComboBoxColumn cm2 = new DataGridViewComboBoxColumn();
            DataGridViewComboBoxColumn cm3 = new DataGridViewComboBoxColumn();

            cm1.Name = "tov_kat_name";
            cm1.ValueType = typeof(String);
            cm1.DataSource = dt3.Tables[0];
            cm1.ValueMember = "kat_id";
            cm1.DisplayMember = "kat_name";
            cm1.DataPropertyName = "kat_id";
            cm1.FlatStyle = FlatStyle.Flat;
            dgvTov.Columns.Add(cm1);//
            dgvTov.Columns["tov_kat_name"].DisplayIndex = 2;

            cm2.Name = "tov_edIzm_nameS";
            cm2.ValueType = typeof(String);
            cm2.DataSource = dt4.Tables[0];
            cm2.ValueMember = "edIzm_id";
            cm2.DisplayMember = "oboznachenie";
            cm2.DataPropertyName = "edIzm_id";
            cm2.FlatStyle = FlatStyle.Flat;
            dgvTov.Columns.Add(cm2);//
            dgvTov.Columns["tov_edIzm_nameS"].DisplayIndex = 3;

            cm3.Name = "tov_proizv_name";
            cm3.ValueType = typeof(String);
            cm3.DataSource = dt5.Tables[0];
            cm3.ValueMember = "proizv_id";
            cm3.DisplayMember = "proizv_name";
            cm3.DataPropertyName = "proizv_id";
            cm3.FlatStyle = FlatStyle.Flat;
            dgvTov.Columns.Add(cm3);//
            dgvTov.Columns["tov_proizv_name"].DisplayIndex = 4;
            
            #endregion //tab1 _load

            #region tab2_load
            //..........................tab2...........................//
            
            //получение таблиц из базы данных
            DataSet dt0 = ASP_service.Get_Table(Name_Table[0]);//tKlienty
            dgvKlienty.DataSource = dt0.Tables[0];
            DataSet dt2 = ASP_service.Get_Table(Name_Table[2]);//tRabotniki
            dgvRab.DataSource = dt2.Tables[0];
            DataSet dt6 = ASP_service.Get_Table(Name_Table[6]);//tPostavschiki
            dgvPost.DataSource = dt6.Tables[0];

            #endregion //tab2_load

            #region tab3_load
            //..........................tab3...........................//

            //получение таблиц из базы данных
            DataSet dt1 = ASP_service.Get_Table(Name_Table[1]);//tRashodTTN
            dgvRashodTTN.DataSource = dt1.Tables[0];
            DataSet dt8 = ASP_service.Get_Table(Name_Table[8]);//tPrihodTovara
            dgvPrihod.DataSource = dt8.Tables[0];
            DataSet dt9 = ASP_service.Get_Table(Name_Table[9]);//tRashodTovara
            dgvRashod.DataSource = dt9.Tables[0];
            DataSet dt10 = ASP_service.Get_Table(Name_Table[10]);//tPrihodTTN
            dgvPrihodTTN.DataSource = dt10.Tables[0];

            //создание выпадающих списков по полям id:
            //РасходТТН
            DataGridViewComboBoxColumn cm4 = new DataGridViewComboBoxColumn();
            DataGridViewComboBoxColumn cm5 = new DataGridViewComboBoxColumn();

            cm4.Name = "firm_pol";
            cm4.ValueType = typeof(String);
            cm4.DataSource = dt0.Tables[0];
            cm4.ValueMember = "klient_id";
            cm4.DisplayMember = "kl_firm_name";
            cm4.DataPropertyName = "klient_id";
            cm4.FlatStyle = FlatStyle.Flat;
            dgvRashodTTN.Columns.Add(cm4);//
            dgvRashodTTN.Columns["firm_pol"].DisplayIndex = 2;

            cm5.Name = "rash_FIO_otpr";
            cm5.ValueType = typeof(String);
            cm5.DataSource = dt2.Tables[0];
            cm5.ValueMember = "rab_id";
            cm5.DisplayMember = "rab_name";
            cm5.DataPropertyName = "rab_id";
            cm5.FlatStyle = FlatStyle.Flat;
            dgvRashodTTN.Columns.Add(cm5);//
            dgvRashodTTN.Columns["rash_FIO_otpr"].DisplayIndex = 4;

            //ПриходТТН
            DataGridViewComboBoxColumn cm6 = new DataGridViewComboBoxColumn();
            DataGridViewComboBoxColumn cm7 = new DataGridViewComboBoxColumn();

            cm6.Name = "firm_otpr";
            cm6.ValueType = typeof(String);
            cm6.DataSource = dt6.Tables[0];
            cm6.ValueMember = "post_id";
            cm6.DisplayMember = "p_firm_name";
            cm6.DataPropertyName = "post_id";
            cm6.FlatStyle = FlatStyle.Flat;
            dgvPrihodTTN.Columns.Add(cm6);//
            dgvPrihodTTN.Columns["firm_otpr"].DisplayIndex = 2;

            cm7.Name = "prih_FIO_pol";
            cm7.ValueType = typeof(String);
            cm7.DataSource = dt2.Tables[0];
            cm7.ValueMember = "rab_id";
            cm7.DisplayMember = "rab_name";
            cm7.DataPropertyName = "rab_id";
            cm7.FlatStyle = FlatStyle.Flat;
            dgvPrihodTTN.Columns.Add(cm7);//
            dgvPrihodTTN.Columns["prih_FIO_pol"].DisplayIndex = 4;

            //Расход товаров (продажи)
            DataGridViewComboBoxColumn cm8 = new DataGridViewComboBoxColumn();
            DataGridViewComboBoxColumn cm9 = new DataGridViewComboBoxColumn();

            cm8.Name = "rash_TTN_num";
            cm8.ValueType = typeof(String);
            cm8.DataSource = dt1.Tables[0];
            cm8.ValueMember = "rashTTN_id";
            cm8.DisplayMember = "rashTTN_number";
            cm8.DataPropertyName = "rashTTN_id";
            cm8.FlatStyle = FlatStyle.Flat;
            dgvRashod.Columns.Add(cm8);//
            dgvRashod.Columns["rash_TTN_num"].DisplayIndex = 1;

            cm9.Name = "rash_tov_name";
            cm9.ValueType = typeof(String);
            cm9.DataSource = dt7.Tables[0];
            cm9.ValueMember = "tov_id";
            cm9.DisplayMember = "tov_name";
            cm9.DataPropertyName = "tov_id";
            cm9.FlatStyle = FlatStyle.Flat;
            dgvRashod.Columns.Add(cm9);//
            dgvRashod.Columns["rash_tov_name"].DisplayIndex = 2;

            //Приход товаров (поставки)
            DataGridViewComboBoxColumn cm10 = new DataGridViewComboBoxColumn();
            DataGridViewComboBoxColumn cm11 = new DataGridViewComboBoxColumn();

            cm10.Name = "prih_TTN_num";
            cm10.ValueType = typeof(String);
            cm10.DataSource = dt10.Tables[0];
            cm10.ValueMember = "prihTTN_id";
            cm10.DisplayMember = "prihTTN_number";
            cm10.DataPropertyName = "prihTTN_id";
            cm10.FlatStyle = FlatStyle.Flat;
            dgvPrihod.Columns.Add(cm10);//
            dgvPrihod.Columns["prih_TTN_num"].DisplayIndex = 1;

            cm11.Name = "prih_tov_name";
            cm11.ValueType = typeof(String);
            cm11.DataSource = dt7.Tables[0];
            cm11.ValueMember = "tov_id";
            cm11.DisplayMember = "tov_name";
            cm11.DataPropertyName = "tov_id";
            cm11.FlatStyle = FlatStyle.Flat;
            dgvPrihod.Columns.Add(cm11);//
            dgvPrihod.Columns["prih_tov_name"].DisplayIndex = 2;

            #endregion //tab3_load

            ASP_service.Close();
        }

        //форматирование таблиц
        public void tableFormat()
        {
            #region tab1 _format 
            //////////////////////////////tab1///////////////////////////
            //сокрытие полей id
            //товары
            dgvTov.Columns["tov_id"].Visible = false;
            dgvTov.Columns["kat_id"].Visible = false;
            dgvTov.Columns["edIzm_id"].Visible = false;
            dgvTov.Columns["proizv_id"].Visible = false;
            //категории
            dgvKat.Columns["kat_id"].Visible = false;
            //единицы измерения
            dgvEdIzm.Columns["edIzm_id"].Visible = false;
            //производители
            dgvProizv.Columns["proizv_id"].Visible = false;
            
            //заголовки столбцов таблицы:
            //товары
            dgvTov.Columns["tov_name"].HeaderText = "Наименование товара";
            dgvTov.Columns["tov_kat_name"].HeaderText = "Категория товара";
            dgvTov.Columns["tov_edIzm_nameS"].HeaderText = "Единицы измерения";
            dgvTov.Columns["tov_proizv_name"].HeaderText = "Производитель";
            dgvTov.Columns["country"].HeaderText = "Страна происхождения";
            dgvTov.Columns["kol"].HeaderText = "Количество";
            dgvTov.Columns["min"].HeaderText = "Min";
            dgvTov.Columns["max"].HeaderText = "Max";
            //категории
            dgvKat.Columns["kat_name"].HeaderText = "Категория товара";
            //единицы измерения
            dgvEdIzm.Columns["edIzm_name"].HeaderText = "Единицы измерения";
            dgvEdIzm.Columns["oboznachenie"].HeaderText = "Обозначение";
            //производители
            dgvProizv.Columns["proizv_name"].HeaderText = "Производитель";
            
            //ширина столбцов таблицы не по умолчанию:
            //товары
            dgvTov.Columns["tov_name"].Width = 170;
            dgvTov.Columns["kol"].Width = 75;
            dgvTov.Columns["min"].Width = 40;
            dgvTov.Columns["max"].Width = 40;
            //категории
            dgvKat.Columns["kat_name"].Width = 152;
            //единицы измерения
            dgvEdIzm.Columns["edIzm_name"].Width = 160;
            dgvEdIzm.Columns["oboznachenie"].Width = 92;
            //производители
            dgvProizv.Columns["proizv_name"].Width = 152;

            //отключение возможности автоматический сортировки столбцов
            foreach (DataGridViewColumn col in dgvTov.Columns)
            {
                dgvTov.Columns[col.Name.ToString()].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            foreach (DataGridViewColumn col in dgvEdIzm.Columns)
            {
                dgvEdIzm.Columns[col.Name.ToString()].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            foreach (DataGridViewColumn col in dgvKat.Columns)
            {
                dgvKat.Columns[col.Name.ToString()].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            foreach (DataGridViewColumn col in dgvProizv.Columns)
            {
                dgvProizv.Columns[col.Name.ToString()].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            #endregion //tab1_format

            #region tab2_format
            //////////////////////////////tab2///////////////////////////
            //сокрытие полей id:
            //работники
            dgvRab.Columns["rab_id"].Visible = false;
            //клиенты
            dgvKlienty.Columns["klient_id"].Visible = false;
            //поставщики
            dgvPost.Columns["post_id"].Visible = false;

            //заголовки столбцов таблицы:
            //работники
            dgvRab.Columns["rab_name"].HeaderText = "ФИО работника";
            dgvRab.Columns["position"].HeaderText = "Должность";
            dgvRab.Columns["date_of_birth"].HeaderText = "Дата рождения";
            dgvRab.Columns["telefon"].HeaderText = "Телефон";
            dgvRab.Columns["passport"].HeaderText = "Паспорт";
            dgvRab.Columns["home_address"].HeaderText = "Домашний адрес";
            dgvRab.Columns["data_priema"].HeaderText = "Дата приема на работу";
            dgvRab.Columns["data_uvolnenia"].HeaderText = "Дата увольнения";
            //клиенты
            dgvKlienty.Columns["kl_firm_name"].HeaderText = "Название фирмы-клиента";
            dgvKlienty.Columns["kontaktFIO"].HeaderText = "Контактное лицо (ФИО)";
            dgvKlienty.Columns["position"].HeaderText = "Должность";
            dgvKlienty.Columns["telefon"].HeaderText = "Телефон";
            dgvKlienty.Columns["email"].HeaderText = "Email";
            dgvKlienty.Columns["address"].HeaderText = "Адрес";
            dgvKlienty.Columns["EGRPOU_code"].HeaderText = "Код ЕГРПОУ";
            //поставщики
            dgvPost.Columns["p_firm_name"].HeaderText = "Название фирмы-поставщика";
            dgvPost.Columns["kontaktFIO"].HeaderText = "Контактное лицо (ФИО)";
            dgvPost.Columns["position"].HeaderText = "Должность";
            dgvPost.Columns["telefon"].HeaderText = "Телефон";
            dgvPost.Columns["email"].HeaderText = "Email";
            dgvPost.Columns["address"].HeaderText = "Адрес";
            dgvPost.Columns["EGRPOU_code"].HeaderText = "Код ЕГРПОУ";

            //ширина столбцов таблицы не по умолчанию:
            //работники
            dgvRab.Columns["rab_name"].Width = 155;
            dgvRab.Columns["position"].Width = 80;
            dgvRab.Columns["date_of_birth"].Width = 80;
            dgvRab.Columns["passport"].Width = 80;
            dgvRab.Columns["data_priema"].Width = 72;
            dgvRab.Columns["data_uvolnenia"].Width = 72;
            //клиенты
            dgvKlienty.Columns["kl_firm_name"].Width = 110;
            dgvKlienty.Columns["kontaktFIO"].Width = 150;
            dgvKlienty.Columns["EGRPOU_code"].Width = 80;
            //поставщики
            dgvPost.Columns["p_firm_name"].Width = 110;
            dgvPost.Columns["kontaktFIO"].Width = 150;
            dgvPost.Columns["EGRPOU_code"].Width = 80;

            //отключение возможности автоматический сортировки столбцов
            foreach (DataGridViewColumn col in dgvRab.Columns)
            {
                dgvRab.Columns[col.Name.ToString()].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            foreach (DataGridViewColumn col in dgvKlienty.Columns)
            {
                dgvKlienty.Columns[col.Name.ToString()].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            foreach (DataGridViewColumn col in dgvPost.Columns)
            {
                dgvPost.Columns[col.Name.ToString()].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            #endregion tab2_format

            #region tab3_format
            //////////////////////////////tab3///////////////////////////
            //сокрытие полей id
            //РасходТТН
            dgvRashodTTN.Columns["rashTTN_id"].Visible = false;
            dgvRashodTTN.Columns["klient_id"].Visible = false;
            dgvRashodTTN.Columns["rab_id"].Visible = false;
            //ПриходТТН
            dgvPrihodTTN.Columns["prihTTN_id"].Visible = false;
            dgvPrihodTTN.Columns["post_id"].Visible = false;
            dgvPrihodTTN.Columns["rab_id"].Visible = false;
            //Расход товара (продажи)
            dgvRashod.Columns["rash_id"].Visible = false;
            dgvRashod.Columns["rashTTN_id"].Visible = false;
            dgvRashod.Columns["tov_id"].Visible = false;
            //Приход товара (поставки)
            dgvPrihod.Columns["prih_id"].Visible = false;
            dgvPrihod.Columns["prihTTN_id"].Visible = false;
            dgvPrihod.Columns["tov_id"].Visible = false;

            //заголовки столбцов таблицы:
            //РасходТТН
            dgvRashodTTN.Columns["rashTTN_number"].HeaderText = "Номер ТТН (расход)";
            dgvRashodTTN.Columns["firm_pol"].HeaderText = "Фирма-получатель";
            dgvRashodTTN.Columns["FIO_pol"].HeaderText = "ФИО получателя";
            dgvRashodTTN.Columns["rash_FIO_otpr"].HeaderText = "ФИО отправителя";
            dgvRashodTTN.Columns["data_otpr"].HeaderText = "Дата отправления";
            //ПриходТТН
            dgvPrihodTTN.Columns["prihTTN_number"].HeaderText = "Номер ТТН (приход)";
            dgvPrihodTTN.Columns["firm_otpr"].HeaderText = "Фирма-отправитель";
            dgvPrihodTTN.Columns["FIO_otpr"].HeaderText = "ФИО отправителя";
            dgvPrihodTTN.Columns["prih_FIO_pol"].HeaderText = "ФИО получателя";
            dgvPrihodTTN.Columns["data_pol"].HeaderText = "Дата получения";
            //Расход товара (продажи)
            dgvRashod.Columns["rash_TTN_num"].HeaderText = "Номер ТТН расхода";
            dgvRashod.Columns["rash_tov_name"].HeaderText = "Наименование товара";
            dgvRashod.Columns["rash_kol"].HeaderText = "Кол-во";
            dgvRashod.Columns["price_for_one_sell"].HeaderText = "Цена за единицу";
            //Приход товара (поставки)
            dgvPrihod.Columns["prih_TTN_num"].HeaderText = "Номер ТТН прихода";
            dgvPrihod.Columns["prih_tov_name"].HeaderText = "Наименование товара";
            dgvPrihod.Columns["prih_kol"].HeaderText = "Кол-во";
            dgvPrihod.Columns["price_for_one_buy"].HeaderText = "Цена за единицу";

            //ширина столбцов таблицы не по умолчанию:
            //РасходТТН
            dgvRashodTTN.Columns["rashTTN_number"].Width = 120;
            dgvRashodTTN.Columns["firm_pol"].Width = 120;
            dgvRashodTTN.Columns["FIO_pol"].Width = 130;
            dgvRashodTTN.Columns["rash_FIO_otpr"].Width = 130;
            //ПриходТТН
            dgvPrihodTTN.Columns["prihTTN_number"].Width = 120;
            dgvPrihodTTN.Columns["firm_otpr"].Width = 120;
            dgvPrihodTTN.Columns["FIO_otpr"].Width = 130;
            dgvPrihodTTN.Columns["prih_FIO_pol"].Width = 130;
            //Расход товара (продажи)
            dgvRashod.Columns["rash_tov_name"].Width = 120;
            dgvRashod.Columns["rash_kol"].Width = 70;
            dgvRashod.Columns["price_for_one_sell"].Width = 70;
            //Приход товара (поставки)
            dgvPrihod.Columns["prih_tov_name"].Width = 120;
            dgvPrihod.Columns["prih_kol"].Width = 70;
            dgvPrihod.Columns["price_for_one_buy"].Width = 70;

            //отключение возможности автоматический сортировки столбцов
            foreach (DataGridViewColumn col in dgvRashodTTN.Columns)
            {
                dgvRashodTTN.Columns[col.Name.ToString()].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            foreach (DataGridViewColumn col in dgvPrihodTTN.Columns)
            {
                dgvPrihodTTN.Columns[col.Name.ToString()].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            foreach (DataGridViewColumn col in dgvRashod.Columns)
            {
                dgvRashod.Columns[col.Name.ToString()].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            foreach (DataGridViewColumn col in dgvPrihod.Columns)
            {
                dgvPrihod.Columns[col.Name.ToString()].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            #endregion //tab3_format

            fl_ch = true; //флаг, сигнализирующий о завершении загрузки или обновления (!) таблиц
        }

        #region ASP_Functions(Buttons)

        //кнопка открытия окна поиска по категории
        private void buttSearch1_Click(object sender, EventArgs e)
        {
            Form2 SearchForm = new Form2();
            SearchForm.Owner = this;
            SearchForm.Show();
        }

        //кнопка открытия окна перечня актуальных работников компании
        private void buttRabNow_Click(object sender, EventArgs e)
        {
            Form3 RabNowForm = new Form3();
            RabNowForm.Owner = this;
            RabNowForm.Show();
        }
        
        //кнопка отчета по выбранной ТТН расхода
        private void buttRashTTN_Click(object sender, EventArgs e)
        {
            Form4 dvTovForm = new Form4();
            dvTovForm.Owner = this;
            dvTovForm.Show();
        }
        //кнопка отчета по выбранной ТТН прихода
        private void buttPrihTTN_Click(object sender, EventArgs e)
        {
            Form5 dvTovForm = new Form5();
            dvTovForm.Owner = this;
            dvTovForm.Show();
        }

        #endregion //ASP_Functions(Buttons)

        public void func_min()  //предупреждение о необходимости дозаказа товара
        {
            string[][] wcf1 = new string[][] { };
            wcf1 = WCF_service.Get_Tovar_dozakaz();

            if (wcf1 != null)
            {
                foreach (string[] i in wcf1)
                {//вывод соответствующего оповещения
                    textMessage.SelectionColor = Color.Firebrick;
                    textMessage.SelectedText = "Товар '" + i[0] + "' (производитель: " + i[1] + ") заканчивается! Необходим дозаказ!" + "\n";
                }
            }
        }

        public void func_max() //предупреждение об избытке товара
        {
            string[][] wcf2 = new string[][] { };
            wcf2 = WCF_service.Get_Tovar_izbytok();

            if (wcf2 != null)
            {
                foreach (string[] i in wcf2)
                {//вывод соответствующего оповещения
                    textMessage.SelectionColor = Color.Green;
                    textMessage.SelectedText = "Товар '" + i[0] + "' (производитель: " + i[1] + ") в избытке!" + "\n";
                }
            }
        }



        #region Callback
        //Выполнение обновления таблиц

        class ServiceCallback : iWCFCallback
        {
            public void NewMessage(string message) { }
            public void Update_client(string table_name, DataSet table)
            {
                myForm.Pre_Callback(table_name, table);
            }
        }

        public void Pre_Callback(string table_name, DataSet table)
        {
            update_table_name = table_name;
            update_table = table;
            if (this.InvokeRequired)
                this.Invoke(new Action(Callback));
            else
                Callback();
        }
        
        public void Callback()
        {
            switch (update_table_name)
            {
                case "tTovary":
                {
                    dgvTov.DataSource = update_table.Tables[0];
                    Update_Combobox(dgvRashod, 2, "Наименование товара", "tov_id", "tov_name");
                    Update_Combobox(dgvPrihod, 2, "Наименование товара", "tov_id", "tov_name");
                    break;
                }
                case "tKategorii_tovara":
                {
                    dgvKat.DataSource = update_table.Tables[0];
                    Update_Combobox(dgvTov, 2, "Категория товара", "kat_id", "kat_name");
                    break;
                }
                case "tEdIzmerenija":
                {
                    dgvEdIzm.DataSource = update_table.Tables[0];
                    Update_Combobox(dgvTov, 3, "Единицы измерения", "edIzm_id", "oboznachenie");
                    break;
                }
                case "tProizvoditeli":
                {
                    dgvProizv.DataSource = update_table.Tables[0];
                    Update_Combobox(dgvTov, 4, "Производитель", "proizv_id", "proizv_name");
                    break;
                }
                case "tRabotniki":
                {
                    dgvRab.DataSource = update_table.Tables[0];
                    Update_Combobox(dgvRashodTTN, 4, "ФИО отправителя", "rab_id", "rab_name");
                    Update_Combobox(dgvPrihodTTN, 4, "ФИО получателя", "rab_id", "rab_name");
                    break;
                }
                case "tKlienty":
                {
                    dgvKlienty.DataSource = update_table.Tables[0];
                    Update_Combobox(dgvRashodTTN, 2, "Фирма-получатель", "klient_id", "kl_firm_name");
                    break;
                }
                case "tPostavschiki":
                {
                    dgvPost.DataSource = update_table.Tables[0];
                    Update_Combobox(dgvPrihodTTN, 2, "Фирма-отправитель", "post_id", "p_firm_name");
                    break;
                }
                case "tRashodTTN":
                {
                    dgvRashodTTN.DataSource = update_table.Tables[0];
                    Update_Combobox(dgvRashod, 1, "Номер ТТН расхода", "rashTTN_id", "rashTTN_number");
                    break;
                }
                case "tPrihodTTN":
                {
                    dgvPrihodTTN.DataSource = update_table.Tables[0];
                    Update_Combobox(dgvPrihod, 1, "Номер ТТН прихода", "prihTTN_id", "prihTTN_number");
                    break;
                }
                case "tRashodTovara":
                {
                    dgvRashod.DataSource = update_table.Tables[0];
                    break;
                }
                case "tPrihodTovara":
                {
                    dgvPrihod.DataSource = update_table.Tables[0];
                    break;
                }
            }

        }
        
        public void Update_Combobox(DataGridView dgv, int i, string a, string b, string c)
        {
            dgv.Columns.Remove(dgv.Columns[i]);
            DataGridViewComboBoxColumn cm = new DataGridViewComboBoxColumn();
            cm.Name = a;
            cm.ValueType = typeof(String);
            cm.DataSource = update_table.Tables[0];
            cm.ValueMember = b;
            cm.DisplayMember = c;
            cm.DataPropertyName = b;
            cm.FlatStyle = FlatStyle.Flat;
            dgv.Columns.Insert(i, cm);
        }

        #endregion //Callback

        //

        //удаление строк из таблицы
        private void dgvAny_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            
            DialogResult result = MessageBox.Show("Выбранная строка будет удалена из базы данных. Продолжить?", "Удаление строки из таблицы",
               MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                //выполнение удаления из базы (из таблицы - автоматически)
                string res = "";
                try
                {
                    res = WCF_service.Delete_Line(e.Row.DataGridView.Tag.ToString(), e.Row.Cells[0].Value.ToString(), e.Row.Cells[0].OwningColumn.Name.ToString());
                    
                    if (res == "Not Del")
                    {
                        MessageBox.Show("Сервер отказал в удалении выбранной строки! \nВероятнее всего, она используется в дочерней таблице.");
                        e.Cancel = true;
                    }
                    if (res == "Ok")
                    {
                        WCF_service.OnTimer(e.Row.DataGridView.Tag.ToString());
                        e.Cancel = false;
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                
                //если в таблице присутствовала несохраненная в базе данных строка
                if (fl_add == true)
                {
                    fl_add = false;

                    //разблокирование всех кнопок и таблиц
                    foreach (DataGridView d in dgv)
                    {
                        d.Enabled = true;
                    }
                    foreach (Button b in butt)
                    {
                        b.Enabled = true;
                    }
                }
            }
            else
            {
                //отмена операции удаления
                e.Cancel = true;
            }
        }

        //добавление строки в таблицу: создание новой строки
        private void dgvAny_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            fl_add = true; //флаг добавления новой строки в таблицу (!)
            e.Row.DataGridView.CurrentRow.ErrorText = "Строка стоит в очереди на добавление";

            //блокирование всех кнопок и таблиц, кроме выбранной (для добавления новой строки)
            foreach (DataGridView d in dgv)
            {
                if (d.Name.ToString() != e.Row.DataGridView.Name.ToString())
                {
                    d.Enabled = false;
                }
            }
            foreach (Button b in butt)
            {
                b.Enabled = false;
            }
        }

        //добавление строки в таблицу: переход на новую строку
        private void dgvAny_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            bool fl_ok = true;
            fl_err = false;
            //если в таблице присутствует несохраненная строка
            if (fl_add != false)
            {
                 try
                    {
                        if (((DataGridView)(this.ActiveControl)).Rows[e.RowIndex].ErrorText == "Строка стоит в очереди на добавление")
                        {
                            ((DataGridView)(this.ActiveControl)).EndEdit();//завершение редактирования текущей ячейки
                            for (int i = 1; i <= ((DataGridView)(this.ActiveControl)).DisplayedColumnCount(true); i++)
                            {
                                //проверка на наличие пустых ячеек в таблице
                                if ((((DataGridView)(this.ActiveControl))[i, e.RowIndex].Value.ToString() == "") && (((DataGridView)(this.ActiveControl)).Columns[i].Name != "data_uvolnenia"))
                                {
                                    MessageBox.Show("Строка не может быть добавлена в базу данных, пока в ней не будут заполнены все обязательные поля!");
                                    fl_ok = false;
                                    break;
                                }
                            }
                            if (fl_ok != false)
                            {
                                for (int i = 1; i <= ((DataGridView)(this.ActiveControl)).DisplayedColumnCount(true); i++)
                                {
                                    //проверка на наличие некорректно заполненных ячеек
                                    if ((((DataGridView)(this.ActiveControl))[i, e.RowIndex].ErrorText != null) && (((DataGridView)(this.ActiveControl))[i, e.RowIndex].ErrorText != ""))
                                    {
                                        MessageBox.Show("Одно или более полей в строке имеют неверный формат! \nСохранение в базе данных возможно только после редактирования соответствующих полей.");
                                        fl_err = true;
                                        break;
                                    }
                                }
                            }
                            if ((fl_ok == true) && (fl_err != true))
                            {
                                //добавление строки в базу данных
                                fl_add = false;
                                ((DataGridView)(this.ActiveControl)).Rows[e.RowIndex].ErrorText = null;
                                Add_function(((DataGridView)(this.ActiveControl)).Name.ToString());
                                MessageBox.Show("Строка добавлена в базу данных!");
                                

                                //разблокирование всех кнопок и таблиц
                                foreach (DataGridView d in dgv)
                                {
                                    d.Enabled = true;
                                }
                                foreach (Button b in butt)
                                {
                                    b.Enabled = true;
                                }
                            }
                        }
                    }
                 catch (System.Exception ex)
                 {
                     //MessageBox.Show(ex.Message);
                 }
            }
        }

        private void Add_function(string name)
        {
            foreach (DataGridView d in dgv)
            {
                if (d.Name.ToString() == name)
                {
                    for (int j = 1; j < d.Columns.Count; j++)
                    {
                        string column = "";
                        if (d == dgvTov)
                        {
                            if (d.Columns[j].Name.ToString() == "tov_kat_name")
                                column = "kat_id";
                            else if (d.Columns[j].Name.ToString() == "tov_edIzm_nameS")
                                column = "edIzm_id";
                            else if (d.Columns[j].Name.ToString() == "tov_proizv_name")
                                column = "proizv_id";
                        }
                        else if (d == dgvRashodTTN)
                        {
                            if (d.Columns[j].Name.ToString() == "firm_pol")
                                column = "klient_id";
                            else if (d.Columns[j].Name.ToString() == "rash_FIO_otpr")
                                column = "rab_id";
                        }
                        else if (d == dgvPrihodTTN)
                        {
                            if (d.Columns[j].Name.ToString() == "firm_otpr")
                                column = "post_id";
                            else if (d.Columns[j].Name.ToString() == "prih_FIO_pol")
                                column = "rab_id";
                        }
                        else if (d == dgvRashod)
                        {
                            if (d.Columns[j].Name.ToString() == "rash_TTN_num")
                                column = "rashTTN_id";
                            else if (d.Columns[j].Name.ToString() == "rash_tov_name")
                                column = "tov_id";
                        }
                        else if (d == dgvPrihod)
                        {
                            if (d.Columns[j].Name.ToString() == "prih_TTN_num")
                                column = "prihTTN_id";
                            else if (d.Columns[j].Name.ToString() == "prih_tov_name")
                                column = "tov_id";
                        }
                        else
                            column = d.Columns[j].Name.ToString();

                        WCF_service.Add_Line(d.Tag.ToString(),
                            d.CurrentRow.Cells[j].Value.ToString(),
                            //d.Columns[j].Name.ToString()
                            column
                        );
                    }
                    break;

                }
            }
            //WCF_service.Add_Line(((DataGridView)(this.ActiveControl)).Tag.ToString(),
            //    ((DataGridView)(this.ActiveControl)).CurrentRow.Cells[((DataGridView)(this.ActiveControl)).CurrentCell.ColumnIndex].Value.ToString(),
            //                         ((DataGridViewComboBoxColumn)(((DataGridView)(this.ActiveControl)).Columns[((DataGridView)(this.ActiveControl)).CurrentCell.ColumnIndex])).ValueMember.ToString());
        }

        #region Edit (ValueChanged) 
        //проверка корректности введенных данных

        private void dgvTov_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {//товары
            if (fl_ch == true) //если произошло начальное форматирование таблиц
            {
                try
                {
                    DataGridViewColumn col = dgvTov.Columns[e.ColumnIndex];
                    DataGridViewCell current = dgvTov.CurrentCell;

                    //числовые поля
                    if ((col.Name.Contains("kol")) || (col.Name.Contains("min")) || (col.Name.Contains("max")))
                    {
                        if (Convert.ToInt32(current.Value) < 0)
                        {
                            MessageBox.Show("Значение поля не может быть отрицательным! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Значение поля не может быть отрицательным!";
                        }
                        else
                        {
                            if (dgvTov.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvTov.Tag.ToString(), dgvTov.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvTov.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvTov.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvTov.Columns[0].Name.ToString());
                            current.ErrorText = null;

                            textMessage.Clear();
                            func_min();//предупреждение о необходимости дозаказа товара
                            func_max();//предупреждение об избытке товара

                        }
                    }
                    //поле с названием страны происхождения
                    else if (col.Name == "country")
                    {
                        str = current.Value.ToString();
                        Match m1 = Regex.Match(str, @"^[А-ЯЁA-Z][а-яёa-z]+$", RegexOptions.IgnoreCase);
                        Match m2 = Regex.Match(str, @"^[А-ЯЁA-Z][а-яёa-z]+ [А-ЯЁA-Z][а-яёa-z]+$", RegexOptions.IgnoreCase);
                        Match m3 = Regex.Match(str, @"^[А-ЯЁA-Z][а-яёa-z]+ [А-ЯЁA-Z][а-яёa-z]+ [А-ЯЁA-Z][а-яёa-z]+$", RegexOptions.IgnoreCase);
                        if (!m1.Success && !m2.Success && !m3.Success)
                        {
                            MessageBox.Show("Введенное название страны происхождения некорректно! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Введенное название страны происхождения некорректно!";
                        }
                        else
                        {
                            if (dgvTov.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvTov.Tag.ToString(), dgvTov.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvTov.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvTov.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvTov.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //остальные текстовые поля не могут быть пустыми
                    else
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^.+$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Поле не может быть пустым! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Поле не может быть пустым!";
                        }
                        else
                        {
                            string column = "";
                            if (dgvTov.Columns[e.ColumnIndex].Name.ToString() == "tov_kat_name")
                                column = "kat_id";
                            else if (dgvTov.Columns[e.ColumnIndex].Name.ToString() == "tov_edIzm_nameS")
                                column = "edIzm_id";
                            else if (dgvTov.Columns[e.ColumnIndex].Name.ToString() == "tov_proizv_name")
                                column = "proizv_id";
                            else
                                column = dgvTov.Columns[e.ColumnIndex].Name.ToString();
                            if (dgvTov.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvTov.Tag.ToString(), column,
                                                            dgvTov.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvTov.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvTov.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //сравнение значений полей min и max
                    if ((col.Name.Contains("min")) || (col.Name.Contains("max")))
                    {
                        if ((dgvTov[dgvTov.Columns["min"].Index, dgvTov.CurrentRow.Index].Value.ToString() != "")
                            && (dgvTov[dgvTov.Columns["max"].Index, dgvTov.CurrentRow.Index].Value.ToString() != ""))
                        {
                            if (Convert.ToInt32(dgvTov[dgvTov.Columns["min"].Index, dgvTov.CurrentRow.Index].Value)
                                >= Convert.ToInt32(dgvTov[dgvTov.Columns["max"].Index, dgvTov.CurrentRow.Index].Value))
                            {
                                MessageBox.Show("Значение Min должно быть меньше Max! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                                current.ErrorText = "Значение Min должно быть меньше Max!";
                            }
                            else
                            {
                                if (dgvTov.CurrentRow.ErrorText == "")
                                WCF_service.Ch_Line(dgvTov.Tag.ToString(), dgvTov.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvTov.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvTov.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvTov.Columns[0].Name.ToString());
                                current.ErrorText = null;
                            }
                        }
                    }

                }
                catch (System.Exception ex) { }
            }
        }

        private void dgvKat_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {//категории товаров
            if (fl_ch == true) //если произошло начальное форматирование таблиц
            {
                try
                {
                    DataGridViewColumn col = dgvKat.Columns[e.ColumnIndex];
                    DataGridViewCell current = dgvKat.CurrentCell;

                    //поле категории товара
                    str = current.Value.ToString();
                    Match m1 = Regex.Match(str, @"^[А-ЯЁA-Z][а-яёa-z]+$", RegexOptions.IgnoreCase);
                    Match m2 = Regex.Match(str, @"^[А-ЯЁA-Z][а-яёa-z]+ [А-ЯЁA-Z][а-яёa-z]+$", RegexOptions.IgnoreCase);
                    Match m3 = Regex.Match(str, @"^[А-ЯЁA-Z][а-яёa-z]+ [А-ЯЁA-Z][а-яёa-z]+ [А-ЯЁA-Z][а-яёa-z]+$", RegexOptions.IgnoreCase);
                    if (!m1.Success && !m2.Success && !m3.Success)
                    {
                        MessageBox.Show("Введенное название категории некорректно! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                        current.ErrorText = "Введенное название категории некорректно!";
                    }
                    else
                    {
                        if (dgvKat.CurrentRow.ErrorText == "")
                        WCF_service.Ch_Line(dgvKat.Tag.ToString(), dgvKat.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvKat.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvKat.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvKat.Columns[0].Name.ToString());
                        current.ErrorText = null;
                    }
                }
                catch (System.Exception ex) { }
            }
        }

        private void dgvEdIzm_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {//единицы измерения
            if (fl_ch == true) //если произошло начальное форматирование таблиц
            {
                try
                {
                    DataGridViewColumn col = dgvEdIzm.Columns[e.ColumnIndex];
                    DataGridViewCell current = dgvEdIzm.CurrentCell;

                    str = current.Value.ToString();
                    Match m = Regex.Match(str, @"^.+$", RegexOptions.IgnoreCase);
                    if (!m.Success)
                    {
                        MessageBox.Show("Поле не может быть пустым! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                        current.ErrorText = "Поле не может быть пустым!";
                    }
                    else
                    {
                        if (dgvEdIzm.CurrentRow.ErrorText == "")
                        WCF_service.Ch_Line(dgvEdIzm.Tag.ToString(), dgvEdIzm.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvEdIzm.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvEdIzm.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvEdIzm.Columns[0].Name.ToString());
                        current.ErrorText = null;
                    }
                }
                catch (System.Exception ex) { }
            }
        }

        private void dgvProizv_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {//производители
            if (fl_ch == true) //если произошло начальное форматирование таблиц
            {
                try
                {
                    DataGridViewColumn col = dgvProizv.Columns[e.ColumnIndex];
                    DataGridViewCell current = dgvProizv.CurrentCell;

                    str = current.Value.ToString();
                    Match m = Regex.Match(str, @"^.+$", RegexOptions.IgnoreCase);
                    if (!m.Success)
                    {
                        MessageBox.Show("Поле не может быть пустым! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                        current.ErrorText = "Поле не может быть пустым!";
                    }
                    else
                    {
                        if (dgvProizv.CurrentRow.ErrorText == "")
                        WCF_service.Ch_Line(dgvProizv.ToString(), dgvProizv.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvProizv.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvProizv.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvProizv.Columns[0].Name.ToString());
                        current.ErrorText = null;
                    }
                }
                catch (System.Exception ex) { }
            }
        }

        private void dgvRab_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {//работники
            if (fl_ch == true) //если произошло начальное форматирование таблиц
            {
                try
                {
                    DataGridViewColumn col = dgvRab.Columns[e.ColumnIndex];
                    DataGridViewCell current = dgvRab.CurrentCell;

                    //поля ФИО
                    if (col.Name.Contains("name"))
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^[А-ЯЁA-Z][а-яёa-z]+ [А-ЯЁA-Z][а-яёa-z]+ [А-ЯЁA-Z][а-яёa-z]+$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Введенные ФИО некорректны! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Введенные ФИО некорректны!";
                        }
                        else
                        {
                            if (dgvRab.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvRab.Tag.ToString(), dgvRab.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvRab.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvRab.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvRab.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //поля с датами
                    else if ((col.Name.Contains("date")) || (col.Name == "data_priema"))
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^[0-9]{2}.[0-9]{2}.[0-9]{4}", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Дата введена некорректно! Используемый формат: 'dd.mm.gggg'\n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Дата введена некорректно!";
                        }
                        else
                        {
                            if (dgvRab.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvRab.Tag.ToString(), dgvRab.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvRab.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvRab.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvRab.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //отдельная проверка для поля с датой увольнения (может быть пустым)
                    else if (col.Name == "data_uvolnenia")
                    {
                        str = current.Value.ToString();
                        if (str != "")
                        {
                            Match m = Regex.Match(str, @"^[0-9]{2}\.[0-9]{2}\.[0-9]{4}", RegexOptions.IgnoreCase);
                            if (!m.Success)
                            {
                                MessageBox.Show("Дата введена некорректно! Используемый формат: 'dd.mm.gggg' \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                                current.ErrorText = "Дата введена некорректно!";
                            }
                            else
                            {
                                if (dgvRab.CurrentRow.ErrorText == "")
                                WCF_service.Ch_Line(dgvRab.Tag.ToString(), dgvRab.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvRab.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvRab.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvRab.Columns[0].Name.ToString());
                                current.ErrorText = null;
                            }
                        }
                        else
                        {
                            if (dgvRab.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvRab.Tag.ToString(), dgvRab.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvRab.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvRab.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvRab.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //поле с номером телефона
                    else if (col.Name.Contains("telefon"))
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^[0-9]{3}-[0-9]{3}-[0-9]{4}", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Номер телефона введен некорректно! Используемый формат: '111-111-1111' \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Номер телефона введен некорректно!";
                        }
                        else
                        {
                            if (dgvRab.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvRab.Tag.ToString(), dgvRab.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvRab.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvRab.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvRab.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //поле с номером паспорта
                    else if (col.Name == "passport")
                    {
                        str = current.Value.ToString();
                        Match m1 = Regex.Match(str, @"^[А-ЯЁA-Z]{2}[0-9]{6,8}$", RegexOptions.IgnoreCase);
                        Match m2 = Regex.Match(str, @"^[0-9]{8,10}$", RegexOptions.IgnoreCase);
                        if (!m1.Success && !m2.Success)
                        {
                            MessageBox.Show("Введенные паспортные данные некорректны! Должны быть введены: серия+номер (без пробела) или номер паспорта. \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Введенные паспортные данные некорректны!";
                        }
                        else
                        {
                            if (dgvRab.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvRab.Tag.ToString(), dgvRab.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvRab.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvRab.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvRab.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //остальные текстовые поля не могут быть пустыми
                    else
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^.+$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Поле не может быть пустым! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Поле не может быть пустым!";
                        }
                        else
                        {
                            if (dgvRab.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvRab.Tag.ToString(), dgvRab.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvRab.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvRab.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvRab.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }

                }
                catch (System.Exception ex) { }
            }
        }

        private void dgvKlienty_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (fl_ch == true) //если произошло начальное форматирование таблиц
            {
                try
                {
                    DataGridViewColumn col = dgvKlienty.Columns[e.ColumnIndex];
                    DataGridViewCell current = dgvKlienty.CurrentCell;

                    //поля ФИО
                    if (col.Name.Contains("FIO"))
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^[А-ЯЁA-Z][а-яёa-z]+ [А-ЯЁA-Z][а-яёa-z]+ [А-ЯЁA-Z][а-яёa-z]+$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Введенные ФИО некорректны! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Введенные ФИО некорректны!";
                        }
                        else
                        {
                            if (dgvKlienty.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvKlienty.Tag.ToString(), dgvKlienty.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvKlienty.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvKlienty.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvKlienty.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //поле с номером телефона
                    else if (col.Name.Contains("telefon"))
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^[0-9]{3}-[0-9]{3}-[0-9]{4}", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Номер телефона введен некорректно! Используемый формат: '111-111-1111' \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Номер телефона введен некорректно!";
                        }
                        else
                        {
                            if (dgvKlienty.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvKlienty.Tag.ToString(), dgvKlienty.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvKlienty.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvKlienty.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvKlienty.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //поле с email
                    else if (col.Name.Contains("email"))
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^.+@[a-z]+\.[a-z]{2,4}$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Email введен некорректно! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Email введен некорректно!";
                        }
                        else
                        {
                            if (dgvKlienty.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvKlienty.Tag.ToString(), dgvKlienty.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvKlienty.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvKlienty.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvKlienty.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //поле с кодом ЕГРПОУ
                    else if (col.Name.Contains("code"))
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^[0-9]{6,8}$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Код ЕГРПОУ может состоять только из цифр! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Код ЕГРПОУ может состоять только из цифр!";
                        }
                        else
                        {
                            if (dgvKlienty.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvKlienty.Tag.ToString(), dgvKlienty.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvKlienty.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvKlienty.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvKlienty.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //остальные текстовые поля не могут быть пустыми
                    else
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^.+$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Поле не может быть пустым! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Поле не может быть пустым!";
                        }
                        else
                        {
                            if (dgvKlienty.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvKlienty.Tag.ToString(), dgvKlienty.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvKlienty.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvKlienty.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvKlienty.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }

                }
                catch (System.Exception ex) { }
            }
        }

        private void dgvPost_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {//поставщики
            if (fl_ch == true) //если произошло начальное форматирование таблиц
            {
                try
                {
                    DataGridViewColumn col = dgvPost.Columns[e.ColumnIndex];
                    DataGridViewCell current = dgvPost.CurrentCell;

                    //поля ФИО
                    if (col.Name.Contains("FIO"))
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^[А-ЯЁA-Z][а-яёa-z]+ [А-ЯЁA-Z][а-яёa-z]+ [А-ЯЁA-Z][а-яёa-z]+$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Введенные ФИО некорректны! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Введенные ФИО некорректны!";
                        }
                        else
                        {
                            if (dgvPost.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvPost.Tag.ToString(), dgvPost.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvPost.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvPost.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvPost.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //поле с номером телефона
                    else if (col.Name.Contains("telefon"))
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^[0-9]{3}-[0-9]{3}-[0-9]{4}", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Номер телефона введен некорректно! Используемый формат: '111-111-1111' \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Номер телефона введен некорректно!";
                        }
                        else
                        {
                            if (dgvPost.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvPost.Tag.ToString(), dgvPost.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvPost.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvPost.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvPost.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //поле с email
                    else if (col.Name.Contains("email"))
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^.+@[a-z]+\.[a-z]{2,4}$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Email введен некорректно! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Email введен некорректно!";
                        }
                        else
                        {
                            if (dgvPost.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvPost.Tag.ToString(), dgvPost.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvPost.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvPost.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvPost.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //поле с кодом ЕГРПОУ
                    else if (col.Name.Contains("code"))
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^[0-9]{6,8}$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Код ЕГРПОУ может состоять только из цифр! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Код ЕГРПОУ может состоять только из цифр!";
                        }
                        else
                        {
                            if (dgvPost.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvPost.Tag.ToString(), dgvPost.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvPost.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvPost.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvPost.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //остальные текстовые поля не могут быть пустыми
                    else
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^.+$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Поле не может быть пустым! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Поле не может быть пустым!";
                        }
                        else
                        {
                            if (dgvPost.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvPost.Tag.ToString(), dgvPost.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvPost.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvPost.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvPost.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }

                }
                catch (System.Exception ex) { }
            }
        }

        private void dgvRashodTTN_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {//расходТТН
            if (fl_ch == true) //если произошло начальное форматирование таблиц
            {
                try
                {
                    DataGridViewColumn col = dgvRashodTTN.Columns[e.ColumnIndex];
                    DataGridViewCell current = dgvRashodTTN.CurrentCell;

                    //поля ФИО
                    if (col.Name == "FIO_pol")
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^[А-ЯЁA-Z][а-яёa-z]+ [А-ЯЁA-Z][а-яёa-z]+ [А-ЯЁA-Z][а-яёa-z]+$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Введенные ФИО некорректны! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Введенные ФИО некорректны!";
                        }
                        else
                        {
                            if (dgvRashodTTN.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvRashodTTN.Tag.ToString(), dgvRashodTTN.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvRashodTTN.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvRashodTTN.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvRashodTTN.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //поле с кодом ТТН
                    else if (col.Name.Contains("TTN"))
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^[0-9]{6,18}$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Номер ТТН может состоять только из цифр! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Номер ТТН может состоять только из цифр!";
                        }
                        else
                        {
                            if (dgvRashodTTN.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvRashodTTN.Tag.ToString(), dgvRashodTTN.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvRashodTTN.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvRashodTTN.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvRashodTTN.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //поля с датами
                    else if (col.Name.Contains("data"))
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^[0-9]{2}.[0-9]{2}.[0-9]{4}", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Дата введена некорректно! Используемый формат: 'dd.mm.gggg'\n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Дата введена некорректно!";
                        }
                        else
                        {
                            if (dgvRashodTTN.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvRashodTTN.Tag.ToString(), dgvRashodTTN.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvRashodTTN.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvRashodTTN.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvRashodTTN.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //остальные текстовые поля не могут быть пустыми
                    else
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^.+$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Поле не может быть пустым! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Поле не может быть пустым!";
                        }
                        else
                        {
                            string column = "";
                            if (dgvRashodTTN.Columns[e.ColumnIndex].Name.ToString() == "firm_pol")
                                column = "klient_id";
                            else if (dgvRashodTTN.Columns[e.ColumnIndex].Name.ToString() == "rash_FIO_otpr")
                                column = "rab_id";
                            else
                                column = dgvTov.Columns[e.ColumnIndex].Name.ToString();
                            if (dgvRashodTTN.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvRashodTTN.Tag.ToString(), column,
                                                            dgvRashodTTN.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvRashodTTN.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvRashodTTN.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }

                }
                catch (System.Exception ex) { }
            }
        }

        private void dgvPrihodTTN_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {//приходТТН
            if (fl_ch == true) //если произошло начальное форматирование таблиц
            {
                try
                {
                    DataGridViewColumn col = dgvPrihodTTN.Columns[e.ColumnIndex];
                    DataGridViewCell current = dgvPrihodTTN.CurrentCell;

                    //поля ФИО
                    if (col.Name == "FIO_otpr")
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^[А-ЯЁA-Z][а-яёa-z]+ [А-ЯЁA-Z][а-яёa-z]+ [А-ЯЁA-Z][а-яёa-z]+$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Введенные ФИО некорректны! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Введенные ФИО некорректны!";
                        }
                        else
                        {
                            if (dgvPrihodTTN.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvPrihodTTN.Tag.ToString(), dgvPrihodTTN.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvPrihodTTN.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvPrihodTTN.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvPrihodTTN.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //поле с кодом ТТН
                    else if (col.Name.Contains("TTN"))
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^[0-9]{6,18}$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Номер ТТН может состоять только из цифр! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Номер ТТН может состоять только из цифр!";
                        }
                        else
                        {
                            if (dgvPrihodTTN.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvPrihodTTN.Tag.ToString(), dgvPrihodTTN.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvPrihodTTN.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvPrihodTTN.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvPrihodTTN.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //поля с датами
                    else if (col.Name.Contains("data"))
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^[0-9]{2}.[0-9]{2}.[0-9]{4}", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Дата введена некорректно! Используемый формат: 'dd.mm.gggg'\n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Дата введена некорректно!";
                        }
                        else
                        {
                            if (dgvPrihodTTN.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvPrihodTTN.Tag.ToString(), dgvPrihodTTN.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvPrihodTTN.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvPrihodTTN.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvPrihodTTN.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //остальные текстовые поля не могут быть пустыми
                    else
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^.+$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Поле не может быть пустым! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Поле не может быть пустым!";
                        }
                        else
                        {
                            string column = "";
                            if (dgvPrihodTTN.Columns[e.ColumnIndex].Name.ToString() == "firm_otpr")
                                column = "post_id";
                            else if (dgvPrihodTTN.Columns[e.ColumnIndex].Name.ToString() == "prih_FIO_pol")
                                column = "rab_id";
                            else
                                column = dgvTov.Columns[e.ColumnIndex].Name.ToString();
                            if (dgvPrihodTTN.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvPrihodTTN.Tag.ToString(), column,
                                                            dgvPrihodTTN.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvPrihodTTN.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvPrihodTTN.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }

                }
                catch (System.Exception ex) { }
            }
        }

        private void dgvRashod_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {//расход товаров
            if (fl_ch == true) //если произошло начальное форматирование таблиц
            {
                try
                {
                    DataGridViewColumn col = dgvRashod.Columns[e.ColumnIndex];
                    DataGridViewCell current = dgvRashod.CurrentCell;

                    //поле с количеством
                    if (col.Name.Contains("kol"))
                    {
                        if (Convert.ToInt32(current.Value) < 0)
                        {
                            MessageBox.Show("Значение поля не может быть отрицательным! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Значение поля не может быть отрицательным!";
                        }
                        else
                        {
                            if (dgvRashod.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvRashod.Tag.ToString(), dgvRashod.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvRashod.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvRashod.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvRashod.Columns[0].Name.ToString());
                            current.ErrorText = null;

                            //функция: рассчет значения kol в dgvTov
                            WCF_service.new_kol_after_rashod(dgvRashod.CurrentRow.Cells[2].Value.ToString(), dgvRashod.CurrentRow.Cells[1].Value.ToString(), dgvRashod.CurrentRow.Cells[3].Value.ToString());
                            //обновление таблицы dgvTov
                            WCF_service.Ch_Line(dgvTov.Tag.ToString(), dgvTov.Columns[1].Name.ToString(),
                                                            dgvTov[1,1].Value.ToString(),
                                                            dgvTov[0, 1].Value.ToString(),
                                                            dgvTov.Columns[0].Name.ToString());
                        }
                    }
                    //поле с ценой
                    else if (col.Name.Contains("price"))
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^[0-9]{1,4},[0-9]{4}$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Значение цены введено некорректно! Пример корректных данных: 'mmm,mm' \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Значение цены введено некорректно!";
                        }
                        else
                        {
                            if (dgvRashod.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvRashod.Tag.ToString(), dgvRashod.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvRashod.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvRashod.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvRashod.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //остальные текстовые поля не могут быть пустыми
                    else
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^.+$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Поле не может быть пустым! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Поле не может быть пустым!";
                        }
                        else
                        {
                            string column = "";
                            if (dgvRashod.Columns[e.ColumnIndex].Name.ToString() == "rash_TTN_num")
                                column = "rashTTN_id";
                            else if (dgvRashod.Columns[e.ColumnIndex].Name.ToString() == "rash_tov_name")
                                column = "tov_id";
                            else
                                column = dgvTov.Columns[e.ColumnIndex].Name.ToString();
                            if (dgvRashod.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvRashod.Tag.ToString(), column,
                                                            dgvRashod.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvRashod.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvRashod.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }

                }
                catch (System.Exception ex) { }
            }
        }

        private void dgvPrihod_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {//приход товаров
            if (fl_ch == true) //если произошло начальное форматирование таблиц
            {
                try
                {
                    DataGridViewColumn col = dgvPrihod.Columns[e.ColumnIndex];
                    DataGridViewCell current = dgvPrihod.CurrentCell;

                    //поле с количеством
                    if (col.Name.Contains("kol"))
                    {
                        if (Convert.ToInt32(current.Value) < 0)
                        {
                            MessageBox.Show("Значение поля не может быть отрицательным! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Значение поля не может быть отрицательным!";
                        }
                        else
                        {
                            if (dgvPrihod.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvPrihod.Tag.ToString(), dgvPrihod.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvPrihod.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvPrihod.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvPrihod.Columns[0].Name.ToString());
                            current.ErrorText = null;

                            //функция: рассчет значения kol в dgvTov
                            WCF_service.new_kol_after_prihod(dgvPrihod.CurrentRow.Cells[2].Value.ToString(), dgvPrihod.CurrentRow.Cells[1].Value.ToString(), dgvPrihod.CurrentRow.Cells[3].Value.ToString());
                            //обновление таблицы dgvTov
                            WCF_service.Ch_Line(dgvTov.Tag.ToString(), dgvTov.Columns[1].Name.ToString(),
                                                            dgvTov[1, 2].Value.ToString(),
                                                            dgvTov[0, 1].Value.ToString(),
                                                            dgvTov.Columns[0].Name.ToString());
                        }
                    }
                    //поле с ценой
                    else if (col.Name.Contains("price"))
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^[0-9]{1,4},[0-9]{4}$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Значение цены введено некорректно! Пример корректных данных: 'mmm,mm' \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Значение цены введено некорректно!";
                        }
                        else
                        {
                            if (dgvPrihod.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvPrihod.Tag.ToString(), dgvPrihod.Columns[e.ColumnIndex].Name.ToString(),
                                                            dgvPrihod.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvPrihod.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvPrihod.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }
                    //остальные текстовые поля не могут быть пустыми
                    else
                    {
                        str = current.Value.ToString();
                        Match m = Regex.Match(str, @"^.+$", RegexOptions.IgnoreCase);
                        if (!m.Success)
                        {
                            MessageBox.Show("Поле не может быть пустым! \n\nЕсли происходит редактирование строки в таблице, это значение не будет добавлено в базу данных.");
                            current.ErrorText = "Поле не может быть пустым!";
                        }
                        else
                        {
                            string column = "";
                            if (dgvPrihod.Columns[e.ColumnIndex].Name.ToString() == "prih_TTN_num")
                                column = "prihTTN_id";
                            else if (dgvPrihod.Columns[e.ColumnIndex].Name.ToString() == "prih_tov_name")
                                column = "tov_id";
                            else
                                column = dgvTov.Columns[e.ColumnIndex].Name.ToString();
                            if (dgvPrihod.CurrentRow.ErrorText == "")
                            WCF_service.Ch_Line(dgvPrihod.Tag.ToString(), column,
                                                            dgvPrihod.CurrentRow.Cells[e.ColumnIndex].Value.ToString(),
                                                            dgvPrihod.CurrentRow.Cells[0].Value.ToString(),
                                                            dgvPrihod.Columns[0].Name.ToString());
                            current.ErrorText = null;
                        }
                    }

                }
                catch (System.Exception ex) { }
            }
        }

        #endregion //Edit (ValueChanged)

        //кнопка выхода из приложения
        private void buttExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        //отключение сервисов при закрытии формы
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                WCF_service.CloseClient();
                ASP_service.Close();
            }
            catch (System.Exception ex) { }
        }

    }
}
