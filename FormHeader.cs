using System;
using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks; 
using System.Windows.Forms;

namespace WindowsFormsApplication
{
    public partial class FormHeader : Form
    {
		private List<CheckBox> lCheckBox = new List<CheckBox>();
		public uint nNandHeader;

        public FormHeader(){
            InitializeComponent();
        }

		private void buttonOk_Click(object sender, EventArgs e)
		{
			uint bitMask = 0x80000000;
			uint nNewHeader = 0;

			for(int i=0;i<32;i++){
				if(lCheckBox[i].Checked){
					nNewHeader = nNewHeader | bitMask;
				}
				bitMask = bitMask >> 1;
			}
			nNandHeader = nNewHeader;

			this.Close();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}
    }
}

namespace WindowsFormsApplication
{
    partial class FormHeader : Form{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

		private void InitializeComponent()
        {
			Button buttonOk = new Button();
			Button buttonCancel = new Button();

			this.SuspendLayout();

			// FormGui
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "SAMA5D2 NAND header";
			this.ClientSize = new System.Drawing.Size(640, 320);
			this.Controls.Add(buttonOk);
			this.Controls.Add(buttonCancel);

			// Button [OK]
			buttonOk.Location = new System.Drawing.Point(25, 280);
			buttonOk.Name = "buttonOk";
			buttonOk.Size = new System.Drawing.Size(75, 23);
			buttonOk.TabIndex = 1;
			buttonOk.Text = "OK";
			buttonOk.UseVisualStyleBackColor = true;
			buttonOk.Click += new System.EventHandler(this.buttonOk_Click);

			// Button [CANCEL]
			buttonCancel.Location = new System.Drawing.Point(110, 280);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new System.Drawing.Size(75, 23);
			buttonCancel.TabIndex = 2;
			buttonCancel.Text = "CANCEL";
			buttonCancel.UseVisualStyleBackColor = true;
			buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);

			// Checkbox BIT[31:0]
			for(int j=0;j<4;j++){
				for(int i=0;i<8;i++){
					lCheckBox.Add(new CheckBox());
					lCheckBox[i+(8*j)].Location = new System.Drawing.Point(20+(i*70), 20 + (j*50));
					lCheckBox[i+(8*j)].Name = "writeEnable";
					lCheckBox[i+(8*j)].TabIndex = 2+i+(8*j);
					lCheckBox[i+(8*j)].Text = "Bit" + (31 - (i+(8*j))).ToString();
					lCheckBox[i+(8*j)].Size = new System.Drawing.Size(65, 23);
					lCheckBox[i+(8*j)].CheckAlign = ContentAlignment.BottomLeft;
					lCheckBox[i+(8*j)].TextAlign = ContentAlignment.TopLeft;
					this.Controls.Add(lCheckBox[i+(8*j)]);
				}
			}

			lCheckBox[0].Checked = true;
			lCheckBox[0].Enabled = false;
			lCheckBox[1].Checked = true;
			lCheckBox[1].Enabled = false;
			lCheckBox[2].Enabled = false;
			lCheckBox[3].Enabled = false;
			lCheckBox[4].Enabled = false;
		}

		/*
		 * Name		: getHeader
		 */
		public uint GetHeader(){
			return nNandHeader;
		}

		/*
		 * Name		: setHeader
		 */
		public void SetHeader(uint nHeader){
			nNandHeader = nHeader;
		}

		public void SetCheck(uint nNandHeaderValue){
			uint bitMask = 0x80000000;

			for(int i=0;i<32;i++){
				if((bitMask & nNandHeader) > 0){
					lCheckBox[i].Checked = true;
				}
				bitMask = bitMask >> 1;
			}
		}

		#endregion
    }
}
