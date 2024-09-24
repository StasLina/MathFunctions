namespace MathFunctions
{
    partial class MainWindowForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            GBGraph = new GroupBox();
            SuspendLayout();
            // 
            // GBGraph
            // 
            GBGraph.Location = new Point(331, 84);
            GBGraph.Name = "GBGraph";
            GBGraph.Size = new Size(351, 247);
            GBGraph.TabIndex = 0;
            GBGraph.TabStop = false;
            // 
            // MainWindowForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(GBGraph);
            Name = "MainWindowForm";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private GroupBox GBGraph;
    }
}
