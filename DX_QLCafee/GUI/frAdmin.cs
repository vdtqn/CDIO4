using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DX_QLCafee.DTO;
using DX_QLCafee.DAO;

namespace DX_QLCafee.GUI
{
    public partial class frAdmin : DevExpress.XtraEditors.XtraForm
    {
        BindingSource foodList = new BindingSource();
        

        BindingSource categoryList = new BindingSource();

        BindingSource accountList = new BindingSource();

        BindingSource tableList = new BindingSource();
        

        public Account loginAccount;
        public frAdmin()
        {
            InitializeComponent();
            LoadForm();
        }

        #region methods

        List<Food> SearchFoodByName(string name)
        {
            List<Food> listFood = FoodDAO.Instance.SearchFoodByName(name);

            return listFood;
        }
        void LoadForm()
        {
            dtgvFood.DataSource = foodList;
            dtgvAccount.DataSource = accountList;
            dtgvCategory.DataSource = categoryList;
            dtgvTable.DataSource = tableList;
            btnSelect.Hide();

            LoadDateTimePickerBill();
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
            LoadListFood();
            
            LoadListCategory();
            LoadAccount();
            LoadListTable();
            LoadCategoryIntoCombobox(cboCategory);
            //LoadStatusIntoCombobox(cbTableStatus);
            AddFoodBinding();
            AddAccountBinding();
            AddCategoryBinding();
            AddTableBinding();
        }

