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
    public partial class Form3 : Form
    {
        KursClient.ASP.ASPSoapClient ASP_service;

        public Form3()
        {
            InitializeComponent();

            dgvRab1.DataSource = null;
            dgvRab1.Rows.Clear();

            ASP_service = new ASPSoapClient();
            DataSet dt = ASP_service.Get_Workers();//вызывается соответствующая функция сервиса
            dgvRab1.DataSource = dt.Tables[0];

            //заголовки столбцов результирующейтаблицы:
            dgvRab1.Columns["rab_name"].HeaderText = "ФИО работника";
            dgvRab1.Columns["position"].HeaderText = "Должность";
            dgvRab1.Columns["date_of_birth"].HeaderText = "Дата рождения";
            dgvRab1.Columns["telefon"].HeaderText = "Телефон";
            dgvRab1.Columns["passport"].HeaderText = "Паспорт";
            dgvRab1.Columns["home_address"].HeaderText = "Домашний адрес";
            dgvRab1.Columns["data_priema"].HeaderText = "Дата приема на работу";

            //ширина столбцов таблицы не по умолчанию
            dgvRab1.Columns["rab_name"].Width = 155;
            dgvRab1.Columns["position"].Width = 80;
            dgvRab1.Columns["date_of_birth"].Width = 80;
            dgvRab1.Columns["telefon"].Width = 95;
            dgvRab1.Columns["passport"].Width = 80;
            dgvRab1.Columns["data_priema"].Width = 80;
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {
            ASP_service.Close();
        }
    }
}
