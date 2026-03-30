using Social.Core.Entities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TweetingPlatform
{
    /// <summary>
    /// Instagram story bubble-тэй төстэй custom control.
    /// Story-г үзээгүй үед цэнхэр хүрээтэй,
    /// үзсэн үед саарал хүрээтэй харагдана.
    /// </summary>
    public class StoryButton : Control
    {
        /// <summary>
        /// Энэ товч дээр холбогдсон story объект.
        /// </summary>
        private Story story;

        /// <summary>
        /// Story мэдээлэл.
        /// </summary>
        [Browsable(false)]
        public Story StoryData
        {
            get { return story; }
            set
            {
                story = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Story дээр дарахад ажиллах event.
        /// </summary>
        public event EventHandler StoryClicked;

        /// <summary>
        /// StoryButton constructor.
        /// </summary>
        public StoryButton()
        {
            this.Size = new Size(90, 110);
            this.BackColor = Color.White;
            this.Cursor = Cursors.Hand;
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// Story bubble-ийг өөрөө зурж харуулна.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (story == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Color borderColor = story.IsViewed ? Color.Gray : Color.DeepSkyBlue;

            // Гаднах хүрээ
            using (Pen pen = new Pen(borderColor, 4))
            {
                g.DrawEllipse(pen, 15, 10, 55, 55);
            }

            // Доторх profile circle
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(230, 230, 230)))
            {
                g.FillEllipse(brush, 22, 17, 41, 41);
            }

            // Username-ийн эхний 2 үсэг
            string initials = story.Username.Length >= 2
                ? story.Username.Substring(0, 2).ToUpper()
                : story.Username.ToUpper();

            using (Font font = new Font("Segoe UI", 10, FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            {
                SizeF size = g.MeasureString(initials, font);
                g.DrawString(initials, font, textBrush,
                    42 - size.Width / 2, 37 - size.Height / 2);
            }

            // Username
            using (Font nameFont = new Font("Segoe UI", 9, FontStyle.Regular))
            using (SolidBrush nameBrush = new SolidBrush(Color.Black))
            {
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;

                g.DrawString(story.Username, nameFont, nameBrush,
                    new RectangleF(5, 72, 75, 25), sf);
            }
        }

        /// <summary>
        /// Story bubble дээр дарахад StoryClicked event дуудагдана.
        /// </summary>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            StoryClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}