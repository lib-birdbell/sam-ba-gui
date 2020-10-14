// ÆÄÀÏ FormGui.cs
using System;
using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication
{
    public partial class FormGui : Form
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName,string lpWindowName);

        [DllImport("user32.dll")]       
        static  extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
		private List<CheckBox> lCheckBox = new List<CheckBox>();
		private List<TextBox> lTextBoxAddress = new List<TextBox>();
		private List<TextBox> lTextBoxSize = new List<TextBox>();
		private List<TextBox> lTextBoxFile = new List<TextBox>();
		private int mtdCount;
		
        public FormGui()
        {
			// Hide console window
			IntPtr hWnd = FindWindow(null, Console.Title);
			if(hWnd != IntPtr.Zero){
				ShowWindow(hWnd, 0/*SW_HIDE*/);
			}

            InitializeComponent();
        }

		private void buttonErase_Click(object sender, EventArgs e){
			if(MessageBox.Show("The entire Nand is erased and cannot be recovered!", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes){
				string strappname = Application.StartupPath + "\\erase.bat";
				Process.Start(strappname);
				MessageBox.Show("Nand deleted.");
			}else{
				MessageBox.Show("Safely canceled.");
			}
		}

		private void buttonProgram_Click(object sender, EventArgs e){
			string batFile = Application.StartupPath + "\\program.bat";
			//string executeText = "echo \"tetest\"\r\nPAUSE";
			//string header = "import SAMBA 3.2\r\nimport SAMBA.Connection.Serial 3.2\r\nimport SAMBA.Device.SAMA5D2 3.2\r\n\r\nSerialConnection {\r\n	device: SAMA5D2 {\r\n		config {\r\n			nandflash {\r\n				ioset: 2\r\n				busWidth: 8\r\n				header: 0xc0902405\r\n			}\r\n		}\r\n	}\r\n\r\n	onConnectionOpened: {\r\n		////////////////////	init	////////////////////\r\n		// initialize NAND flash applet\r\n		initializeApplet(\"nandflash\")\r\n";
			string header = "import SAMBA 3.2\r\nimport SAMBA.Connection.Serial 3.2\r\nimport SAMBA.Device.SAMA5D2 3.2\r\n\r\nSerialConnection {\r\n	device: SAMA5D2 {\r\n		config {\r\n			nandflash {\r\n				ioset: 2\r\n				busWidth: 8\r\n				header: 0xc0c00405\r\n			}\r\n		}\r\n	}\r\n\r\n	onConnectionOpened: {\r\n		////////////////////	init	////////////////////\r\n		// initialize NAND flash applet\r\n		initializeApplet(\"nandflash\")\r\n";
			string textProgram = "";
			string tail = "	}\r\n}";

			// File open
			StreamWriter outputFile = new StreamWriter(Application.StartupPath + "\\nand-usb.qml");

			// Write header
			outputFile.WriteLine(header);

			// Read status
			for(int i=0;i<lCheckBox.Count;i++){
				if(lCheckBox[i].Text == "bootstrap"){
					continue;
				}
				if(lCheckBox[i].Checked){
					textProgram += "		//";
					textProgram += lCheckBox[i].Text;
					textProgram += "\r\n";
					textProgram += "		applet.erase(";
					textProgram += lTextBoxAddress[i].Text;
					textProgram += ", ";
					textProgram += lTextBoxSize[i].Text;
					textProgram += ")\r\n";
					textProgram += "		applet.write(";
					textProgram += lTextBoxAddress[i].Text;
					textProgram += ", \"";
					textProgram += lTextBoxFile[i].Text;
					textProgram += "\")\r\n";
				}
			}
			for(int i=0;i<lCheckBox.Count;i++){
				if(lCheckBox[i].Text == "bootstrap"){
					if(lCheckBox[i].Checked){
						textProgram += "		//";
						textProgram += lCheckBox[i].Text;
						textProgram += "\r\n";
						textProgram += "		applet.erase(";
						textProgram += lTextBoxAddress[i].Text;
						textProgram += ", ";
						textProgram += lTextBoxSize[i].Text;
						textProgram += ")\r\n";
						textProgram += "		applet.write(";
						textProgram += lTextBoxAddress[i].Text;
						textProgram += ", \"";
						textProgram += lTextBoxFile[i].Text;
						textProgram += "\", true)\r\n";
					}
				}
			}
			outputFile.WriteLine(textProgram);

			// Write tail
			outputFile.WriteLine(tail);

			outputFile.Close();

			// Run program
			Process.Start(Application.StartupPath + "\\program.bat");
			/*
			//using (StreamWriter outputFile = new StreamWrite(Application.StartupPath + "\\nand-usb.qml"){
			//StreamWriter outputfile = new StreamWriter(new FileStream(Application.StartupPath + "\\program.bat"), fileMode.Create);
			//StreamWriter outputFile = new StreamWriter("program.bat");
			StreamWriter outputFile = new StreamWriter(Application.StartupPath + "\\program.bat");
			outputFile.WriteLine(executeText);
			outputFile.Close();

			Process.Start(batFile);
			//MessageBox.Show("PROGRAM!");
			*/
		}

		private void buttonLoad_Click(object sender, EventArgs e){
			// File open
			OpenFileDialog ofd = new OpenFileDialog();

			ofd.DefaultExt = "xml";
			ofd.Filter = "xml files (*.xml)|*.xml";
			ofd.ShowDialog();

			// File parsing
			if(ofd.FileName.Length > 0){
				string text = ofd.FileName;
				string temp = "";

				XmlDocument xml = new XmlDocument();
				xml.Load(@text);

				XmlElement nand = xml.DocumentElement;
				temp = nand.Name;		// TOP NAME
				if((temp != "NAND") && (temp != "nand") && (temp != "Nand")){
					//Console.WriteLine("NAND = " + temp);
					MessageBox.Show("Invalid XML file.");
					return;
				}

				XmlElement xmlNand = (XmlElement)nand.FirstChild;

				// Initial data
				mtdCount = 0;
				delList();

				while( (xmlNand != null) && ( xmlNand.IsEmpty == false) ){
					string attr = xmlNand.GetAttribute("MTD");
					string name = "";
					string address = "";
					string size = "";
					string file = "";

					XmlNodeList nlChilds = xmlNand.ChildNodes;

					foreach(XmlElement eNode in nlChilds){
						if(eNode.LocalName.ToLower().Contains("name") == true){
							name = eNode.InnerText;
						}
						if(eNode.LocalName.ToLower().Contains("address") == true){
							address = eNode.InnerText;
						}
						if(eNode.LocalName.ToLower().Contains("size") == true){
							size = eNode.InnerText;
						}
						if(eNode.LocalName.ToLower().Contains("file") == true){
							file = eNode.InnerText;
						}
					}

					// Add check box
					lCheckBox.Add(new CheckBox());
					lCheckBox[mtdCount].Location = new System.Drawing.Point(10, 60 + (mtdCount*20));
					lCheckBox[mtdCount].Name = "writeEnable";
					lCheckBox[mtdCount].Size = new System.Drawing.Size(90, 20);
					lCheckBox[mtdCount].TabIndex = 4+(mtdCount*4)+0;
					lCheckBox[mtdCount].Text = name;
					this.Controls.Add(lCheckBox[mtdCount]);

					// Add address
					lTextBoxAddress.Add(new TextBox());
					lTextBoxAddress[mtdCount].Location = new System.Drawing.Point(110, 60 + (mtdCount*20));
					lTextBoxAddress[mtdCount].Name = "textAddress";
					lTextBoxAddress[mtdCount].Size = new System.Drawing.Size(90, 20);
					lTextBoxAddress[mtdCount].TabIndex = 4+(mtdCount*4)+1;
					lTextBoxAddress[mtdCount].Text = address;
					this.Controls.Add(lTextBoxAddress[mtdCount]);

					// Add size
					lTextBoxSize.Add(new TextBox());
					lTextBoxSize[mtdCount].Location = new System.Drawing.Point(200, 60 + (mtdCount*20));
					lTextBoxSize[mtdCount].Name = "textSize";
					lTextBoxSize[mtdCount].Size = new System.Drawing.Size(90, 20);
					lTextBoxSize[mtdCount].TabIndex = 4+(mtdCount*4)+1;
					lTextBoxSize[mtdCount].Text = size;
					this.Controls.Add(lTextBoxSize[mtdCount]);

					// Add file
					lTextBoxFile.Add(new TextBox());
					lTextBoxFile[mtdCount].Location = new System.Drawing.Point(290, 60 + (mtdCount*20));
					lTextBoxFile[mtdCount].Name = "textSize";
					lTextBoxFile[mtdCount].Size = new System.Drawing.Size(300, 20);
					lTextBoxFile[mtdCount].TabIndex = 4+(mtdCount*4)+1;
					lTextBoxFile[mtdCount].Text = file;
					this.Controls.Add(lTextBoxFile[mtdCount]);

					//Console.WriteLine(name + address + size + file);
					mtdCount++;

					xmlNand = (XmlElement)xmlNand.NextSibling;
				}
			}else{
				return;
			}
			/*
			delList();

			Console.WriteLine(mtdCount);
			for(int i=0;i<mtdCount;i++){
				Console.WriteLine("mkCheckBox() new check box" + i);
				lCheckBox.Add(new CheckBox());
				lCheckBox[i].Location = new System.Drawing.Point(10, 40 + (i*20));
				lCheckBox[i].Name = "writeEnable";
				lCheckBox[i].TabIndex = 4+i;
				lCheckBox[i].Text = "checkcheck";
				this.Controls.Add(lCheckBox[i]);
			}
			mtdCount++;
			Console.WriteLine(mtdCount);
			//MessageBox.Show("Hello wellcome hell!");
			*/
		}

		private void buttonSave_Click(object sender, EventArgs e){
			MessageBox.Show("Hello wellcome hell! SAVE");
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.DefaultExt = "xml";
			ofd.Filter = "xml files (*.xml)|*.xml";
			ofd.ShowDialog();
			if(ofd.FileName.Length > 0){
				string text = ofd.FileName;
				string temp = "";

				MessageBox.Show(text);
/*
				XmlTextReader reader = new XmlTextReader(text);
				
				while(reader.Read()) {
					switch(reader.NodeType){
						case XmlNodeType.Element:	// The node is an element.
							Console.WriteLine("<" + reader.Name);
							Console.WriteLine(">");
							break;
						case XmlNodeType.Text:	// Display the text in each element.
							Console.WriteLine(reader.Value);
							break;
						case XmlNodeType.EndElement:	// Display the end of the element.
							Console.WriteLine("[" + reader.Name);
							Console.WriteLine("]");
							break;
					}
				}
*/
				XmlDocument xml = new XmlDocument();
				xml.Load(@text);
				//xml.Load(@"E:\\DATA\\Product\\MS-281\\document\\MYD-JA5D2X\\download\\myir.xml");
				//xml.Load(@"E:\DATA\Product\MS-281\document\MYD-JA5D2X\download\myir.xml");
				/*
				XmlTextWriter textWriter = new XmlTextWriter(@"E:\DATA\Product\MS-281\document\MYD-JA5D2X\download\myir2.xml", Encoding.UTF8);
				textWriter.WriteStartDocument();
				textWriter.WriteStartElement("root");
				textWriter.WriteStartElement("root_a");
				textWriter.WriteString("1234");
				textWriter.WriteEndElement();
				textWriter.WriteEndDocument();
				textWriter.Close();
				*/

				//XmlNodeList xmlList = xml.SelectNodes("NAND");

				XmlElement nand = xml.DocumentElement;
				temp = nand.Name;		// TOP NAME
				//Console.WriteLine(temp);
				
				XmlElement xmlNand = (XmlElement)nand.FirstChild;
				
				while( (xmlNand != null) && ( xmlNand.IsEmpty == false) ){
					string attr = xmlNand.GetAttribute("MTD");
					XmlNodeList nlChilds = xmlNand.ChildNodes;
					
					foreach(XmlElement eNode in nlChilds){
						if(eNode.LocalName.ToLower().Contains("name") == true){
							string value = eNode.InnerText;
							//Console.WriteLine(value);
						}
						if(eNode.LocalName.ToLower().Contains("address") == true){
							string value = eNode.InnerText;
							//Console.WriteLine(value);
						}
						if(eNode.LocalName.ToLower().Contains("size") == true){
							string value = eNode.InnerText;
							//Console.WriteLine(value);
						}
					}
					
					xmlNand = (XmlElement)xmlNand.NextSibling;
				}
				//XmlNodeList nlChilds = xmlNand.ChildNodes;
				/*
				for(int i=0;i<nlChilds.Count;i++){
					XmlElement eChild = (XmlElement)nlChilds[i];
					Console.WriteLine(eChild.Name + ";" + eChild.InnerText);
				}
*/
/*
				foreach (XmlNode node in xmlList.SelectNodes("MTD")){
					temp = node.Name;
					Console.WriteLine(temp);

					temp = node.InnerText;
					Console.WriteLine(temp);

					//temp = node["address"].InnerText;
					temp += node["name"].InnerText;
					temp += node["address"].InnerText;
					temp += node["size"].InnerText;
					//MessageBox.Show(temp);
					temp = "";
				}
*/
				MessageBox.Show(text);
			}
		}
    }
}


