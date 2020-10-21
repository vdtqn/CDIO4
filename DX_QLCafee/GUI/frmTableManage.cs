using DX_QLCafee.DAO;
using DX_QLCafee.DTO;
using DX_QLCafee.GUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static DX_QLCafee.GUI.frmAccount;

namespace DX_QLCafee
{
    public partial class frmTableManage : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public frmTableManage()
        {
            InitializeComponent();
        }

        private Account loginAccount;

        public Account LoginAccount
        {
            get { return loginAccount; }
            set { loginAccount = value; ChangeAccount(loginAccount.Type); }
        }
        public frmTableManage(Account acc)
        {
            InitializeComponent();

            this.LoginAccount = acc;

            LoadTable();
            LoadCategory();
            //LoadFoodListByCategoryID();
            LoadComboboxTable(cbSwitchTable);
        }

        #region Method

        void ChangeAccount(int type)
        {
            int typeLog = LoginAccount.Type;
            if (typeLog!=1)
            {
                ribbonPage1.Visible = false;
                //ribbonPage1.PageIndex.GetType.Enabled = type == 1;
                ribbonPage2.Text += " (" + LoginAccount.DisplayName + ")";
            }
            else
            {
                ribbonPage1.Visible=true;
                ribbonPage2.Text += " (" + LoginAccount.DisplayName + ")";
            }
            
        }
        void LoadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.GetListCategory();
            cboCategory.DataSource = listCategory;
            cboCategory.DisplayMember = "Name";
        }

        void LoadFoodListByCategoryID(int id)
        {
            //int id = (cbCategory.SelectedItem as Category).ID;
            List<Food> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            cboFood.DataSource = listFood;
            cboFood.DisplayMember = "Name";

            //List<Food> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            //cbFood.DataSource = listFood;
            //cbFood.DisplayMember = "Name";
        }
        void LoadTable()
        {
            flpTable.Controls.Clear();

            List<Table> tableList = TableDAO.Instance.LoadTableList();

            foreach (Table item in tableList)
            {
                Button btn = new Button() { Width = TableDAO.TableWidth, Height = TableDAO.TableHeight };
                btn.Text = item.Name + Environment.NewLine + item.Status;
                btn.Click += btn_Click;
                btn.Tag = item;

                //switch (item.Status)
                //{
                //    case "Trống":
                //        btn.BackColor = Color.Aqua;
                //        break;
                //    default:
                //        btn.BackColor = Color.LightPink;
                //        break;
                //}
                if (item.Status.Equals("Trống"))
                {
                    btn.BackColor = Color.Aqua;
                    //break;
                }
                else
                {
                    btn.BackColor = Color.LightPink;
                    //break;
                }
                flpTable.Controls.Add(btn);
            }
        }

        void ShowBill(int id)
        {
            lsvBill.Items.Clear();
            List<DX_QLCafee.DTO.Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);
            float totalPrice = 0;
            foreach (DX_QLCafee.DTO.Menu item in listBillInfo)
            {
                ListViewItem lsvItem = new ListViewItem(item.FoodName.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.TotalPrice.ToString());
                totalPrice += item.TotalPrice;
                lsvBill.Items.Add(lsvItem);
            }
            CultureInfo culture = new CultureInfo("vi-VN");

            Thread.CurrentThread.CurrentCulture = culture;

            txtTotalPrice.Text = totalPrice.ToString("c", culture);

        }

        void LoadComboboxTable(ComboBox cb)
        {
            cb.DataSource = TableDAO.Instance.LoadTableList();
            cb.DisplayMember = "Name";
        }

        void btn_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).ID;
            lsvBill.Tag = (sender as Button).Tag;
            ShowBill(tableID);
        }

        void f_UpdateFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cboCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }

        void f_DeleteFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cboCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
            LoadTable();
        }

        void f_InsertFood(object sender, EventArgs e)
        {
            LoadFoodListByCategoryID((cboCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }

        #endregion

        private void btnDangxuat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Application.Exit();
            //frmLogin2 fl = new frmLogin2();
            //fl.Show();
            //this.Close();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnInfor_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frmAccount f = new frmAccount(LoginAccount);
            f.UpdateAccount += f_UpdateAccount;
            f.ShowDialog();
        }

        void f_UpdateAccount(object sender, AccountEvent e)
        {
            ribbonPage2.Text = "Thông tin tài khoản (" + e.Acc.DisplayName + ")";
        }

        public void MyrefeshMethod()
        {
            MessageBox.Show("refreshed");
        }

        private void btnAdmin_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            frAdmin f = new frAdmin();
            f.loginAccount = LoginAccount;
            f.InsertFood += f_InsertFood;
            f.DeleteFood += f_DeleteFood;
            f.UpdateFood += f_UpdateFood;
            //f.FormClosing += new FormClosingEventHandler(ChildFormClosing);
            f.Show();
            //this.FormClosed();
        }

        private void ChildFormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;

            if (table == null)
            {
                MessageBox.Show("Hãy chọn bàn");
                return;
            }

            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int foodID = (cboFood.SelectedItem as Food).ID;
            int count = (int)nmFoodCount.Value;

            if (idBill == -1)
            {
                BillDAO.Instance.InsertBill(table.ID);
                BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID, count);
                TableDAO.Instance.UpdateTableStatus(table.ID, "Có người");
            }
            else
            {
                BillInfoDAO.Instance.InsertBillInfo(idBill, foodID, count);
                TableDAO.Instance.UpdateTableStatus(table.ID, "Có người");
            }

            ShowBill(table.ID);

            LoadTable();
        }

        private void btnCheckout_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;

            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int discount = (int)nmDisCount.Value;

            double totalPrice = Convert.ToDouble(txtTotalPrice.Text.Split(',')[0]);
            double finalTotalPrice = totalPrice - (totalPrice / 100) * discount;

            if (idBill != -1)
            {
                if (MessageBox.Show(string.Format("Bạn có chắc thanh toán hóa đơn cho bàn {0}\n\nTổng tiền - (Tổng tiền / 100) x Giảm giá\n\n=> {1} - ({1} / 100) x {2} = {3}", table.Name, totalPrice, discount, finalTotalPrice), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
                {
                    BillDAO.Instance.CheckOut(idBill, discount, (float)finalTotalPrice);
                    ShowBill(table.ID);

                    LoadTable();
                }
            }
        }

        private void btnSwitchTable_Click(object sender, EventArgs e)
        {
            int id1 = (lsvBill.Tag as Table).ID;

            int id2 = (cbSwitchTable.SelectedItem as Table).ID;

            if (MessageBox.Show(string.Format("Bạn có thật sự muốn chuyển bàn {0} qua bàn {1}", (lsvBill.Tag as Table).Name, (cbSwitchTable.SelectedItem as Table).Name), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                TableDAO.Instance.SwitchTable(id1, id2);

                LoadTable();
            }
        }

        private void cboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;

            ComboBox cb = sender as ComboBox;

            if (cb.SelectedItem == null)
                return;

            Category selected = cb.SelectedItem as Category;
            id = selected.ID;
            List<Food> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            cboFood.DataSource = listFood;
            cboFood.DisplayMember = "Name";
            LoadFoodListByCategoryID(id);
        }
        private void frmTableManage_FormClosing_1(object sender, FormClosingEventArgs e)
        {
            frmLogin2 fl = new frmLogin2();
            
            //this.Close();

            if (MessageBox.Show("Bạn có thật sự muốn thoát chương trình?", "Thông báo", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
            {
                e.Cancel = true;
            }

        }

        private void ribbonControl1_Click(object sender, EventArgs e)
        {

        }

        private void ribbonControl1_ResetLayout(object sender, DevExpress.XtraBars.Ribbon.ResetLayoutEventArgs e)
        {

        }
    }
}