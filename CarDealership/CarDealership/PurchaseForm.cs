using System;
using System.Drawing;
using System.Windows.Forms;

namespace CarDealership
{
    public partial class PurchaseForm : Form
    {
        private Car _selectedCar;
        private string _username;

        public PurchaseForm(Car selectedCar, string username)
        {
            InitializeComponent();
            _selectedCar = selectedCar;
            _username = username;

            lblCarModel.Text = selectedCar.ModelName;
            lblCarPrice.Text = selectedCar.Price.ToString("C");
            lblCustomerInfo.Text = $"Customer: {username}";

            try
            {
                if (!string.IsNullOrEmpty(selectedCar.ImageUrl))
                {
                    string fullPath = System.IO.Path.Combine(Application.StartupPath, selectedCar.ImageUrl);
                    if (System.IO.File.Exists(fullPath))
                        pbCarImage.Image = Image.FromFile(fullPath);
                }
            }
            catch { }

            ApplyRedTheme();
        }

        private void ApplyRedTheme()
        {
            btnConfirm.MouseEnter += (s, e) => btnConfirm.BackColor = Color.FromArgb(217, 55, 55);
            btnConfirm.MouseLeave += (s, e) => btnConfirm.BackColor = Color.FromArgb(240, 71, 71);

            btnCancel.MouseEnter += (s, e) => btnCancel.BackColor = Color.FromArgb(94, 99, 107);
            btnCancel.MouseLeave += (s, e) => btnCancel.BackColor = Color.FromArgb(64, 68, 75);
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"Purchase request for {_selectedCar.ModelName} has been submitted!\n\n" +
                           $"You will be contacted by our sales team shortly.",
                           "Purchase Initiated",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Information);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}