// File FormGui.Designer.cs
namespace WindowsFormsApplication
{
    partial class FormGui
    {
        private System.ComponentModel.IContainer components = null;
		private Button buttonLoad;

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
			//System.Windows.Forms.Button buttonLoad;
			this.buttonLoad = new System.Windows.Forms.Button();
			Button buttonErase = new Button();
			Button buttonProgram = new Button();
			Button buttonSave = new Button();
			Label labelName = new Label();
			Label labelAddress = new Label();
			Label labelSize = new Label();
			Label labelFile = new Label();

			this.SuspendLayout();

			// FormGui
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "Atmel nand flash";
			this.ClientSize = new System.Drawing.Size(640, 320);
			this.Controls.Add(this.buttonLoad);
			this.Controls.Add(buttonErase);
			this.Controls.Add(buttonProgram);
			this.Controls.Add(buttonSave);
			this.Controls.Add(labelName);
			this.Controls.Add(labelAddress);
			this.Controls.Add(labelSize);
			this.Controls.Add(labelFile);

			// Button [ERASE]
			buttonErase.Location = new System.Drawing.Point(10, 10);
			//this.button.Location = new Point(367, 174);
			buttonErase.Name = "buttonErase";
			buttonErase.Size = new System.Drawing.Size(75, 23);
			buttonErase.TabIndex = 0;
			buttonErase.Text = "ERASE";
			buttonErase.UseVisualStyleBackColor = true;
			buttonErase.Click += new System.EventHandler(this.buttonErase_Click);

