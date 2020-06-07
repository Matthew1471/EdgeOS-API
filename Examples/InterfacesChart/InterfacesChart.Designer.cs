namespace InterfacesChart
{
    partial class InterfacesChart
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InterfacesChart));
            this.bandwidthChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.bandwidthChart)).BeginInit();
            this.SuspendLayout();
            // 
            // bandwidthChart
            // 
            this.bandwidthChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.AlignWithChartArea = "ChartAreaRx";
            chartArea1.AxisX.LabelStyle.Enabled = false;
            chartArea1.Name = "ChartAreaTx";
            chartArea2.AlignWithChartArea = "ChartAreaTx";
            chartArea2.AxisX.LabelStyle.Enabled = false;
            chartArea2.Name = "ChartAreaRx";
            this.bandwidthChart.ChartAreas.Add(chartArea1);
            this.bandwidthChart.ChartAreas.Add(chartArea2);
            legend1.LegendItemOrder = System.Windows.Forms.DataVisualization.Charting.LegendItemOrder.ReversedSeriesOrder;
            legend1.Name = "Legend1";
            this.bandwidthChart.Legends.Add(legend1);
            this.bandwidthChart.Location = new System.Drawing.Point(13, 13);
            this.bandwidthChart.Name = "bandwidthChart";
            this.bandwidthChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.None;
            this.bandwidthChart.Size = new System.Drawing.Size(479, 416);
            this.bandwidthChart.TabIndex = 0;
            this.bandwidthChart.Text = "chart1";
            title1.Name = "Title1";
            title1.Text = "Interfaces";
            this.bandwidthChart.Titles.Add(title1);
            // 
            // InterfacesChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 441);
            this.Controls.Add(this.bandwidthChart);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InterfacesChart";
            this.Text = "EdgeOS Bandwidth Chart";
            this.Load += new System.EventHandler(this.FormBandwidthChart_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bandwidthChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart bandwidthChart;
    }
}

