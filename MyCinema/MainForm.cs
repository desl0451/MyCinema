using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MyCinema
{
    public partial class MainForm : Form
    {
        Cinema cinema = null;
        string key = "";
        Dictionary<string, Label> labels = new Dictionary<string, Label>();


        public MainForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        /// <summary>
        /// 获取放映列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiNew_Click(object sender, EventArgs e)
        {
            cinema.Schedule.LoadItems();
            cinema.SoldTickets.Clear();
            InitTreeView();

        }
        
        #region 设置票种类是否可用
        private void TicketControl()
        {
            groupBox2.Enabled = true;
        }
        #endregion


        #region 初始化TreeView控件
        private void InitTreeView()
        {
            tvMovies.BeginUpdate();//禁用任何数视图重绘
            tvMovies.Nodes.Clear();//清空树视图
            string movieName = null;
            TreeNode movieNode = null;
            foreach (ScheduleItem item in cinema.Schedule.Items.Values)
            {
                if (movieName != item.Movie.MovieName)
                {
                    movieNode = new TreeNode(item.Movie.MovieName);
                    tvMovies.Nodes.Add(movieNode);
                }
                TreeNode timeNode = new TreeNode(item.Time);
                movieNode.Nodes.Add(timeNode);
                movieName = item.Movie.MovieName;
            }
            tvMovies.EndUpdate();
        }
        #endregion
        
        /// <summary>
        /// 用例2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvMovies_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = tvMovies.SelectedNode;
            if (node == null) return;
            if (node.Level != 1) return;
            key = node.Text;
            this.lblMovieName.Text = cinema.Schedule.Items[key].Movie.MovieName;

            this.lblDirector.Text = cinema.Schedule.Items[key].Movie.Director;

            this.lblActor.Text = cinema.Schedule.Items[key].Movie.Actor;

            this.lblPrice.Text = cinema.Schedule.Items[key].Movie.Price.ToString();

            this.lblTime.Text = cinema.Schedule.Items[key].Time;

            this.picMovie.Image = Image.FromFile("film/" + cinema.Schedule.Items[key].Movie.Poster);
            this.lblCalcPrice.Text = "";
            //设置是否可以选择票种类
            TicketControl();
            
            //遍历该场电影的已售集合
            foreach (Ticket ticket in cinema.SoldTickets)
            {
                //遍历所有座位
                foreach (Seat seat in cinema.Seats.Values)
                {
                    if ((ticket.ScheduleItem.Time == key) && (ticket.Seat.SeatNum == seat.SeatNum))
                    {
                        seat.Color = Color.Red;
                    }
                }
            }
            
        }
        /// <summary>
        /// 学生票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoStudent_CheckedChanged(object sender, EventArgs e)
        {
            this.txtCustomer.Enabled = false;
            this.txtCustomer.Text = "";
            this.cmbDisCount.Enabled = true;
            this.cmbDisCount.Text = "7";

            //根据当前选中的电影,设置"优惠价"
            if (this.lblPrice.Text != "")
            {
                int price = int.Parse(this.lblPrice.Text);
                int discount = int.Parse(this.cmbDisCount.Text);
                this.lblCalcPrice.Text = (price * discount / 10).ToString();
            }
        }
        /// <summary>
        /// 赠票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoFree_CheckedChanged(object sender, EventArgs e)
        {
            this.txtCustomer.Enabled = true;
            this.cmbDisCount.Enabled = false;
            this.cmbDisCount.Text = "";
            //设置"优惠价"
            this.lblCalcPrice.Text = "0";
        }
        /// <summary>
        /// 普通票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdoNormal_CheckedChanged(object sender, EventArgs e)
        {
            this.txtCustomer.Enabled = false;
            this.txtCustomer.Text = "";
            this.cmbDisCount.Enabled = false;
            this.cmbDisCount.Text = "";

            this.lblCalcPrice.Text = this.lblPrice.Text;
        }
        /// <summary>
        /// 窗体加载时显示座位号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            cinema = new Cinema();
            cinema.Load();
            
            
            
            InitSeat(7, 5, tpCinema);
        }

        /// <summary>
        /// 初始化座位信息
        /// </summary>
        /// <param name="seatRow"></param>
        /// <param name="seatLine"></param>
        /// <param name="tb"></param>
        private void InitSeat(int seatRow, int seatLine, TabPage tb)
        {
            Label label = null;

            Seat seat = null;
            for (int i = 0; i < seatRow; i++) //行
            {
                for (int j = 0; j < seatLine; j++) //列
                {
                    label = new Label();
                    //设置背景颜色
                    label.BackColor = Color.Yellow;
                    //设置字体
                    label.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                    //设置尺寸
                    label.AutoSize = false;
                    label.Size = new System.Drawing.Size(50, 25);
                    //设置座位号
                    label.Text = (j + 1).ToString() + "-" + (i + 1).ToString();
                    label.TextAlign = ContentAlignment.MiddleCenter;
                    //设置位置
                    label.Location = new Point(60 + (i * 90), 60 + (j * 60));
                    //所有的标签都绑定同一事件
                    label.Click += new System.EventHandler(lblSeat_Click);
                    tb.Controls.Add(label);
                    labels.Add(label.Text, label);
                    //实例化一个座位,Seat构造函数的参数为座位号和颜色
                    seat = new Seat((j + 1).ToString() + "-" + (i + 1).ToString(), Color.Yellow);
                    //保存的座位集合
                    cinema.Seats.Add(seat.SeatNum, seat);
                }
            }
        }
        /// <summary>
        /// 编写单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblSeat_Click(object sender, EventArgs e)
        {
            try
            {
                string seatNum = ((Label)sender).Text.ToString();
                string customerName = this.txtCustomer.Text.ToString();
                int discount = 0;
                string type = "";
                if (this.rdoStudent.Checked)
                {
                    type = "student";
                    if (this.cmbDisCount.Text == null)
                    {
                        MessageBox.Show("请输入折扣数!", "提示");
                        return;
                    }
                    else
                    {
                        discount = int.Parse(this.cmbDisCount.Text);
                    }
                }
                else if (this.rdoFree.Checked)
                {
                    type = "free";
                    if (string.IsNullOrEmpty(this.txtCustomer.Text))
                    {
                        MessageBox.Show("请输入赠票者姓名!", "提示");
                        return;
                    }
                }
                else
                {
                    type = "";
                }
                //调用工具类创建票
                Ticket newTicket = TicketUtil.CreateTicket(cinema.Schedule.Items[key], cinema.Seats[seatNum], discount, customerName, type);
                if (cinema.Seats[seatNum].Color == Color.Yellow)
                {
                    //打印
                    DialogResult result = MessageBox.Show("是否购买?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        cinema.Seats[seatNum].Color = Color.Red;
                        UpdateSeat();
                        newTicket.CalcPrice();
                        cinema.SoldTickets.Add(newTicket);
                        lblCalcPrice.Text = newTicket.Price.ToString();
                        newTicket.Print();
                    }
                    else if (result == DialogResult.No)
                    {
                        return;
                    }
                }
                else
                {
                    //显示当前售出票的信息
                    foreach (Ticket ticket0 in cinema.SoldTickets)
                    {
                        if (ticket0.Seat.SeatNum == seatNum && ticket0.ScheduleItem.Time == tvMovies.SelectedNode.Text && ticket0.ScheduleItem.Movie.MovieName == tvMovies.SelectedNode.Parent.Text)
                        {
                            ticket0.Show();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #region 更新座位状态
        private void UpdateSeat()
        {
            foreach (string key in cinema.Seats.Keys)
            {
                labels[key].BackColor = cinema.Seats[key].Color;
            }
        }
        #endregion
    }
}
