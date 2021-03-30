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
    public partial class Form4 : Form
    {
        KursClient.ASP.ASPSoapClient ASP_service;

        public Form4()
        {
            InitializeComponent();

            //заполнение выпадающего списка значениями ТТН расхода
            ASP_service = new ASPSoapClient();
            DataSet dt = ASP_service.Get_Table("tRashodTTN");
            cbTTN.DataSource = dt.Tables[0];
            cbTTN.ValueMember = "rashTTN_number";
            cbTTN.DisplayMember = "rashTTN_number";
        }

        private void buttSearch3_Click(object sender, EventArgs e)
        {
            dgvRashodTTN.DataSource = null;
            dgvRashodTTN.Rows.Clear();
            //
            dgvRashod.DataSource = null;
            dgvRashod.Rows.Clear();

            //textBox1.Text = cbTTN.SelectedValue.ToString();

            //ASP_service = new ASPSoapClient();
            DataSet dt1 = ASP_service.Get_Otchet_Rashod_1part(cbTTN.SelectedValue.ToString());//сервис получает значение номера ТТН
            dgvRashodTTN.DataSource = dt1.Tables[0];

            //заголовки столбцов результирующей таблицы:
            dgvRashodTTN.Columns["rashTTN_number"].HeaderText = "Номер ТТН (расход)";
            dgvRashodTTN.Columns["kl_firm_name"].HeaderText = "Фирма-получатель";
            dgvRashodTTN.Columns["FIO_pol"].HeaderText = "ФИО получателя";
            dgvRashodTTN.Columns["rab_name"].HeaderText = "ФИО отправителя";
            dgvRashodTTN.Columns["data_otpr"].HeaderText = "Дата отправления";

            //ширина столбцов таблицы не по умолчанию
            dgvRashodTTN.Columns["rashTTN_number"].Width = 120;
            dgvRashodTTN.Columns["kl_firm_name"].Width = 120;
            dgvRashodTTN.Columns["FIO_pol"].Width = 120;
            dgvRashodTTN.Columns["rab_name"].Width = 122;

            //ASP_service = new ASPSoapClient();
            DataSet dt2 = ASP_service.Get_Otchet_Rashod_2part(cbTTN.SelectedValue.ToString());//сервис получает значение номера ТТН
            dgvRashod.DataSource = dt2.Tables[0];

            //заголовки столбцов результирующей таблицы:
            dgvRashod.Columns["tov_name"].HeaderText = "Наименование товара";
            dgvRashod.Columns["rash_kol"].HeaderText = "Количество";
            dgvRashod.Columns["price_for_one_sell"].HeaderText = "Цена за единицу товара";
            dgvRashod.Columns["oboznachenie"].HeaderText = "Единицы измерения";

            //ширина столбцов таблицы не по умолчанию
            dgvRashod.Columns["tov_name"].Width = 170;
            dgvRashod.Columns["price_for_one_sell"].Width = 130;

        }
        private void Form4_FormClosed(object sender, FormClosedEventArgs e)
        {
            ASP_service.Close();
        }

    }
}