			// Button [PROGRAM]
			buttonProgram.Location = new System.Drawing.Point(95, 10);
			buttonProgram.Name = "buttonProgram";
			buttonProgram.Size = new System.Drawing.Size(75, 23);
			buttonProgram.TabIndex = 1;
			buttonProgram.Text = "PROGRAM";
			buttonProgram.UseVisualStyleBackColor = true;
			buttonProgram.Click += new System.EventHandler(this.buttonProgram_Click);

			// Button [LOAD]
			this.buttonLoad.Location = new System.Drawing.Point(180, 10);
			//this.buttonLoad.Location = new Point(367, 174);
			this.buttonLoad.Name = "buttonLoad";
			this.buttonLoad.Size = new System.Drawing.Size(75, 23);
			this.buttonLoad.TabIndex = 2;
			this.buttonLoad.Text = "LOAD";
			this.buttonLoad.UseVisualStyleBackColor = true;
			this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
			
			// Button [SAVE]
			buttonSave.Location = new System.Drawing.Point(265, 10);
			buttonSave.Name = "buttonSave";
			buttonSave.Size = new System.Drawing.Size(75, 23);
			buttonSave.TabIndex = 3;
			buttonSave.Text = "SAVE";
			buttonSave.UseVisualStyleBackColor = true;
			buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
			buttonSave.Enabled = false;
			
