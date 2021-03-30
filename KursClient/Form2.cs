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
    public partial class Form2 : Form
    {
        KursClient.ASP.ASPSoapClient ASP_service;

        public Form2()
        {
            InitializeComponent();

            //заполнение выпадающего списка значениями категорий
            ASP_service = new ASPSoapClient();
            DataSet dt = ASP_service.Get_Table("tKategorii_tovara");
            cbKat.DataSource = dt.Tables[0];
            cbKat.ValueMember = "kat_id";
            cbKat.DisplayMember = "kat_name";
        }

        private void buttSearch2_Click(object sender, EventArgs e)
        {
            dgvSearchForm.DataSource = null;
            dgvSearchForm.Rows.Clear();

            DataSet dt = ASP_service.Get_Tovar_by_Cat_id(cbKat.SelectedValue.ToString());//сервис получает значение id
            dgvSearchForm.DataSource = dt.Tables[0];

            //заголовки столбцов результирующейтаблицы:
            dgvSearchForm.Columns["tov_name"].HeaderText = "Наименование товара";
            dgvSearchForm.Columns["oboznachenie"].HeaderText = "Единицы измерения";
            dgvSearchForm.Columns["proizv_name"].HeaderText = "Производитель";
            dgvSearchForm.Columns["country"].HeaderText = "Страна происхождения";
            dgvSearchForm.Columns["kol"].HeaderText = "Количество";

            //ширина столбцов таблицы не по умолчанию
            dgvSearchForm.Columns["kol"].Width = 80;
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            ASP_service.Close();
        }

    }
}