        void AddAccountBinding()
        {
            txtUserName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "UserName", true, DataSourceUpdateMode.Never));
            txtDisplayName.DataBindings.Add(new Binding("Text", dtgvAccount.DataSource, "DisplayName", true, DataSourceUpdateMode.Never));
            numericUpDown4.DataBindings.Add(new Binding("Value", dtgvAccount.DataSource, "Type", true, DataSourceUpdateMode.Never));
        }

        void LoadAccount()
        {
            accountList.DataSource = AccountDAO.Instance.GetListAccount();
        }
        void LoadDateTimePickerBill()
        {
            DateTime today = DateTime.Now;
            dtpkFromDate.Value = new DateTime(today.Year, today.Month, 1);
            dtpkToDate.Value = dtpkFromDate.Value.AddMonths(1).AddDays(-1);
        }
        void LoadListBillByDate(DateTime checkIn, DateTime checkOut)
        {
            dtgvBill.DataSource = BillDAO.Instance.GetBillListByDate(checkIn, checkOut);
        }

        void AddFoodBinding()
        {
            
            
            cboCategory.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "CategoryID", true, DataSourceUpdateMode.Never));
            nmFoodPrice.DataBindings.Add(new Binding("Value", dtgvFood.DataSource, "Price", true, DataSourceUpdateMode.Never));
            txtIDF.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "ID", true, DataSourceUpdateMode.Never));
            txtNameF.DataBindings.Add(new Binding("Text", dtgvFood.DataSource, "Name", true, DataSourceUpdateMode.Never));
        }

        void AddCategoryBinding()
        {
            txtIDC.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "ID", true, DataSourceUpdateMode.Never));
            txtNameC.DataBindings.Add(new Binding("Text", dtgvCategory.DataSource, "Name", true, DataSourceUpdateMode.Never));
            

        }

        void AddTableBinding()
        {
            txtIDT.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "ID", true, DataSourceUpdateMode.Never));
            txtNameT.DataBindings.Add(new Binding("Text", dtgvTable.DataSource, "Name", true, DataSourceUpdateMode.Never));
            

        }

        void LoadCategoryIntoCombobox(System.Windows.Forms.ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategory();
            cb.DisplayMember = "Name";
        }

        void LoadStatusIntoCombobox(System.Windows.Forms.ComboBox cb)
        {
            cb.DataSource = TableDAO.Instance.LoadTableList();
            cb.DisplayMember = "status";
        }
        void LoadListFood()
        {
            foodList.DataSource = FoodDAO.Instance.GetListFood();
        }

        
        void LoadListCategory()
        {
            categoryList.DataSource = CategoryDAO.Instance.GetListCategory();
        }

        void LoadListTable()
        {
            tableList.DataSource = TableDAO.Instance.LoadTableList();
        }

        void AddAccount(string userName, string displayName, int type)
        {
            if (AccountDAO.Instance.InsertAccount(userName, displayName, type))
            {
                MessageBox.Show("Thêm tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Thêm tài khoản thất bại");
            }

            LoadAccount();
        }

        void EditAccount(string userName, string displayName, int type)
        {
            if (AccountDAO.Instance.UpdateAccount(userName, displayName, type))
            {
                MessageBox.Show("Cập nhật tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Cập nhật tài khoản thất bại");
            }

            LoadAccount();
        }

        void DeleteAccount(string userName)
        {
            if (loginAccount.UserName.Equals(userName))
            {
                MessageBox.Show("Vui lòng đừng xóa chính bạn chứ");
                return;
            }
            if (AccountDAO.Instance.DeleteAccount(userName))
            {
                MessageBox.Show("Xóa tài khoản thành công");
            }
            else
            {
                MessageBox.Show("Xóa tài khoản thất bại");
            }

            LoadAccount();
        }

        void ResetPass(string userName)
        {
            if (AccountDAO.Instance.ResetPassword(userName))
            {
                MessageBox.Show("Đặt lại mật khẩu thành công");
            }
            else
            {
                MessageBox.Show("Đặt lại mật khẩu thất bại");
            }
        }
        #endregion

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnDeleteFood_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txtIDF.Text);

            if (FoodDAO.Instance.DeleteFood(id))
            {
                MessageBox.Show("Xóa món thành công");
                LoadListFood();
                if (deleteFood != null)
                    deleteFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi xóa thức ăn");
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            LoadListFood();
            this.btnSelect.Hide();
           
        }

        private void panel25_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnAddFood_Click(object sender, EventArgs e)
        {
            string name = txtNameF.Text;
            int categoryIDx = (cboCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;
            string idF = txtIDF.Text;

            if (name.Equals("") || categoryIDx.Equals(""))
            {
                MessageBox.Show("Không để trống tên món mới và danh mục");
            }

            else if (FoodDAO.Instance.InsertFood(name, categoryIDx, price))
            {
                MessageBox.Show("Thêm món thành công");
                LoadListFood();
                if (insertFood != null)
                    insertFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi thêm món mới");
            }
            
        }

        private void simpleButton15_Click(object sender, EventArgs e)
        {
            string userName = txtUserName.Text;
            string displayName = txtDisplayName.Text;
            int type = (int)numericUpDown4.Value;
            if (userName.Equals("") || displayName.Equals(""))
            {
                MessageBox.Show("Không để trống tên khi thêm tài khoản mới");
            }
            else
            {

                AddAccount(userName, displayName, type);
            }
            
            
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            string userName = txtUserName.Text;

            DeleteAccount(userName);
        }

        private void btnUpdateAccount_Click(object sender, EventArgs e)
        {
            string userName = txtUserName.Text;
            string displayName = txtDisplayName.Text;
            int type = (int)numericUpDown4.Value;

            EditAccount(userName, displayName, type);
        }

        
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string search = txtSearchName.Text.ToLower();
            string idF = txtIDF.Text;
            string name = txtNameF.Text;
            int categoryID = (cboCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;
            if (!search.Equals("")) 
            {
                foodList.DataSource = SearchFoodByName(txtSearchName.Text);
                btnSelect.Show();
            }
            else
            {
                MessageBox.Show("Chưa nhập tên món cần tìm");
            }
            
        }

        private void btnUpdateFood_Click(object sender, EventArgs e)
        {
            string name = txtNameF.Text;
            int categoryID = (cboCategory.SelectedItem as Category).ID;
            float price = (float)nmFoodPrice.Value;
            int id = Convert.ToInt32(txtIDF.Text);

            if (FoodDAO.Instance.UpdateFood(id, name, categoryID, price))
            {
                MessageBox.Show("Sửa món thành công");
                LoadListFood();
                if (updateFood != null)
                    updateFood(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi sửa thức ăn");
            }
        }

        private event EventHandler insertFood;
        public event EventHandler InsertFood
        {
            add { insertFood += value; }
            remove { insertFood -= value; }
        }

        private event EventHandler deleteFood;
        public event EventHandler DeleteFood
        {
            add { deleteFood += value; }
            remove { deleteFood -= value; }
        }

        private event EventHandler updateFood;
        public event EventHandler UpdateFood
        {
            add { updateFood += value; }
            remove { updateFood -= value; }
        }

        private void btnAddCategory_Click(object sender, EventArgs e)
        {
            string name = txtNameC.Text;

            if (name.Equals(""))
            {
                MessageBox.Show("Không được để trống tên danh mục");
            }
            else if (CategoryDAO.Instance.InsertCategory(name))
            {
                LoadListCategory();
                if (insertCategory != null)
                    insertCategory(this, new EventArgs());
                MessageBox.Show("Thêm danh mục thành công");
            }
            else
            {
                MessageBox.Show("Có lỗi khi thêm danh mục");
            }
        }
        

        private void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txtIDC.Text);
            //Food food = dtgvFood.Tag as Food;

            //int idc = FoodDAO.Instance.GetCategoryIDFood(food.CategoryID);
            //if (id!=idc)
            //{
            try
            {
                if (CategoryDAO.Instance.DeleteCategory(id))
                {


                    LoadListCategory();
                    if (deleteCategory != null)
                        deleteCategory(this, new EventArgs());
                    MessageBox.Show("Xóa danh mục thành công");
                }
                else
                {
                    MessageBox.Show("Có lỗi khi xóa danh mục");
                }
            }
            catch (Exception )
            {

                throw;
            }
                
            //}
            //else
            //{
            //    MessageBox.Show("Hãy xóa hết thực phẩm trong danh mục");
            //}
            
        }

        private void btnUpdateCategory_Click(object sender, EventArgs e)
        {
            string name = txtNameC.Text;
            int id = Convert.ToInt32(txtIDC.Text);

            if (CategoryDAO.Instance.UpdateCategory(id, name))
            {
                
                LoadListCategory();
                if (updateCategory != null)
                    updateCategory(this, new EventArgs());
                MessageBox.Show("Sửa danh mục thành công");
            }
            else
            {
                MessageBox.Show("Có lỗi khi sửa danh mục");
            }
        }

        private event EventHandler insertCategory;
        public event EventHandler InsertCategory
        {
            add { insertCategory += value; }
            remove { insertCategory -= value; }
        }

        private event EventHandler deleteCategory;
        public event EventHandler DeleteCategory
        {
            add { deleteCategory += value; }
            remove { deleteCategory -= value; }
        }

        private event EventHandler updateCategory;
        public event EventHandler UpdateCategory
        {
            add { updateCategory += value; }
            remove { updateCategory -= value; }
        }

        private void btnAddTable_Click(object sender, EventArgs e)
        {
            string name = txtNameT.Text;
            string status = cboStatusT.SelectedItem.ToString();
            //int selectedIndex = cbTableStatus.SelectedIndex;
            //Object selectedItem = cbTableStatus.SelectedItem;

            //MessageBox.Show("Selected Item Text: " + selectedItem.ToString() + "\n" +
            //                "Index: " + selectedIndex.ToString());
            if (name.Equals("")||status.Equals(null))
            {
                MessageBox.Show("Không được để trống tên bàn");
            }
            else if (TableDAO.Instance.InsertTable(name, status))
            {
                
                LoadListTable();
                if (insertTable != null)
                    insertTable(this, new EventArgs());
                MessageBox.Show("Thêm bàn ăn mới thành công");
            }
            else
            {
                MessageBox.Show("Có lỗi khi thêm bàn ăn mới");
            }
            //frmTableManage frmTableManage = new frmTableManage();
            //frmTableManage.ResumeLayout();
        }

        private void btnDeleteTable_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txtIDT.Text);

            if (TableDAO.Instance.DeleteTable(id))
            {
                
                LoadListTable();
                if (deleteTable != null)
                    deleteTable(this, new EventArgs());

                MessageBox.Show("Xóa bàn ăn thành công");
            }
            else
            {
                MessageBox.Show("Có lỗi khi xóa bàn ăn");
            }
        }

        private void btnUpdateTable_Click(object sender, EventArgs e)
        {
            string name = txtNameT.Text;
            int id = Convert.ToInt32(txtIDT.Text);
            string status = cboStatusT.SelectedItem.ToString();
            //MessageBox.Show(status);
            if (name.Equals("")||status.Equals(null))
            {
                MessageBox.Show("Có lỗi khi sửa thông tin bàn ăn");
            }
            else if (TableDAO.Instance.UpdateTable(id, name, status))
            {
                MessageBox.Show("Sửa thông tin bàn ăn thành công");
                LoadListTable();
                if (updateTable != null)
                    updateTable(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi khi sửa thông tin bàn ăn");
            }
        }

        private event EventHandler insertTable;
        public event EventHandler InsertTable
        {
            add { insertTable += value; }
            remove { insertTable -= value; }
        }

        private event EventHandler deleteTable;
        public event EventHandler DeleteTable
        {
            add { deleteTable += value; }
            remove { deleteTable -= value; }
        }

        private event EventHandler updateTable;
        public event EventHandler UpdateTable
        {
            add { updateTable += value; }
            remove { updateTable -= value; }
        }

        private void dateNavigator1_Click(object sender, EventArgs e)
        {

        }

        private void cboStatusT_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void frAdmin_FormClosed(object sender, FormClosedEventArgs e)
        {
            frmTableManage f = new frmTableManage();
            f.Update();
        }

        private void frAdmin_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void dtgvFood_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnThongkeNgay_Click(object sender, EventArgs e)
        {
            LoadListBillByDate(dtpkFromDate.Value, dtpkToDate.Value);
        }

        //#endregion

    }
}