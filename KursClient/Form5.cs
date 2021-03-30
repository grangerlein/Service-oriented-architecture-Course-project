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

namespace KursClient
{
    public partial class Form5 : Form
    {
        KursClient.ASP.ASPSoapClient ASP_service;

        public Form5()
        {
            InitializeComponent();

            //заполнение выпадающего списка значениями ТТН расхода
            ASP_service = new ASPSoapClient();
            DataSet dt = ASP_service.Get_Table("tPrihodTTN");
            cbTTN.DataSource = dt.Tables[0];
            cbTTN.ValueMember = "prihTTN_number";
            cbTTN.DisplayMember = "prihTTN_number";
        }

        private void buttSearch4_Click(object sender, EventArgs e)
        {
            dgvPrihodTTN.DataSource = null;
            dgvPrihodTTN.Rows.Clear();
            //
            dgvPrihod.DataSource = null;
            dgvPrihod.Rows.Clear();

            //textBox1.Text = cbTTN.SelectedValue.ToString();

            //ASP_service = new ASPSoapClient();
            DataSet dt1 = ASP_service.Get_Otchet_Prihod_1part(cbTTN.SelectedValue.ToString());//сервис получает значение номера ТТН
            dgvPrihodTTN.DataSource = dt1.Tables[0];

            //заголовки столбцов результирующей таблицы:
            dgvPrihodTTN.Columns["prihTTN_number"].HeaderText = "Номер ТТН (приход)";
            dgvPrihodTTN.Columns["p_firm_name"].HeaderText = "Фирма-отправитель";
            dgvPrihodTTN.Columns["FIO_otpr"].HeaderText = "ФИО отправителя";
            dgvPrihodTTN.Columns["rab_name"].HeaderText = "ФИО получателя";
            dgvPrihodTTN.Columns["data_pol"].HeaderText = "Дата получения";

            //ширина столбцов таблицы не по умолчанию
            dgvPrihodTTN.Columns["prihTTN_number"].Width = 120;
            dgvPrihodTTN.Columns["p_firm_name"].Width = 120;
            dgvPrihodTTN.Columns["FIO_otpr"].Width = 120;
            dgvPrihodTTN.Columns["rab_name"].Width = 122;

            //ASP_service = new ASPSoapClient();
            DataSet dt2 = ASP_service.Get_Otchet_Prihod_2part(cbTTN.SelectedValue.ToString());//сервис получает значение номера ТТН
            dgvPrihod.DataSource = dt2.Tables[0];

            //заголовки столбцов результирующей таблицы:
            dgvPrihod.Columns["tov_name"].HeaderText = "Наименование товара";
            dgvPrihod.Columns["prih_kol"].HeaderText = "Количество";
            dgvPrihod.Columns["price_for_one_buy"].HeaderText = "Цена за единицу товара";
            dgvPrihod.Columns["oboznachenie"].HeaderText = "Единицы измерения";

            //ширина столбцов таблицы не по умолчанию
            dgvPrihod.Columns["tov_name"].Width = 170;
            dgvPrihod.Columns["price_for_one_buy"].Width = 130;

        }

        private void Form5_FormClosed(object sender, FormClosedEventArgs e)
        {
            ASP_service.Close();
        }

    }
}
