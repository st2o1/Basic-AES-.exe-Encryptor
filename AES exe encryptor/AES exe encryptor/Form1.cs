using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AES_exe_encryptor
{
    public partial class Form1 : Form
    {
        private string selectedFilePath;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string generatedKey = GenAESKey();
                textBox2.Text = generatedKey;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenAESKey()
        {
            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                aesProvider.GenerateKey();
                return Convert.ToBase64String(aesProvider.Key);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Executable Files|*.exe|All Files|*.*";
                openFileDialog.Title = "Choose an executable file";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFilePath = openFileDialog.FileName;
                    textBox1.Text = selectedFilePath;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                try
                {
                    string keyText = textBox2.Text;

                    if (string.IsNullOrEmpty(keyText))
                    {
                        MessageBox.Show("Please enter a key in the textbox.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    byte[] key = Encoding.UTF8.GetBytes(keyText);
                    Array.Resize(ref key, 32);

                    using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
                    {
                        aesProvider.Key = key;

                        using (FileStream inputFileStream = new FileStream(selectedFilePath, FileMode.Open, FileAccess.Read))
                        {
                            using (FileStream encryptedFileStream = new FileStream(selectedFilePath + ".exe", FileMode.Create, FileAccess.Write))
                            {
                                inputFileStream.Seek(0, SeekOrigin.Begin);
                                inputFileStream.CopyTo(encryptedFileStream, 64);
                                using (CryptoStream cryptoStream = new CryptoStream(encryptedFileStream, aesProvider.CreateEncryptor(), CryptoStreamMode.Write))
                                {
                                    inputFileStream.Seek(64, SeekOrigin.Begin);
                                    inputFileStream.CopyTo(cryptoStream);
                                }
                            }
                        }
                    }

                    MessageBox.Show("File encrypted successfully!", "Encryption Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please choose an executable file first.", "File Not Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
