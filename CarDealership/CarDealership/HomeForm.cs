using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace CarDealership
{
    public partial class HomeForm : Form
    {
        private string _connectionString;
        private string _username;
        private List<Car> cars;
        private int currentIndex = 0;

        public HomeForm(string connectionString, string username)
        {
            InitializeComponent();
            _connectionString = connectionString;
            _username = username;
            lblUserGreeting.Text = "Welcome, " + username;

            ApplyRedTheme();

            pbCarImage.BackColor = Color.Transparent;
            pbCarImage.SizeMode = PictureBoxSizeMode.Zoom;

            LoadCars();
        }

        private void ApplyRedTheme()
        {
            btnLogout.MouseEnter += (s, e) => btnLogout.BackColor = Color.FromArgb(217, 55, 55);
            btnLogout.MouseLeave += (s, e) => btnLogout.BackColor = Color.FromArgb(240, 71, 71);

            btnSelect.MouseEnter += (s, e) => btnSelect.BackColor = Color.FromArgb(217, 55, 55);
            btnSelect.MouseLeave += (s, e) => btnSelect.BackColor = Color.FromArgb(240, 71, 71);

            btnPrevious.MouseEnter += (s, e) => btnPrevious.BackColor = Color.FromArgb(94, 99, 107);
            btnPrevious.MouseLeave += (s, e) => btnPrevious.BackColor = Color.FromArgb(64, 68, 75);

            btnNext.MouseEnter += (s, e) => btnNext.BackColor = Color.FromArgb(94, 99, 107);
            btnNext.MouseLeave += (s, e) => btnNext.BackColor = Color.FromArgb(64, 68, 75);
        }

        private void LoadCars()
        {
            cars = new List<Car>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = "SELECT CarID, ModelName, Price, ImageUrl, Year, Description FROM Cars";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            cars.Add(new Car
                            {
                                CarID = (int)reader["CarID"],
                                ModelName = reader["ModelName"].ToString(),
                                Price = (decimal)reader["Price"],
                                ImageUrl = reader["ImageUrl"]?.ToString() ?? "",
                                Year = reader["Year"] != DBNull.Value ? (int)reader["Year"] : 0,
                                Description = reader["Description"]?.ToString() ?? ""
                            });
                        }
                    }
                }

                if (cars.Count > 0)
                {
                    ShowCar(currentIndex);
                    timerCarousel.Start();
                }
                else
                {
                    MessageBox.Show("No cars available to display.", "Information",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading cars: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowCar(int index)
        {
            if (cars == null || cars.Count == 0) return;

            try
            {
                var car = cars[index];
                lblCarName.Text = $"{car.ModelName} {(car.Year > 0 ? $"({car.Year})" : "")}";
                lblCarPrice.Text = car.Price.ToString("C");

                if (pbCarImage.Image != null)
                {
                    pbCarImage.Image.Dispose();
                    pbCarImage.Image = null;
                }

                if (!string.IsNullOrEmpty(car.ImageUrl))
                {
                    LoadImageFromFile(car.ImageUrl);
                }
                else
                {
                    CreatePlaceholderImage(car.ModelName, "No image path");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying car: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadImageFromFile(string imagePath)
        {
            try
            {
                string fullPath = Path.Combine(Application.StartupPath, imagePath);

                if (!File.Exists(fullPath))
                {
                    string fileName = Path.GetFileName(imagePath);
                    fullPath = Path.Combine(Application.StartupPath, "CarImages", fileName);
                }

                if (File.Exists(fullPath))
                {
                    using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                    {
                        Image original = Image.FromStream(fs);

                        Bitmap bmp = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.Clear(Color.Transparent);
                            g.DrawImage(original, 0, 0, original.Width, original.Height);
                        }

                        pbCarImage.Image = bmp;
                        original.Dispose();
                    }
                }
                else
                {
                    CreatePlaceholderImage("Image Not Found", Path.GetFileName(imagePath));
                }
            }
            catch (Exception ex)
            {
                CreatePlaceholderImage("Error", ex.Message);
            }
        }

        private void CreatePlaceholderImage(string line1, string line2)
        {
            Bitmap placeholder = new Bitmap(600, 250);
            using (Graphics g = Graphics.FromImage(placeholder))
            {
                g.Clear(Color.FromArgb(64, 68, 75));
                using (Font font1 = new Font("Segoe UI", 16, FontStyle.Bold))
                using (Font font2 = new Font("Segoe UI", 10))
                using (Brush brush = new SolidBrush(Color.White))
                using (Brush brush2 = new SolidBrush(Color.FromArgb(185, 187, 190)))
                {
                    StringFormat sf = new StringFormat();
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;

                    g.DrawString(line1, font1, brush, new Rectangle(0, 80, 600, 50), sf);
                    g.DrawString(line2, font2, brush2, new Rectangle(0, 140, 600, 30), sf);
                }
            }
            pbCarImage.Image = placeholder;
        }

        private void timerCarousel_Tick(object sender, EventArgs e)
        {
            if (cars == null || cars.Count == 0) return;
            currentIndex = (currentIndex + 1) % cars.Count;
            ShowCar(currentIndex);
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (cars == null || cars.Count == 0) return;
            timerCarousel.Stop();
            currentIndex--;
            if (currentIndex < 0) currentIndex = cars.Count - 1;
            ShowCar(currentIndex);
            timerCarousel.Start();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (cars == null || cars.Count == 0) return;
            timerCarousel.Stop();
            currentIndex = (currentIndex + 1) % cars.Count;
            ShowCar(currentIndex);
            timerCarousel.Start();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (cars == null || cars.Count == 0)
            {
                MessageBox.Show("No cars available to select.", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Car selectedCar = cars[currentIndex];
            PurchaseForm purchaseForm = new PurchaseForm(selectedCar, _username);
            purchaseForm.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?",
                "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                LoginForm login = new LoginForm();
                login.Show();
                this.Close();
            }
        }

        // Navigation button hover effects
        private void btnPrevious_MouseEnter(object sender, EventArgs e) => btnPrevious.BackColor = Color.FromArgb(94, 99, 107);
        private void btnPrevious_MouseLeave(object sender, EventArgs e) => btnPrevious.BackColor = Color.FromArgb(64, 68, 75);
        private void btnNext_MouseEnter(object sender, EventArgs e) => btnNext.BackColor = Color.FromArgb(94, 99, 107);
        private void btnNext_MouseLeave(object sender, EventArgs e) => btnNext.BackColor = Color.FromArgb(64, 68, 75);
        private void btnSelect_MouseEnter(object sender, EventArgs e) => btnSelect.BackColor = Color.FromArgb(217, 55, 55);
        private void btnSelect_MouseLeave(object sender, EventArgs e) => btnSelect.BackColor = Color.FromArgb(240, 71, 71);

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Application.Exit();
            }
        }
    }

    public class Car
    {
        public int CarID { get; set; }
        public string ModelName { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
    }
}