			// Label
			labelName.Location = new System.Drawing.Point(30, 40);
			labelName.Name = "labelName";
			labelName.Size = new System.Drawing.Size(55, 20);
			labelName.Text = "NAME";

			labelAddress.Location = new System.Drawing.Point(110, 40);
			labelAddress.Name = "labelAddress";
			labelAddress.Size = new System.Drawing.Size(55, 20);
			labelAddress.Text = "ADDRESS";
			
			labelSize.Location = new System.Drawing.Point(200, 40);
			labelSize.Name = "labelSize";
			labelSize.Size = new System.Drawing.Size(55, 20);
			labelSize.Text = "SIZE";
			
			labelFile.Location = new System.Drawing.Point(290, 40);
			labelFile.Name = "labelFile";
			labelFile.Size = new System.Drawing.Size(55, 20);
			labelFile.Text = "FILE";
			
			this.ResumeLayout(false);
        }

        #endregion



		/*
		 * Name		: delList
		 * Remarks	: Button delete
		 */
		private void delList(){
			for(int i=0;i<lCheckBox.Count;i++){
				//Console.WriteLine("delList() Remove() " + i);
				this.Controls.Remove(lCheckBox[i]);
			}
			for(int i=0;i<lCheckBox.Count;i++){
				//Console.WriteLine("delList() RemoveAt() " + i);
				lCheckBox.RemoveAt(i);
			}
			//Console.WriteLine("delList() done");
		}
    }
}

